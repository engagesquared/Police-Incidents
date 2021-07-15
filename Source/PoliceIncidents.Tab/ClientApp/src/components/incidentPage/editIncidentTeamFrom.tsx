import * as React from "react";
import { Flex, Text, Button, Image, Layout, Divider, AddIcon, Dropdown, CloseIcon, DropdownItemProps } from "@fluentui/react-northstar";
import { useTranslation } from "react-i18next";
import { useStyles } from "./editIncidentTeamFrom.styles";
import { ErrorMessage } from "../form/errorMessage";
import { PeoplePicker } from "@microsoft/mgt-react";
import { useForm } from "react-hook-form";
import { updateTeamMember, reAssignIncident } from "../../apis/api-list";
import { IIncidentMemberInputModel, IUserRoleModel, IIncidentMemberModel } from "../../models";
import { useGlobalState } from "../../hooks/useGlobalState";
import { v4 as uuidv4 } from "uuid";

export interface IEditIncidentTeamFormProps {
    incidentId: number;
    incidentManager: string | undefined;
    incidentMembers: IIncidentMemberModel[];
    onCancel(): void;
    onUpdated(update: IIncidentMemberModel[], newManager?: string): void;
}

export interface UserRoleDropdownItemProps extends DropdownItemProps {
    role: IUserRoleModel;
}

interface ICustomEvent {
    detail: { id: string; displayName: string }[];
}

interface OtherRoleItem {
    id: string;
    validationMessage?: string;
    userId?: string;
    role?: IUserRoleModel;
}

export const EditIncidentTeamForm = (props: React.PropsWithChildren<IEditIncidentTeamFormProps>) => {
    const { t } = useTranslation();
    const [isLoading, setIsLoading] = React.useState(false);
    const [managerValidationError, setManagerValidationError] = React.useState<string>();
    const [allRoles, setAllRoles] = React.useState<UserRoleDropdownItemProps[]>([]);
    const classes = useStyles();
    const { state } = useGlobalState();

    const getDefaultValues = () => {
        const otherRoles: OtherRoleItem[] = (props.incidentMembers || []).map((x) => ({
            id: uuidv4(),
            userId: x.userId,
            role: state.userRoles.find((r) => r.id === x.roleId),
        }));
        otherRoles.forEach((x) => {
            x.validationMessage = getValidateOtherRoleMessage(x, otherRoles, props.incidentManager);
        });
        const result: {
            incidentManager?: string;
            otherRoles: OtherRoleItem[];
        } = {
            incidentManager: props.incidentManager,
            otherRoles: otherRoles,
        };
        return result;
    };

    const { handleSubmit, getValues, setValue, register, watch } = useForm({
        defaultValues: getDefaultValues(),
    });

    React.useEffect(() => {
        setAllRoles(
            state.userRoles.map((x) => {
                return {
                    role: x,
                    header: x.name,
                };
            })
        );
    }, [state.userRoles]);

    React.useEffect(() => {
        register({ name: "incidentManager" });
        register({ name: "otherRoles" });
    }, [getValues, register, t]);

    const { incidentManager, otherRoles } = watch();

    const onConfirm = handleSubmit(async () => {
        try {
            setIsLoading(true);
            const membersUpdate: IIncidentMemberInputModel[] = otherRoles
                .filter((x) => !!x.userId && !!x.role)
                .map((x) => ({
                    roleId: x.role!.id,
                    userId: x.userId!,
                }));

            const promises = [updateTeamMember(props.incidentId, membersUpdate)];
            if (incidentManager && props.incidentManager !== incidentManager) {
                promises.push(
                    reAssignIncident([
                        {
                            incidentId: props.incidentId,
                            incidentManagerId: incidentManager,
                        },
                    ])
                );
            }

            await Promise.all(promises);
            props.onUpdated(membersUpdate, incidentManager);
            props.onCancel();
        } catch (ex) {
            console.log(ex);
        } finally {
            setIsLoading(false);
        }
    });

    const onManagerChanged = (managerId: string | undefined) => {
        validateManager(managerId, otherRoles);
        const newValue = [...otherRoles];
        newValue.forEach((x) => {
            x.validationMessage = getValidateOtherRoleMessage(x, newValue, managerId);
        });
        setValue("otherRoles", newValue);
        setValue("incidentManager", managerId);
    };

    const onUserIdChanged = (id: string, newUserId?: string) => {
        const newValue = [...otherRoles];
        const index = newValue.findIndex((x) => x.id === id);
        newValue[index].userId = newUserId;
        newValue[index].validationMessage = getValidateOtherRoleMessage(newValue[index], newValue, incidentManager);
        validateManager(incidentManager, newValue);
        setValue("otherRoles", newValue);
    };

    const onUserRoleChanged = (id: string, role: IUserRoleModel | undefined) => {
        const newValue = [...otherRoles];
        const index = newValue.findIndex((x) => x.id === id);
        newValue[index].role = role;
        newValue[index].validationMessage = getValidateOtherRoleMessage(newValue[index], newValue, incidentManager);
        validateManager(incidentManager, newValue);
        setValue("otherRoles", newValue);
    };

    const onUserRoleDelete = (id: string) => {
        const newValue = [...otherRoles];
        newValue.splice(
            newValue.findIndex((x) => x.id === id),
            1
        );
        validateManager(incidentManager, newValue);
        setValue("otherRoles", newValue);
    };

    const onUserRoleAdd = () => {
        const newValue = [...otherRoles];
        const newItem: OtherRoleItem = { id: uuidv4() };
        newValue.push(newItem);
        newItem.validationMessage = getValidateOtherRoleMessage(newItem, newValue, incidentManager);
        validateManager(incidentManager, newValue);
        setValue("otherRoles", newValue);
    };

    const getValidateOtherRoleMessage = (value: OtherRoleItem, allUsers: OtherRoleItem[], managerId?: string): string | undefined => {
        if (!value.userId || !value.role) {
            return t("requiredValidationMessage");
        } else if (value.userId === managerId || allUsers.some((x) => x.id !== value.id && x.userId === value.userId)) {
            return t("multipleRolesValidationMessage");
        }
        return undefined;
    };

    const validateManager = (managerId: string | undefined, allUsers: OtherRoleItem[]) => {
        if (!managerId) {
            setManagerValidationError(t("requiredValidationMessage"));
        } else if (allUsers.some((x) => x.userId === managerId)) {
            setManagerValidationError(t("multipleRolesValidationMessage"));
        } else if (!!managerValidationError) {
            setManagerValidationError("");
        }
    };

    const isFormValid = React.useMemo(() => {
        return !(!incidentManager || otherRoles.some((x) => !!x.validationMessage));
    }, [otherRoles, incidentManager]);

    return (
        <Flex className={classes.container} column>
            <Flex gap="gap.medium" padding="padding.medium" style={{ margin: 15 }}>
                <Layout
                    styles={{
                        maxWidth: "50px",
                    }}
                    renderMainArea={() => <Image fluid src="/logo50.png" />}
                />
                <Flex column>
                    <Text content="Incident App" weight="bold" size="larger" />
                    <Text content={t("editTeamMemberTitle")} />
                </Flex>
            </Flex>
            <Divider size={3} color="brand" />
            <form onSubmit={onConfirm}>
                <Flex column padding="padding.medium" style={{ margin: 10 }}>
                    <Flex column gap="gap.medium" padding="padding.medium" style={{ margin: 10 }}>
                        <Flex column>
                            <Text content={t("incidentManager")} />
                            <PeoplePicker
                                selectionMode="single"
                                placeholder=" "
                                showMax={10}
                                defaultSelectedUserIds={!!incidentManager ? [incidentManager] : []}
                                selectionChanged={(e: any) => {
                                    const customEvent = e as ICustomEvent;
                                    onManagerChanged(customEvent?.detail[0]?.id);
                                }}
                            />
                            {!!managerValidationError && <ErrorMessage errorMessage={managerValidationError} />}
                        </Flex>
                        <Divider />
                        {!!otherRoles &&
                            otherRoles.map((userRole) => {
                                return (
                                    <Flex column>
                                        <Flex key={userRole.id} className={classes.row}>
                                            <div className={classes.pickerColumn}>
                                                <PeoplePicker
                                                    selectionMode="single"
                                                    placeholder=" "
                                                    showMax={10}
                                                    defaultSelectedUserIds={!!userRole.userId ? [userRole.userId] : []}
                                                    selectionChanged={(e: any) => {
                                                        const customEvent = e as ICustomEvent;
                                                        onUserIdChanged(userRole.id, customEvent?.detail[0]?.id);
                                                    }}
                                                />
                                            </div>
                                            <div className={classes.ddContainer}>
                                                <Dropdown
                                                    items={allRoles}
                                                    onChange={(e, data) => {
                                                        const val = data.value as UserRoleDropdownItemProps;
                                                        onUserRoleChanged(userRole.id, val?.role);
                                                    }}
                                                    value={allRoles.filter((x) => x.role.id === userRole.role?.id)}
                                                />
                                            </div>
                                            <div className={classes.removeBtnContainer}>
                                                <Button style={{ minWidth: "32px" }} text icon={<CloseIcon />} onClick={() => onUserRoleDelete(userRole.id)} />
                                            </div>
                                        </Flex>
                                        {!!userRole.validationMessage && <ErrorMessage errorMessage={userRole.validationMessage} />}
                                    </Flex>
                                );
                            })}

                        <Flex>
                            <Button text primary icon={<AddIcon />} type={"button"} content={"Add users"} onClick={onUserRoleAdd} />
                        </Flex>
                    </Flex>
                </Flex>
                <Divider size={0} />
                <Flex gap="gap.medium" padding="padding.medium" style={{ margin: 10 }}>
                    <Flex gap="gap.medium" padding="padding.medium" style={{ margin: 10 }}>
                        <Button content={t("updateTeamBtnLabel")} disabled={!isFormValid} primary type="submit" loading={isLoading} />
                        <Button content={t("cancelBtnLabel")} onClick={props.onCancel} />
                    </Flex>
                </Flex>
            </form>
        </Flex>
    );
};

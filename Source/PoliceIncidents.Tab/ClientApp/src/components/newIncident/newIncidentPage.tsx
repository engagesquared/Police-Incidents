import * as React from "react";
import { Flex, Text, Button, Breadcrumb, ChevronEndIcon, Input, Divider, TextArea } from "@fluentui/react-northstar";
import { useTranslation } from "react-i18next";
import { useStyles } from "./newIncidentPage.styles";
import { RegisterOptions, useForm } from "react-hook-form";
import { createIncident } from "../../apis/api-list";
import { ErrorMessage } from "../form/errorMessage";

import { GroupType, PeoplePicker, PersonType } from "@microsoft/mgt-react";
import { Routes } from "../../common";
import { GlobalContext } from "../../providers/GlobalContextProvider";
import { useHistory } from "react-router-dom";

export const NewIncidentPage = () => {
    const { t } = useTranslation();
    const ctx = React.useContext(GlobalContext);
    const classes = useStyles();
    const history = useHistory();
    const [isLoading, setIsLoading] = React.useState(false);
    const getDefaultValues = () => {
        const result = {
            title: "",
            description: "",
            location: "",
            manager: "",
            groups: [],
            members: []
        };
        return result;
    };

    const { handleSubmit, getValues, setValue, register, watch, errors } = useForm({
        defaultValues: getDefaultValues(),
    });

    const { title, manager, location, description, groups, members } = watch();

    React.useEffect(() => {
        const validationRules = {
            title: {
                required: t("requiredValidationMessage"),
                maxLength: { value: 50, message: t("titleMaxLengthValidationMessage").replace("{n}", "50") },
            },
            manager: {
                required: t("requiredValidationMessage"),
            },
            location: {
                required: t("requiredValidationMessage"),
            },
            members: {
                required: t("requiredValidationMessage"),
                validate: {
                    hasValue: ((val: any[]) => { return val.length || groups.length || t("requiredValidationMessage"); })
                }
            } as RegisterOptions,
            groups: {
                required: t("requiredValidationMessage"),
                validate: {
                    hasValue: ((val: any[]) => { return val.length || members.length || t("requiredValidationMessage"); })
                }
            } as RegisterOptions,
            description: undefined
        };

        register({ name: "title" }, validationRules.title);
        register({ name: "description" }, validationRules.description);
        register({ name: "location" }, validationRules.location);
        register({ name: "manager" }, validationRules.manager);
        register({ name: "members" }, validationRules.members);
        register({ name: "groups" }, validationRules.groups);
    }, [getValues, register, t, groups, members]);



    const onConfirm = handleSubmit(async (data) => {
        try {
            setIsLoading(true);
            const incidentId = await createIncident({
                title: title,
                description: description,
                managerId: manager,
                location: location,
                regionId: ctx.teamsContext.groupId || "",
                memberIds: members,
                groupIds: groups
            });
            history.push(Routes.incidentPage.replace(Routes.incidentIdPart, String(incidentId)));
        } catch (ex) {
            console.log(ex);
        } finally {
            setIsLoading(false);
        }
    });

    const onManagerChange = (e: any) => {
        const result = e.detail && e.detail.length ? e.detail[0].id : undefined;
        setValue("manager", result, { shouldValidate: true });
    };

    const onMembersChange = (e: any) => {
        const result = e.detail && e.detail.length ? e.detail.map((g: any) => g.id) : [];
        setValue("members", result, { shouldValidate: true });
    };

    const onGroupsChange = (e: any) => {
        const result = e.detail && e.detail.length ? e.detail.map((g: any) => g.id) : [];
        setValue("groups", result, { shouldValidate: true });
    }

    const onGoBackClick = () => {
        history.goBack();
    };

    // const isO365Group = (group: any) => {
    //     return group.groupTypes.some((type: string) => type === "Unified");
    // }

    // const isSecurityGroup = (group: any) => {
    //     return !group.mailEnabled && group.securityEnabled;
    // }

    return (
        <Flex className={classes.container} column gap="gap.large">
            <Flex>
                <Breadcrumb aria-label="breadcrumb" size="large">
                    <Breadcrumb.Item>
                        <Breadcrumb.Link href={Routes.home}>{t("homePageTitle")}</Breadcrumb.Link>
                    </Breadcrumb.Item>
                    <Breadcrumb.Divider>
                        <ChevronEndIcon size="small" />
                    </Breadcrumb.Divider>
                    <Breadcrumb.Item active>{t("newIncidentPageTitle")}</Breadcrumb.Item>
                </Breadcrumb>
            </Flex>
            <Divider />
            <form onSubmit={onConfirm}>
                <Flex column gap="gap.medium">
                    <Flex column>
                        <Input
                            fluid
                            inverted
                            label={t("titleFieldLabel")}
                            value={title}
                            onChange={(ev: any, p) => {
                                setValue("title", p ? p.value : "", { shouldValidate: true });
                            }}
                        />
                        {!!errors.title && <ErrorMessage errorMessage={errors.title.message} />}
                    </Flex>
                    <Flex column>
                        <Text content={t("descriptionFieldLabel")} />
                        <TextArea
                            fluid
                            inverted
                            value={description}
                            rows={5}
                            onChange={(ev: any, p) => {
                                setValue("description", p ? p.value : "", { shouldValidate: true });
                            }}
                        />
                        {!!errors.description && <ErrorMessage errorMessage={errors.description.message} />}
                    </Flex>
                    <Flex column>
                        <Input
                            fluid
                            inverted
                            label={t("locationFieldLabel")}
                            value={location}
                            onChange={(ev: any, p) => {
                                setValue("location", p ? p.value : "", { shouldValidate: true });
                            }}
                        />
                        {!!errors.location && <ErrorMessage errorMessage={errors.location.message} />}
                    </Flex>
                    <Flex column>
                        <Text content={t("managerFieldLabel")} />
                        <PeoplePicker placeholder=" " selectionMode="single" selectionChanged={onManagerChange} />
                        {!!errors.manager && <ErrorMessage errorMessage={errors.manager.message} />}
                    </Flex>
                    <Flex column>
                        <Text content={t("teamMembersFieldLabel")} />
                        <PeoplePicker placeholder=" " type={PersonType.person} showMax={25} selectionChanged={onMembersChange} />
                        {!!errors.members && <ErrorMessage errorMessage={(errors.members as any).message} />}
                    </Flex>
                    <Flex column>
                        <Text content={t("groupsFieldLabel")} />
                        <PeoplePicker placeholder=" " type={PersonType.group} groupType={GroupType.any} showMax={25} selectionChanged={onGroupsChange} />
                        {!!errors.groups && <ErrorMessage errorMessage={(errors.groups as any).message} />}
                    </Flex>
                    <Flex gap="gap.medium">
                        <Button content={t("goBackBtnLabel")} type="button" onClick={onGoBackClick} />
                        <Button primary content={t("newIncidentBtnLabel")} type="submit" loading={isLoading} />
                    </Flex>
                </Flex>
            </form>
            <Divider />
        </Flex>
    );
};

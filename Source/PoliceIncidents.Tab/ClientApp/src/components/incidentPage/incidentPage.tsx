import * as React from "react";
import { Flex, AddIcon, Loader, EditIcon, Text, Button, Breadcrumb, ShareLocationIcon, ChevronEndIcon, Dialog, MenuButton, ExpandIcon } from "@fluentui/react-northstar";
import { useTranslation } from "react-i18next";
import { useStyles } from "./incidentPage.styles";
import { useParams, useHistory } from "react-router-dom";
import { IIncidentModel } from "../../models/IIncidentModel";
import { IncidentDetailsCard } from "./incidentDetailsCard";
import { IncidentUpdates } from "./incidentUpdates";
import { getIncident, getScheduleMeetingLink, generatePdf } from "../../apis/api-list";
import { Person, MgtTemplateProps, PersonViewType, PersonCardInteraction } from "@microsoft/mgt-react";
import { IncidentUpdateType, IIncidentUpdateModel, IncidentStatus, IIncidentMemberModel, IUserRoleModel } from "../../models";
import { executeDeepLink } from "@microsoft/teams-js";
import { Routes } from "../../common";
import { UpdateCreationForm } from "./updateCreationForm";
import { UpdateLocationForm } from "./updateLocationForm";
import { CloseIncidentForm } from "./closeIncidentForm";
import { EditIncidentTeamForm } from "./editIncidentTeamFrom";
import { useGlobalState } from "../../hooks/useGlobalState";

export const IncidentPage = () => {
    const { t } = useTranslation();
    const { incidentId }: { incidentId?: string } = useParams();
    const history = useHistory();
    const classes = useStyles();
    const [incident, setIncident] = React.useState<IIncidentModel>();
    const [isLoadingAction, setIsLoadingAction] = React.useState(false);
    const [isNewUpdateFormActive, setIsNewUpdateFormActive] = React.useState(false);
    const [isNewCriticalUpdateFormActive, setIsNewCriticalUpdateFormActive] = React.useState(false);
    const [manualUpdates, setManualUpdates] = React.useState<IIncidentUpdateModel[]>([]);
    const [criticalUpdates, setCriticalUpdates] = React.useState<IIncidentUpdateModel[]>([]);
    const [isLocaltionEditActive, setIsLocaltionEditActive] = React.useState(false);
    const [closeIncidentFormOpen, setCloseIncidentFormOpen] = React.useState(false);
    const [editIncidentTeamFormOpen, setEditIncidentTeamFormOpen] = React.useState(false);
    const { state } = useGlobalState();

    React.useEffect(() => {
        (async () => {
            if (!!incidentId) {
                const incident = await getIncident(Number(incidentId));
                setIncident(incident);
                setManualUpdates(incident.incidentUpdates.filter((x) => x.updateType === IncidentUpdateType.Manual));
                setCriticalUpdates(incident.incidentUpdates.filter((x) => x.updateType === IncidentUpdateType.Critical));
            }
        })();
    }, [incidentId]);

    const onLocationClick = () => {
        window.open(`https://www.bing.com/maps?where1=${incident?.location}`, "_blank");
    };

    const onUpdateAdded = (update: IIncidentUpdateModel, isClosed?: boolean) => {
        if (update.updateType === IncidentUpdateType.Manual) {
            const updates = [...manualUpdates];
            updates.splice(0, 0, update);
            setManualUpdates(updates);
        }
        if (update.updateType === IncidentUpdateType.Critical) {
            const updates = [...criticalUpdates];
            updates.splice(0, 0, update);
            setCriticalUpdates(updates);
        }
        if (isClosed) {
            if (incident) {
                let tempIncident = incident;
                tempIncident.status = 3;
                setIncident(tempIncident);
            }
        }
    };

    const onUpdateTeamMember = (update: IIncidentMemberModel[], newManager?: string) => {
        if (incident) {
            setIncident({ ...incident, members: update, managerId: newManager });
        }
    };

    const onScheduleClick = async () => {
        setIsLoadingAction(true);
        try {
            const link = await getScheduleMeetingLink(incident!.id);
            executeDeepLink(link);
        } catch (error) {
        } finally {
            setIsLoadingAction(false);
        }
    };

    const onGoChatClick = async () => {
        try {
            executeDeepLink(incident?.chatThreadLink || "");
        } catch (error) {}
    };

    const onGeneratePdfClick = async () => {
        setIsLoadingAction(true);
        try {
            const link = await generatePdf(incident!.id);
            executeDeepLink(link);
        } catch (error) {
        } finally {
            setIsLoadingAction(false);
        }
        try {
        } catch (error) {}
    };

    const onGoPdfFolderClick = async () => {
        try {
            executeDeepLink(incident?.reportsFolderPath || "");
        } catch (error) {}
    };

    const onGoPlannerClick = async () => {
        try {
            executeDeepLink(incident?.plannerLink || "");
        } catch (error) {}
    };
    const CustomIncidentManager = (props: MgtTemplateProps) => {
        return <div>{t("incidentManager")}</div>;
    };
    type MgtTemplatePropsTemp = {
        roleNumber: number;
        roles: IUserRoleModel[];
    };
    type Itemp = MgtTemplateProps & MgtTemplatePropsTemp;

    const CustomIncidentMembers = (props: Itemp) => {
        return <div>{props.roles.find((x) => x.id === props.roleNumber)?.name}</div>;
    };
    const membersToShow: string[] = [];
    [{ userId: incident?.managerId || "", roleId: 0 }, ...(incident?.members || [])].forEach((user: IIncidentMemberModel) => {
        if (user.userId && membersToShow.findIndex((id: string) => id === user.userId) === -1) {
            membersToShow.push(user.userId);
        }
    });

    return (
        <div className={classes.container}>
            {!incident && <Loader />}
            {!!incident && (
                <Flex column gap="gap.medium">
                    <Flex>
                        <Breadcrumb aria-label="breadcrumb" size="large">
                            <Breadcrumb.Item>
                                <Breadcrumb.Link
                                    styles={{ cursor: "pointer" }}
                                    onClick={() => {
                                        history.push(Routes.backHome);
                                    }}
                                >
                                    {t("homePageTitle")}
                                </Breadcrumb.Link>
                            </Breadcrumb.Item>
                            <Breadcrumb.Divider>
                                <ChevronEndIcon size="small" />
                            </Breadcrumb.Divider>
                            <Breadcrumb.Item active>{incident.title}</Breadcrumb.Item>
                        </Breadcrumb>
                    </Flex>
                    <Flex space="between" vAlign="center" wrap gap="gap.medium">
                        <Text content={incident.description} />
                        <Flex gap="gap.medium" vAlign="center" className={classes.onlyInMobile}>
                            <Dialog
                                content={
                                    <CloseIncidentForm
                                        incidentId={incident.id}
                                        onCancel={() => setCloseIncidentFormOpen(false)}
                                        onAdded={onUpdateAdded}
                                    ></CloseIncidentForm>
                                }
                                style={{ padding: 0 }}
                                open={closeIncidentFormOpen}
                                onOpen={() => setCloseIncidentFormOpen(true)}
                                onCancel={() => setCloseIncidentFormOpen(false)}
                                className={classes.dialogMinWidth}
                            />
                            <MenuButton
                                trigger={<Button icon={<ExpandIcon />} loading={isLoadingAction} content={t("actionsBtnLabel")} />}
                                menu={[
                                    {
                                        disabled: incident.status === IncidentStatus.Closed,
                                        content: t("closeIncidentBtnLabel"),
                                        onClick: () => setCloseIncidentFormOpen(true),
                                    },
                                    {
                                        disabled: !incident.plannerLink,
                                        content: t("openPlannerBtnLabel"),
                                        onClick: onGoPlannerClick,
                                    },
                                    {
                                        disabled: !incident,
                                        content: t("scheduleMeetingBtnLabel"),
                                        onClick: onScheduleClick,
                                    },
                                    {
                                        disabled: !incident.chatThreadLink,
                                        content: t("goToChatThreadBtnLabel"),
                                        onClick: onGoChatClick,
                                    },
                                    {
                                        disabled: !incident,
                                        content: t("generatePdfBtnLabel"),
                                        onClick: onGeneratePdfClick,
                                    },
                                    {
                                        disabled: !incident.reportsFolderPath,
                                        content: t("goToGeneratePdfFolderBtnLabel"),
                                        onClick: onGoPdfFolderClick,
                                    },
                                ]}
                                on="click"
                            />
                        </Flex>
                    </Flex>
                    <Flex className={classes.contentGrid}>
                        <div className={classes.column}>
                            <Flex column gap="gap.medium">
                                <IncidentDetailsCard
                                    header={t("incidentTeamLabel")}
                                    addButton={
                                        incident.status !== 3 ? (
                                            <Dialog
                                                content={
                                                    incident ? (
                                                        <EditIncidentTeamForm
                                                            incidentId={incident.id}
                                                            incidentManager={incident.managerId}
                                                            incidentMembers={incident.members}
                                                            onCancel={() => setEditIncidentTeamFormOpen(false)}
                                                            onUpdated={onUpdateTeamMember}
                                                        ></EditIncidentTeamForm>
                                                    ) : (
                                                        <></>
                                                    )
                                                }
                                                style={{ padding: 0 }}
                                                open={editIncidentTeamFormOpen}
                                                onOpen={() => setEditIncidentTeamFormOpen(true)}
                                                trigger={<Button primary icon={<EditIcon size="small" />} size="small" text content={t("editBtnLabel")} />}
                                                onCancel={() => setEditIncidentTeamFormOpen(false)}
                                                className={classes.dialogMinWidth}
                                            />
                                        ) : (
                                            <></>
                                        )
                                    }
                                >
                                    {!!incident?.managerId && (
                                        <div className={classes.userItem}>
                                            <Person
                                                userId={incident.managerId}
                                                line2Property="jobTitle"
                                                showPresence={false}
                                                view={PersonViewType.twolines}
                                                personCardInteraction={PersonCardInteraction.hover}
                                            >
                                                <CustomIncidentManager template="line2" />
                                            </Person>
                                        </div>
                                    )}
                                    {!!incident?.members?.length &&
                                        incident.members.map((u) => {
                                            return (
                                                <div key={u.userId + u.roleId} className={classes.userItem}>
                                                    <Person
                                                        userId={u.userId}
                                                        line2Property="jobTitle"
                                                        showPresence={false}
                                                        view={PersonViewType.twolines}
                                                        personCardInteraction={PersonCardInteraction.hover}
                                                    >
                                                        <CustomIncidentMembers template="line2" roleNumber={u.roleId} roles={state.userRoles} />
                                                    </Person>
                                                </div>
                                            );
                                        })}
                                </IncidentDetailsCard>
                                <IncidentDetailsCard
                                    header={t("incidentLocationLabel")}
                                    addButton={
                                        incident.status !== 3 ? (
                                            <Flex gap="gap.small">
                                                <Button
                                                    primary
                                                    icon={<ShareLocationIcon size="small" />}
                                                    size="small"
                                                    text
                                                    content={t("seeMapBtnLabel")}
                                                    onClick={onLocationClick}
                                                />
                                                <Button
                                                    primary
                                                    icon={<EditIcon size="small" />}
                                                    onClick={() => setIsLocaltionEditActive(true)}
                                                    size="small"
                                                    text
                                                    content={t("editBtnLabel")}
                                                />
                                            </Flex>
                                        ) : (
                                            <></>
                                        )
                                    }
                                >
                                    {!isLocaltionEditActive ? (
                                        <div className={classes.locationItem}>
                                            <Text weight="semibold">{incident.location}</Text>
                                        </div>
                                    ) : (
                                        <UpdateLocationForm
                                            incidentId={incident.id}
                                            locationValue={incident.location}
                                            onCancel={() => {
                                                setIsLocaltionEditActive(false);
                                            }}
                                            onAdded={(updatedLocation) => {
                                                incident.location = updatedLocation;
                                            }}
                                        ></UpdateLocationForm>
                                    )}
                                </IncidentDetailsCard>
                            </Flex>
                        </div>
                        <div className={classes.column}>
                            <IncidentDetailsCard
                                header={t("latestUpdatesLabel")}
                                addButton={
                                    incident.status !== 3 ? (
                                        <Button
                                            primary
                                            icon={<AddIcon size="small" />}
                                            size="small"
                                            text
                                            content={t("addBtnLabel")}
                                            onClick={() => setIsNewUpdateFormActive(true)}
                                        />
                                    ) : (
                                        <></>
                                    )
                                }
                            >
                                {isNewUpdateFormActive && (
                                    <UpdateCreationForm
                                        incidentId={incident.id}
                                        onAdded={onUpdateAdded}
                                        onCancel={() => {
                                            setIsNewUpdateFormActive(false);
                                        }}
                                        type={IncidentUpdateType.Manual}
                                    />
                                )}
                                <IncidentUpdates itemsToShow={2} updates={manualUpdates} />
                            </IncidentDetailsCard>
                        </div>
                        <div className={classes.column}>
                            <IncidentDetailsCard
                                header={t("criticalDecisionsLabel")}
                                addButton={
                                    incident.status !== 3 ? (
                                        <Button
                                            primary
                                            icon={<AddIcon size="small" />}
                                            size="small"
                                            text
                                            content={t("addBtnLabel")}
                                            onClick={() => setIsNewCriticalUpdateFormActive(true)}
                                        />
                                    ) : (
                                        <></>
                                    )
                                }
                            >
                                {isNewCriticalUpdateFormActive && (
                                    <UpdateCreationForm
                                        incidentId={incident.id}
                                        onAdded={onUpdateAdded}
                                        onCancel={() => {
                                            setIsNewCriticalUpdateFormActive(false);
                                        }}
                                        type={IncidentUpdateType.Critical}
                                    />
                                )}
                                <IncidentUpdates itemsToShow={2} updates={criticalUpdates} />
                            </IncidentDetailsCard>
                        </div>
                    </Flex>
                </Flex>
            )}
        </div>
    );
};

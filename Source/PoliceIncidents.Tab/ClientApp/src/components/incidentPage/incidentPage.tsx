import * as React from "react";
import {
    Flex, AddIcon, Loader, EditIcon, Text, Button, Breadcrumb, ShareLocationIcon,
    ChevronEndIcon, Dialog
} from "@fluentui/react-northstar";
import { useTranslation } from "react-i18next";
import { useStyles } from "./incidentPage.styles";
import { useParams } from "react-router-dom";
import { IIncidentModel } from "../../models/IIncidentModel";
import { IncidentDetailsCard } from "./incidentDetailsCard";
import { IncidentUpdates } from "./incidentUpdates";
import { getIncident, getScheduleMeetingLink } from "../../apis/api-list";
import { Person, PersonViewType, PersonCardInteraction } from "@microsoft/mgt-react";
import { IncidentUpdateType, IIncidentUpdateModel } from "../../models";
import { executeDeepLink } from "@microsoft/teams-js";
import { Routes } from "../../common";
import { UpdateCreationForm } from "./updateCreationForm";
import { UpdateLocationForm } from "./updateLocationForm";
import { CloseIncidentForm } from './closeIncidentForm';
import { EditIncidentTeamForm } from "./editIncidentTeamFrom";
import { IIncidentTeamMemberInputModel } from "../../models/IIncidentTeamMemberInputModel";

export const IncidentPage = () => {
    const { t } = useTranslation();
    const { incidentId }: { incidentId?: string } = useParams();
    const classes = useStyles();
    const [incident, setIncident] = React.useState<IIncidentModel>();
    const [isLoadingMeetingLink, setIsLoadingMeetingLink] = React.useState(false);
    const [isNewUpdateFormActive, setIsNewUpdateFormActive] = React.useState(false);
    const [isNewCriticalUpdateFormActive, setIsNewCriticalUpdateFormActive] = React.useState(false);
    const [manualUpdates, setManualUpdates] = React.useState<IIncidentUpdateModel[]>([]);
    const [criticalUpdates, setCriticalUpdates] = React.useState<IIncidentUpdateModel[]>([]);
    const [isLocaltionEditActive, setIsLocaltionEditActive] = React.useState(false);
    const [closeIncidentFormOpen, setCloseIncidentFormOpen] = React.useState(false);
    const [editIncidentTeamFormOpen, setEditIncidentTeamFormOpen] = React.useState(false);

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

    const onUpdateTeamMember = (update: IIncidentTeamMemberInputModel, isClosed?: boolean) => {
        if (isClosed) {
            if (incident) {
                let tempIncident = incident;
                tempIncident.managerId = update.incidentManager;
                let temp: { item1: string, item2: number }[] = [];
                if (update.fieldOfficer) {
                    let tempitem = { item1: update.fieldOfficer, item2: 1 };
                    temp.push(tempitem);
                }
                if (update.externalAgency) {
                    let tempitem = { item1: update.externalAgency, item2: 2 };
                    temp.push(tempitem);
                }
                if (update.socLead) {
                    let tempitem = { item1: update.socLead, item2: 3 };
                    temp.push(tempitem);
                }
                if (update.familyLiason) {
                    let tempitem = { item1: update.familyLiason, item2: 4 };
                    temp.push(tempitem);
                }
                tempIncident.members = temp;
                setIncident(tempIncident);
            }
        }
    };

    const onScheduleClick = async () => {
        setIsLoadingMeetingLink(true);
        try {
            const link = await getScheduleMeetingLink(incident!.id);
            executeDeepLink(link);
        } catch (error) {
        } finally {
            setIsLoadingMeetingLink(false);
        }
    };

    const onGoChatClick = async () => {
        try {
            executeDeepLink(incident?.chatThreadLink || "");
        } catch (error) { }
    };

    const onGoPlannerClick = async () => {
        try {
            executeDeepLink(incident?.plannerLink || "");
        } catch (error) { }
    };

    const membersToShow: string[] = [];
    [{ item1: incident?.managerId || "" }, ...incident?.members || []].forEach(user => {
        if (user.item1 && (membersToShow.findIndex((id: string) => id === user.item1) == -1)) {
            membersToShow.push(user.item1);
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
                                <Breadcrumb.Link href={Routes.home}>{t("homePageTitle")}</Breadcrumb.Link>
                            </Breadcrumb.Item>
                            <Breadcrumb.Divider>
                                <ChevronEndIcon size="small" />
                            </Breadcrumb.Divider>
                            <Breadcrumb.Item active>{incident.title}</Breadcrumb.Item>
                        </Breadcrumb>
                    </Flex>
                    <Flex space="between" vAlign="center" wrap gap="gap.medium">
                        <Flex.Item grow={1}>
                            <Text content={incident.description} />
                        </Flex.Item>
                        <Flex gap="gap.medium" vAlign="center">
                            <Flex.Item >
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
                                    trigger={<Button disabled={incident.status === 3} primary content={t("closeIncidentBtnLabel")} />}
                                    onCancel={() => setCloseIncidentFormOpen(false)}
                                />
                            </Flex.Item>
                            <Flex.Item align="end">
                                <Button content={t("openPlannerBtnLabel")} disabled={!incident.plannerLink} onClick={onGoPlannerClick} />
                            </Flex.Item>
                            <Flex.Item align="end">
                                <Button content={t("scheduleMeetingBtnLabel")} disabled={!incident} loading={isLoadingMeetingLink} onClick={onScheduleClick} />
                            </Flex.Item>
                            <Flex.Item >
                                <Button primary content={t("goToChatThreadBtnLabel")} disabled={!incident.chatThreadLink} onClick={onGoChatClick} />
                            </Flex.Item>
                        </Flex>
                    </Flex>
                    <Flex className={classes.contentGrid}>
                        <div className={classes.column}>
                            <Flex column gap="gap.medium">
                                <IncidentDetailsCard
                                    header={t("incidentTeamLabel")}
                                    addButton={incident.status !== 3 ?
                                        <Dialog
                                            content={
                                                incident ? <EditIncidentTeamForm
                                                    incidentId={incident.id}
                                                    incidentManager={incident.managerId}
                                                    incidentMembers={incident.members}
                                                    onCancel={() => setEditIncidentTeamFormOpen(false)}
                                                    onAdded={onUpdateTeamMember}
                                                ></EditIncidentTeamForm> : <></>
                                            }
                                            style={{ padding: 0 }}
                                            open={editIncidentTeamFormOpen}
                                            onOpen={() => setEditIncidentTeamFormOpen(true)}
                                            trigger={<Button primary icon={<EditIcon size="small" />}
                                                size="small" text content={t("editBtnLabel")} />}
                                            onCancel={() => setEditIncidentTeamFormOpen(false)}
                                        />
                                        : <></>}
                                >
                                    {membersToShow.map((userId) => {
                                        return (
                                            <div className={classes.userItem}>
                                                <Person
                                                    userId={userId}
                                                    line2Property="jobTitle"
                                                    showPresence={false}
                                                    view={PersonViewType.twolines}
                                                    personCardInteraction={PersonCardInteraction.hover}
                                                />
                                            </div>
                                        );
                                    })}
                                </IncidentDetailsCard>
                                <IncidentDetailsCard
                                    header={t("incidentLocationLabel")}
                                    addButton={
                                        incident.status !== 3 ?
                                            <Flex gap="gap.small"><Button
                                                primary
                                                icon={<ShareLocationIcon size="small" />}
                                                size="small"
                                                text
                                                content={t("seeMapBtnLabel")}
                                                onClick={onLocationClick}
                                            />
                                                <Button primary icon={<EditIcon size="small" />}
                                                    onClick={() => setIsLocaltionEditActive(true)}
                                                    size="small" text content={t("editBtnLabel")} /></Flex> : <></>
                                    }
                                >
                                    {!isLocaltionEditActive ? <div className={classes.locationItem}>
                                        <Text weight="semibold">{incident.location}</Text>
                                    </div> : <UpdateLocationForm incidentId={incident.id}
                                        locationValue={incident.location}
                                        onCancel={() => {
                                            setIsLocaltionEditActive(false);
                                        }}
                                        onAdded={(updatedLocation) => { incident.location = updatedLocation; }}></UpdateLocationForm>}
                                </IncidentDetailsCard>
                            </Flex>
                        </div>
                        <div className={classes.column}>
                            <IncidentDetailsCard
                                header={t("latestUpdatesLabel")}
                                addButton={
                                    incident.status !== 3 ?
                                        <Button
                                            primary
                                            icon={<AddIcon size="small" />}
                                            size="small"
                                            text
                                            content={t("addBtnLabel")}
                                            onClick={() => setIsNewUpdateFormActive(true)}
                                        /> : <></>
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
                                    incident.status !== 3 ?
                                        <Button
                                            primary
                                            icon={<AddIcon size="small" />}
                                            size="small"
                                            text
                                            content={t("addBtnLabel")}
                                            onClick={() => setIsNewCriticalUpdateFormActive(true)}
                                        /> : <></>
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

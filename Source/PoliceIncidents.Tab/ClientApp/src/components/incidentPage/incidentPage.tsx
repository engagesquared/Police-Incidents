import * as React from "react";
import { Flex, AddIcon, Loader, EditIcon, Text, Button, Breadcrumb, ShareLocationIcon, ChevronEndIcon } from "@fluentui/react-northstar";
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

    const onUpdateAdded = (update: IIncidentUpdateModel) => {
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
        } catch (error) {}
    };

    const onGoPlannerClick = async () => {
        try {
            executeDeepLink(incident?.plannerLink || "");
        } catch (error) {}
    };

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
                    <Flex vAlign="center" wrap gap="gap.medium">
                        <Flex.Item grow={1}>
                            <Text content={incident.description} />
                        </Flex.Item>
                        <Flex.Item align="end">
                            <Button content={t("openPlannerBtnLabel")} disabled={!incident.plannerLink} onClick={onGoPlannerClick} />
                        </Flex.Item>
                        <Flex.Item align="end">
                            <Button content={t("scheduleMeetingBtnLabel")} disabled={!incident} loading={isLoadingMeetingLink} onClick={onScheduleClick} />
                        </Flex.Item>
                        <Flex.Item align="end">
                            <Button primary content={t("goToChatThreadBtnLabel")} disabled={!incident.chatThreadLink} onClick={onGoChatClick} />
                        </Flex.Item>
                    </Flex>
                    <Flex className={classes.contentGrid}>
                        <div className={classes.column}>
                            <Flex column gap="gap.medium">
                                <IncidentDetailsCard
                                    header={t("incidentTeamLabel")}
                                    addButton={<Button primary icon={<EditIcon size="small" />} size="small" text content={t("editBtnLabel")} />}
                                >
                                    {!!incident?.managerId && (
                                        <div className={classes.userItem}>
                                            <Person
                                                userId={incident.managerId}
                                                line2Property="jobTitle"
                                                showPresence={false}
                                                view={PersonViewType.twolines}
                                                personCardInteraction={PersonCardInteraction.hover}
                                            />
                                        </div>
                                    )}
                                    {!!incident?.members?.length &&
                                        incident.members.map((u) => {
                                            return (
                                                <div className={classes.userItem}>
                                                    <Person
                                                        userId={u}
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
                                        <Button
                                            primary
                                            icon={<ShareLocationIcon size="small" />}
                                            size="small"
                                            text
                                            content={t("seeMapBtnLabel")}
                                            onClick={onLocationClick}
                                        />
                                    }
                                >
                                    <div className={classes.locationItem}>
                                        <Text weight="semibold">{incident.location}</Text>
                                    </div>
                                </IncidentDetailsCard>
                            </Flex>
                        </div>
                        <div className={classes.column}>
                            <IncidentDetailsCard
                                header={t("latestUpdatesLabel")}
                                addButton={
                                    <Button
                                        primary
                                        icon={<AddIcon size="small" />}
                                        size="small"
                                        text
                                        content={t("addBtnLabel")}
                                        onClick={() => setIsNewUpdateFormActive(true)}
                                    />
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
                                    <Button
                                        primary
                                        icon={<AddIcon size="small" />}
                                        size="small"
                                        text
                                        content={t("addBtnLabel")}
                                        onClick={() => setIsNewCriticalUpdateFormActive(true)}
                                    />
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

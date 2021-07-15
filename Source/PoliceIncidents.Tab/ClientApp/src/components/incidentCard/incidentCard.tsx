import * as React from "react";
import { Button, Flex, Text, Segment, Divider, List } from "@fluentui/react-northstar";
import { useHistory } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { useStyles } from "./incidentCard.styles";
import { Person, PersonCardInteraction, PersonViewType } from "@microsoft/mgt-react";
import { IIncidentModel } from "../../models/IIncidentModel";
import { Routes } from "../../common";
import { executeDeepLink } from "@microsoft/teams-js";
import { IncidentUpdateType } from "../../models";
import { formatDateTime } from "../../utils";
import { useGlobalState } from "../../hooks/useGlobalState";

export const IncidentCard = (props: { incident: IIncidentModel }) => {
    const { t } = useTranslation();
    const history = useHistory();
    const classes = useStyles();
    const { incident } = props;
    const { state } = useGlobalState();

    const onGoChatClick = async () => {
        try {
            executeDeepLink(incident?.chatThreadLink || "");
        } catch (error) {}
    };

    return (
        <div className={classes.container}>
            <Segment color="brand" style={{ borderTopWidth: "5px" }}>
                <Flex gap="gap.smaller" className={classes.header}>
                    <Text content={incident.title} as="h3" />
                    <Text content={incident.description} />
                    <div>
                        <Person userId={incident.managerId} showPresence={false} view={PersonViewType.oneline} personCardInteraction={PersonCardInteraction.hover} />
                    </div>
                </Flex>
                <Divider size={1} />
                <List
                    items={incident.incidentUpdates.map((iu) => ({
                        content: iu.body,
                        header:
                            iu.updateType === IncidentUpdateType.Critical
                                ? `Critical Decision: ${iu.title}`
                                : iu.updateType === IncidentUpdateType.Manual
                                ? `Update: ${iu.title}`
                                : iu.title,
                        headerMedia: formatDateTime(state.teamsContext.locale, iu.createdAt),
                        key: iu.id,
                    }))}
                />
                <Flex gap="gap.smaller" vAlign="center">
                    <Button
                        primary
                        content={t("seeFullIncidentBtnLabel")}
                        onClick={() => {
                            history.push(Routes.incidentPage.replace(Routes.incidentIdPart, String(incident.id)));
                        }}
                    />
                    <Button primary content={t("goToChatThredBtnLabel")} onClick={onGoChatClick} />
                </Flex>
            </Segment>
        </div>
    );
};

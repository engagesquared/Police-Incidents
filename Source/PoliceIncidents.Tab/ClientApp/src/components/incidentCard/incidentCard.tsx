import * as React from "react";
import { Button, Flex, Text, Segment, Divider, List } from "@fluentui/react-northstar";
import { useHistory } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { useStyles } from "./incidentCard.styles";
import { Person, PersonCardInteraction, PersonViewType } from "@microsoft/mgt-react";
import { IIncidentModel } from "../../models/IIncidentModel";
import { Routes } from "../../common";

export const IncidentCard = (props: { incident: IIncidentModel }) => {
    const { t } = useTranslation();
    const history = useHistory();
    const classes = useStyles();
    const { incident } = props;
    return (
        <div className={classes.container}>
            <Segment color="brand" style={{ borderTopWidth: "5px" }}>
                <Flex gap="gap.smaller" vAlign="center">
                    <Text content={incident.title} as="h3" />
                    <Text content={incident.description} />
                    <Person userId={incident.managerId} showPresence={false} view={PersonViewType.oneline} personCardInteraction={PersonCardInteraction.hover} />
                </Flex>
                <Divider size={1} />
                <List
                    items={incident.incidentUpdates.map((iu) => ({
                        content: iu.body,
                        header: iu.title,
                        headerMedia: iu.createdAt,
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
                    <Button primary content={t("goToChatThredBtnLabel")} />
                </Flex>
            </Segment>
        </div>
    );
};

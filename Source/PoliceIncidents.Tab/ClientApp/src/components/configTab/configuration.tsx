import React from "react";
import { Flex, Text } from "@fluentui/react-northstar";
import * as microsoftTeams from "@microsoft/teams-js";
import { Routes } from "../../common";
import { useTranslation } from "react-i18next";
import { useStyles } from "./configuration.styles";

export const Configuration = () => {
    const { t } = useTranslation();
    const classes = useStyles();
    React.useEffect(() => {
        microsoftTeams.settings.registerOnSaveHandler((saveEvent) => {
            microsoftTeams.settings.setSettings({
                entityId: "Home",
                contentUrl: window.location.origin + Routes.home,
                suggestedDisplayName: "Incidents",
            });
            saveEvent.notifySuccess();
        });

        microsoftTeams.settings.setValidityState(true);
    }, []);

    return (
        <Flex className={classes.container} vAlign="center" hAlign="center">
            <Text content={t("configPageMessage")} size="large" />
        </Flex>
    );
};

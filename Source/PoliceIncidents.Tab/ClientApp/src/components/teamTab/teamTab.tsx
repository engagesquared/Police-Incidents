import * as React from "react";
import { Flex, Menu, Button } from "@fluentui/react-northstar";
import { getActiveTeamIncidents, getClosedTeamIncidents } from "../../apis/api-list";
import { useHistory } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { useStyles } from "./teamTab.styles";
import { Routes } from "../../common";
import { useGlobalState } from "../../hooks/useGlobalState";
import { IncidentsList } from "../incidentsList/IncidentsList";

enum Tabs {
    Active = 0,
    Closed = 1,
}

export const TeamTab = () => {
    const { t } = useTranslation();
    const history = useHistory();
    const classes = useStyles();
    const { state } = useGlobalState();
    const [activeTab, setActiveTab] = React.useState<Tabs>(Tabs.Active);
    const teamId = state.teamsContext.groupId || "";

    const menuItems = React.useMemo(
        () => [
            {
                key: "all",
                content: t("allIncidentsHeader"),
            },
            {
                key: "closed",
                content: t("allClosedIncidentsHeader"),
            },
        ],
        [t]
    );

    const onMenuChange = (event: React.SyntheticEvent<HTMLElement>, data?: any) => {
        setActiveTab(Number(data.activeIndex));
    };

    const onNewIncidentClick = () => {
        history.push(Routes.newIncidentPage);
    };
    return (
        <Flex column gap="gap.medium">
            <Flex>
                <Flex.Item grow={1}>
                    <Menu className={classes.menu} defaultActiveIndex={Tabs.Active} items={menuItems} underlined primary onActiveIndexChange={onMenuChange} />
                </Flex.Item>
                <Flex.Item align="end">
                    <Button primary content={t("newIncidentBtnLabel")} onClick={onNewIncidentClick} />
                </Flex.Item>
            </Flex>

            <Flex column>
                {activeTab === Tabs.Active && (
                    <>
                        <IncidentsList
                            getIncidents={(pageId: number) => {
                                return getActiveTeamIncidents(teamId, pageId);
                            }}
                        />
                    </>
                )}
                {activeTab === Tabs.Closed && (
                    <>
                        <IncidentsList
                            getIncidents={(pageId) => {
                                return getClosedTeamIncidents(teamId, pageId);
                            }}
                        />
                    </>
                )}
            </Flex>
        </Flex>
    );
};

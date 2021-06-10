import * as React from "react";
import { Flex, Menu, Button } from "@fluentui/react-northstar";
import { getActiveTeamIncidents, getClosedTeamIncidents } from "../../apis/api-list";
import { useHistory } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { useStyles } from "./teamTab.styles";
import { IncidentCard } from "../incidentCard/incidentCard";
import { Routes } from "../../common";
import { IIncidentModel } from "../../models";
import { GlobalContext } from "../../providers/GlobalContextProvider";

export const TeamTab = () => {
    const { t } = useTranslation();
    const history = useHistory();
    const classes = useStyles();
    const ctx = React.useContext(GlobalContext);
    const [incidents, setIncidents] = React.useState<IIncidentModel[]>([]);
    const [closedIncidents, setClosedIncidents] = React.useState<IIncidentModel[]>([]);
    const [activeIndex, setActiveIndex] = React.useState<number>(0);

    React.useEffect(() => {
        (async () => {
            let incidents = await getActiveTeamIncidents(ctx.teamsContext.groupId || "");
            let closedIncidents = await getClosedTeamIncidents(ctx.teamsContext.groupId || "");
            setIncidents(incidents);
            setClosedIncidents(closedIncidents);
        })();
    }, [ctx]);
    const items = [
        {
            key: "all",
            content: t("allIncidentsHeader") + " (" + incidents.length + ")",
        },
        {
            key: "closed",
            content: t("allClosedIncidentsHeader"),
        },
    ];

    const onMenuChange = (event: React.SyntheticEvent<HTMLElement>, data?: any) => {
        setActiveIndex(data.activeIndex);
    };

    const onNewIncidentClick = () => {
        history.push(Routes.newIncidentPage);
    };
    return (
        <Flex column gap="gap.medium">
            <Flex>
                <Flex.Item grow={1}>
                    <Menu className={classes.menu} defaultActiveIndex={0} items={items} underlined primary onActiveIndexChange={onMenuChange} />
                </Flex.Item>
                <Flex.Item align="end">
                    <Button primary content={t("newIncidentBtnLabel")} onClick={onNewIncidentClick} />
                </Flex.Item>
            </Flex>

            <Flex column>
                {activeIndex === 0 && incidents.map((incident) => (
                    <IncidentCard incident={incident} key={incident.id} />
                ))}
                {activeIndex === 1 && closedIncidents.map((incident) => (
                    <IncidentCard incident={incident} key={incident.id} />
                ))}
            </Flex>
        </Flex>
    );
};

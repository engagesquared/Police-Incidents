import * as React from "react";
import { Flex, Menu, Button } from "@fluentui/react-northstar";
import { getUserIncidents } from "../../apis/api-list";
import { useHistory } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { useStyles } from "./teamTab.styles";
import { IncidentCard } from "../incidentCard/incidentCard";
import { Routes } from "../../common";

export const TeamTab = () => {
    const { t } = useTranslation();
    const history = useHistory();
    const classes = useStyles();
    const [incidents, setIncidents] = React.useState<any[]>([]);
    React.useEffect(() => {
        (async () => {
            var incidents = (await getUserIncidents()).data;
            setIncidents(incidents);
        })();
    }, []);
    const items = [
        {
            key: "all",
            content: t("allIncidentsHeader"),
        },
        {
            key: "closed",
            content: t("allClosedIncidentsHeader"),
        },
    ];

    const onMenuChange = (event: React.SyntheticEvent<HTMLElement>, data?: any) => {
        console.log(data);
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
                {incidents.map((incident) => (
                    <IncidentCard incident={incident} key={incident.id} />
                ))}
            </Flex>
        </Flex>
    );
};

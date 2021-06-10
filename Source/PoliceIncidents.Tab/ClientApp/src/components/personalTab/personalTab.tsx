import * as React from "react";
import { Flex, Menu } from "@fluentui/react-northstar";
import { getAllUserIncidents, getManagedUserIncidents } from "../../apis/api-list";
import { useTranslation } from "react-i18next";
import { useStyles } from "./personalTab.styles";
import { IncidentCard } from "../incidentCard/incidentCard";

export const PersonalTab = () => {
    const { t } = useTranslation();
    const classes = useStyles();
    const [incidents, setIncidents] = React.useState<any[]>([]);
    const [managedIncidents, setManagedIncidents] = React.useState<any[]>([]);
    const [activeIndex, setActiveIndex] = React.useState<number>(0);
    React.useEffect(() => {
        (async () => {
            let incidents = await getAllUserIncidents();
            let managedIncidents = await getManagedUserIncidents();
            setIncidents(incidents);
            setManagedIncidents(managedIncidents);
        })();
    }, []);
    const items = [
        {
            key: "all",
            content: t("myIncidentsHeader"),
        },
        {
            key: "managed",
            content: t("myManagedIncidentsHeader"),
        },
    ];
    const onMenuChange = (event: React.SyntheticEvent<HTMLElement>, data?: any) => {
        setActiveIndex(data.activeIndex);
    };
    return (
        <div className={classes.container}>
            <Flex>
                <Menu defaultActiveIndex={0} items={items} underlined primary onActiveIndexChange={onMenuChange} />
            </Flex>

            <Flex column>
                {activeIndex === 0 && incidents.map((incident) => (
                    <IncidentCard incident={incident} key={incident.id} />
                ))}
                {activeIndex === 1 && managedIncidents.map((incident) => (
                    <IncidentCard incident={incident} key={incident.id} />
                ))}
            </Flex>
        </div>
    );
};

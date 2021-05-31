import * as React from "react";
import { Flex, Menu } from "@fluentui/react-northstar";
import { getAllUserIncidents } from "../../apis/api-list";
import { useTranslation } from "react-i18next";
import { useStyles } from "./personalTab.styles";
import { IncidentCard } from "../incidentCard/incidentCard";

export const PersonalTab = () => {
    const { t } = useTranslation();
    const classes = useStyles();
    const [incidents, setIncidents] = React.useState<any[]>([]);
    React.useEffect(() => {
        (async () => {
            var incidents = await getAllUserIncidents();
            setIncidents(incidents);
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
        console.log(data);
    };
    return (
        <div className={classes.container}>
            <Flex>
                <Menu defaultActiveIndex={0} items={items} underlined primary onActiveIndexChange={onMenuChange} />
            </Flex>

            <Flex column>
                {incidents.map((incident) => (
                    <IncidentCard incident={incident} key={incident.id} />
                ))}
            </Flex>
        </div>
    );
};

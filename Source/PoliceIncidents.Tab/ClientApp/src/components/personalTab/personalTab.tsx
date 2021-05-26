import * as React from "react";
import { Flex, Header } from "@fluentui/react-northstar";
import { getUserIncidents } from "../../apis/api-list";
import { useTranslation } from "react-i18next";
import { useStyles } from "./personalTab.styles";
import { IncidentCard } from "../incidentCard/incidentCard";

export const PersonalTab = () => {
    const { t } = useTranslation();
    const classes = useStyles();
    const [incidents, setIncidents] = React.useState<any[]>([]);
    React.useEffect(() => {
        (async () => {
            var incidents = (await getUserIncidents()).data;
            setIncidents(incidents);
        })();
    }, []);
    return (
        <div className={classes.container}>
            <Flex>
                <Header content={t("allIncidentHeader")} as="h3" />
            </Flex>

            <Flex column>
                {incidents.map((incident) => (
                    <IncidentCard incident={incident} key={incident.id} />
                ))}
            </Flex>
        </div>
    );
};

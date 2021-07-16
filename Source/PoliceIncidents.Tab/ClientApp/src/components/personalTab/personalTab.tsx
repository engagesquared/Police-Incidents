import * as React from "react";
import { Flex, Menu, Button, Dialog } from "@fluentui/react-northstar";
import { getAllUserIncidents, getManagedUserIncidents } from "../../apis/api-list";
import { useTranslation } from "react-i18next";
import { useStyles } from "./personalTab.styles";
import { IIncidentModel } from "../../models";
import { ReAssignMyIncident } from "./reAssignMyIncident";
import { useGlobalState } from "../../hooks/useGlobalState";
import { IncidentsList } from "../incidentsList/IncidentsList";

enum Tabs {
    All = 0,
    Managed = 1,
}

export const PersonalTab = () => {
    const { t } = useTranslation();
    const classes = useStyles();
    const [activeIndex, setActiveIndex] = React.useState(Tabs.All);
    const [managedIncidents, setManagedIncidents] = React.useState<IIncidentModel[]>([]);
    const [managedIncidentsKey, setManagedIncidentsKey] = React.useState(new Date().valueOf());
    const [reassignIncidentOpen, setReassignIncidentOpen] = React.useState<boolean>(false);
    const { state } = useGlobalState();

    const menuItems = React.useMemo(
        () => [
            {
                key: "all",
                content: t("myIncidentsHeader"),
            },
            {
                key: "managed",
                content: t("myManagedIncidentsHeader"),
            },
        ],
        [t]
    );

    const onMenuChange = (event: React.SyntheticEvent<HTMLElement>, data?: any) => {
        setActiveIndex(Number(data.activeIndex));
    };

    const onSuccess = (updatedIncidents: { incidentId: number; incidentManagerId: number }[]) => {
        let tempManagedIncident: IIncidentModel[] = managedIncidents;
        updatedIncidents.map((incident) => {
            tempManagedIncident.filter((t) => t.id === incident.incidentId)[0].managerId = incident.incidentManagerId.toString();
            return incident;
        });
        tempManagedIncident = tempManagedIncident.filter((t) => t.managerId === state.teamsContext.userObjectId);
        setManagedIncidentsKey(new Date().valueOf());
    };

    return (
        <div className={classes.container}>
            <Flex column gap="gap.medium">
                <Flex space="between">
                    <Flex>
                        <Menu className={classes.menu} defaultActiveIndex={Tabs.All} items={menuItems} underlined primary onActiveIndexChange={onMenuChange} />
                    </Flex>
                    {activeIndex === Tabs.Managed && managedIncidents.length > 0 && (
                        <Flex>
                            <Dialog
                                style={{ maxHeight: "500px", overflow: "auto", minWidth: "300px" }}
                                content={managedIncidents ? <ReAssignMyIncident onSuccess={onSuccess} myIncidents={managedIncidents} /> : t("reassignIncidentNoDataMsg")}
                                open={reassignIncidentOpen}
                                header={t("reassignIncidentBtnLabel")}
                                trigger={<Button content={t("reassignIncidentBtnLabel")} onClick={() => setReassignIncidentOpen(true)} />}
                                onCancel={() => setReassignIncidentOpen(false)}
                            />
                        </Flex>
                    )}
                </Flex>
                <Flex column>
                    {activeIndex === Tabs.All && (
                        <>
                            <IncidentsList
                                getIncidents={(pageId) => {
                                    return getAllUserIncidents(pageId);
                                }}
                            />
                        </>
                    )}
                    {activeIndex === Tabs.Managed && (
                        <>
                            <IncidentsList
                                key={managedIncidentsKey}
                                getIncidents={(pageId) => {
                                    return getManagedUserIncidents(pageId);
                                }}
                                loadCallback={(incidents) => {
                                    setManagedIncidents(incidents);
                                }}
                            />
                        </>
                    )}
                </Flex>
            </Flex>
        </div>
    );
};

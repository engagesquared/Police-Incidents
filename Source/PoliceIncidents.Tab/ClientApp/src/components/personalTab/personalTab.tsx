import * as React from "react";
import { Flex, Menu, Button, Dialog } from "@fluentui/react-northstar";
import { getAllUserIncidents, getManagedUserIncidents } from "../../apis/api-list";
import { useTranslation } from "react-i18next";
import { useStyles } from "./personalTab.styles";
import { IncidentCard } from "../incidentCard/incidentCard";
import { IIncidentModel } from "../../models";
import { ReAssignMyIncident } from "./reAssignMyIncident";
import { GlobalContext } from "../../providers/GlobalContextProvider";

export const PersonalTab = () => {
    const { t } = useTranslation();
    const classes = useStyles();
    const [incidents, setIncidents] = React.useState<IIncidentModel[]>([]);
    const [managedIncidents, setManagedIncidents] = React.useState<IIncidentModel[]>([]);
    const [activeIndex, setActiveIndex] = React.useState<number>(0);
    const [pageIndex, setPageIndex] = React.useState<number>(2);
    const [showIncidentLoadMore, setShowIncidentLoadMore] = React.useState<Boolean>(false);
    const [showManagedIncidentsLoadMore, setShowManagedIncidentsLoadMore] = React.useState<Boolean>(false);
    const [reassignIncidentOpen, setReassignIncidentOpen] = React.useState<boolean>(false);
    const context = React.useContext(GlobalContext);

    React.useEffect(() => {
        (async () => {
            let tempincidents = await getAllUserIncidents(1);
            let tempmanagedIncidents = await getManagedUserIncidents(1);
            const incidents = await getAllUserIncidents(2);
            if (incidents.length === 0) {
                setShowIncidentLoadMore(false);
            } else {
                setShowIncidentLoadMore(true);
            }
            setIncidents(tempincidents.concat(incidents));
            const managedIncidents = await getManagedUserIncidents(2);
            if (managedIncidents.length === 0) {
                setShowManagedIncidentsLoadMore(false);
            } else {
                setShowManagedIncidentsLoadMore(true);
            }
            setManagedIncidents(tempmanagedIncidents.concat(managedIncidents));
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

    const onLoadMore = async () => {
        const newPageIndex = pageIndex + 1;
        if (activeIndex === 0) {
            const tempincidents = await getAllUserIncidents(newPageIndex);
            if (tempincidents.length === 0) {
                setShowIncidentLoadMore(false);
            } else {
                setShowIncidentLoadMore(true);
            }
            setIncidents(incidents.concat(tempincidents));
        } else if (activeIndex === 1) {
            const tempmanagedIncidents = await getManagedUserIncidents(newPageIndex);
            if (tempmanagedIncidents.length === 0) {
                setShowManagedIncidentsLoadMore(false);
            } else {
                setShowManagedIncidentsLoadMore(true);
            }
            setManagedIncidents(managedIncidents.concat(tempmanagedIncidents));
        }
        setPageIndex(newPageIndex);
    }

    const onMenuChange = (event: React.SyntheticEvent<HTMLElement>, data?: any) => {
        setActiveIndex(data.activeIndex);
    };

    const onSuccess = (updatedIncidents: { incidentId: number, incidentManagerId: number }[]) => {
        let tempManagedIncident: IIncidentModel[] = managedIncidents;
        updatedIncidents.map((incident) => {
            tempManagedIncident.filter(t => t.id === incident.incidentId)[0].managerId = incident.incidentManagerId.toString();
            return incident;
        });
        tempManagedIncident = tempManagedIncident.filter(t => t.managerId === context.teamsContext.userObjectId);
        setManagedIncidents(tempManagedIncident);
    };

    return (
        <div className={classes.container}>
            <Flex column gap="gap.medium">
                <Flex space="between">
                    <Flex>
                        <Menu className={classes.menu} defaultActiveIndex={0} items={items} underlined primary onActiveIndexChange={onMenuChange} />
                    </Flex>
                    {
                        activeIndex === 1 && managedIncidents.length > 0 &&
                        <Flex>
                            <Dialog style={{ maxHeight: "500px", overflow: "auto", minWidth: "300px" }} content={managedIncidents ? <ReAssignMyIncident onSuccess={onSuccess} myIncidents={managedIncidents} /> : t('reassignIncidentNoDataMsg')} open={reassignIncidentOpen}
                                header={t("reassignIncidentBtnLabel")}
                                trigger={<Button content={t("reassignIncidentBtnLabel")} onClick={() => setReassignIncidentOpen(true)} />}
                                onCancel={() => setReassignIncidentOpen(false)} />
                        </Flex>
                    }
                </Flex>
                <Flex column>
                    {activeIndex === 0 && incidents.slice(0, (pageIndex - 1) * 10).map((incident) => (
                        <IncidentCard incident={incident} key={incident.id} />
                    ))}{activeIndex === 0 && showIncidentLoadMore && <Flex><Button primary content={t("loadMoreBtnLabel")} onClick={onLoadMore} /></Flex>}
                    {activeIndex === 1 && managedIncidents.slice(0, (pageIndex - 1) * 10).map((incident) => (
                        <IncidentCard incident={incident} key={incident.id} />
                    ))}{activeIndex === 1 && showManagedIncidentsLoadMore && <Flex><Button primary content={t("loadMoreBtnLabel")} onClick={onLoadMore} /></Flex>}
                </Flex>
            </Flex>
        </div>
    );
};

import * as React from "react";
import { Flex, Menu, Button } from "@fluentui/react-northstar";
import { getActiveTeamIncidents, getClosedTeamIncidents } from "../../apis/api-list";
import { useHistory } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { useStyles } from "./teamTab.styles";
import { IncidentCard } from "../incidentCard/incidentCard";
import { Routes } from "../../common";
import { IIncidentModel } from "../../models";
import { useGlobalState } from "../../hooks/useGlobalState";

export const TeamTab = () => {
    const { t } = useTranslation();
    const history = useHistory();
    const classes = useStyles();
    const { state } = useGlobalState();
    const [incidents, setIncidents] = React.useState<IIncidentModel[]>([]);
    const [closedIncidents, setClosedIncidents] = React.useState<IIncidentModel[]>([]);
    const [activeIndex, setActiveIndex] = React.useState<number>(0);
    const [pageIndex, setPageIndex] = React.useState<number>(2);
    const [showIncidentLoadMore, setShowIncidentLoadMore] = React.useState<Boolean>(false);
    const [showClosedIncidentLoadMore, setShowClosedIncidentLoadMore] = React.useState<Boolean>(false);

    React.useEffect(() => {
        (async () => {
            const tempincidents = await getActiveTeamIncidents(state.teamsContext.groupId || "", 1);
            const tempclosedIncidents = await getClosedTeamIncidents(state.teamsContext.groupId || "", 1);
            const incidents = await getActiveTeamIncidents(state.teamsContext.groupId || "", 2);
            if (incidents.length === 0) {
                setShowIncidentLoadMore(false);
            } else {
                setShowIncidentLoadMore(true);
            }
            setIncidents(tempincidents.concat(incidents));
            const closedIncidents = await getClosedTeamIncidents(state.teamsContext.groupId || "", 2);
            if (closedIncidents.length === 0) {
                setShowClosedIncidentLoadMore(false);
            } else {
                setShowClosedIncidentLoadMore(true);
            }
            setClosedIncidents(tempclosedIncidents.concat(closedIncidents));
        })();
    }, [state]);
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

    const onLoadMore = async () => {
        const newPageIndex = pageIndex + 1;
        if (activeIndex === 0) {
            const tempincidents = await getActiveTeamIncidents(state.teamsContext.groupId || "", newPageIndex);
            if (tempincidents.length === 0) {
                setShowIncidentLoadMore(false);
            } else {
                setShowIncidentLoadMore(true);
            }
            setIncidents(incidents.concat(tempincidents));
        } else if (activeIndex === 1) {
            const tempclosedIncidents = await getClosedTeamIncidents(state.teamsContext.groupId || "", newPageIndex);
            if (tempclosedIncidents.length === 0) {
                setShowClosedIncidentLoadMore(false);
            } else {
                setShowClosedIncidentLoadMore(true);
            }
            setClosedIncidents(closedIncidents.concat(tempclosedIncidents));
        }
        setPageIndex(newPageIndex);
    };

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
                {activeIndex === 0 && incidents.slice(0, (pageIndex - 1) * 10).map((incident) => <IncidentCard incident={incident} key={incident.id} />)}
                {activeIndex === 0 && showIncidentLoadMore && (
                    <Flex>
                        <Button primary content={t("loadMoreBtnLabel")} onClick={onLoadMore} />
                    </Flex>
                )}
                {activeIndex === 1 && closedIncidents.slice(0, (pageIndex - 1) * 10).map((incident) => <IncidentCard incident={incident} key={incident.id} />)}{" "}
                {activeIndex === 1 && showClosedIncidentLoadMore && (
                    <Flex>
                        {" "}
                        <Button primary content={t("loadMoreBtnLabel")} onClick={onLoadMore} />
                    </Flex>
                )}
            </Flex>
        </Flex>
    );
};

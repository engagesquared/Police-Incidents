import * as React from "react";
import { Flex, Button, Loader } from "@fluentui/react-northstar";
import { useTranslation } from "react-i18next";
import { IncidentCard } from "../incidentCard/incidentCard";
import { globalConstants } from "../../common";
import { IIncidentModel } from "../../models";

export interface IIncidentsListProps {
    getIncidents(page: number): Promise<IIncidentModel[]>;
    loadCallback?: (incidents: IIncidentModel[]) => void;
}

export const IncidentsList = (props: IIncidentsListProps) => {
    const { t } = useTranslation();
    const [incidents, setIncidents] = React.useState<IIncidentModel[]>([]);
    const [showIncidentLoadMore, setShowIncidentLoadMore] = React.useState(false);
    const [isLoading, setIsLoading] = React.useState(false);

    React.useEffect(() => {
        onLoadMore();
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, []);

    React.useEffect(() => {
        setShowIncidentLoadMore(!!incidents.length && incidents.length % globalConstants.pageSize === 0);
    }, [incidents.length]);

    const onLoadMore = async () => {
        setIsLoading(true);
        const nextPageNumber = incidents.length / globalConstants.pageSize + 1;
        const newIncidents = await props.getIncidents(nextPageNumber);
        const updatedIncidents = incidents.concat(newIncidents);
        setIncidents(updatedIncidents);
        if (props.loadCallback) {
            props.loadCallback(updatedIncidents);
        }
        setIsLoading(false);
    };

    return (
        <Flex column gap="gap.medium">
            <Flex column>
                {incidents.map((incident) => (
                    <IncidentCard incident={incident} key={incident.id} />
                ))}
                {!showIncidentLoadMore && isLoading && <Loader />}
                {showIncidentLoadMore && (
                    <Flex>
                        <Button loading={isLoading} primary content={t("loadMoreBtnLabel")} onClick={onLoadMore} />
                    </Flex>
                )}
            </Flex>
        </Flex>
    );
};

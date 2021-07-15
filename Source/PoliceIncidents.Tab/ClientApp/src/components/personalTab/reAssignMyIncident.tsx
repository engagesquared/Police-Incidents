import * as React from "react";
import { Flex, Text, Button, CheckmarkCircleIcon } from "@fluentui/react-northstar";
import { PeoplePicker, Person, MgtTemplateProps } from "@microsoft/mgt-react";
import { useTranslation } from "react-i18next";
import { useStyles } from "./reAssignMyIncident.styles";
import { IIncidentModel, IReassignIncidentInputModel } from "../../models";
import { reAssignIncident } from "../../apis/api-list";

export interface IReAssignMyIncidentProps {
    myIncidents: IIncidentModel[];
    onSuccess: any;
}

export const ReAssignMyIncident = (props: React.PropsWithChildren<IReAssignMyIncidentProps>) => {
    const { t } = useTranslation();
    const classes = useStyles();
    const [isLoading, setIsLoading] = React.useState(false);
    const [isSuccessMessage, setIsSuccessMessage] = React.useState(false);
    const [updatedIncidentManagers, setupdatedIncidentManagers] = React.useState<IReassignIncidentInputModel[]>([]);

    const onUserChange = (e: any, incidentId: number) => {
        const result = e ? (e.detail && e.detail.length ? (e.detail[0] ? e.detail[0].id : undefined) : undefined) : undefined;
        if (result) {
            let incidentManagers = updatedIncidentManagers;
            if (incidentManagers.filter((t) => t.incidentId === incidentId).length > 0) {
                let indexNumber: number = incidentManagers.map((t) => t.incidentId).indexOf(incidentId);
                incidentManagers[indexNumber].incidentManagerId = result;
            } else {
                incidentManagers.push({ incidentId: incidentId, incidentManagerId: result });
            }
            setupdatedIncidentManagers(incidentManagers);
        }
    };

    const onConfirm = async () => {
        try {
            setIsLoading(true);
            await reAssignIncident(updatedIncidentManagers);
            props.onSuccess(updatedIncidentManagers);
            setIsSuccessMessage(true);
        } catch (ex) {
            console.log(ex);
        } finally {
            setIsLoading(false);
        }
    };

    const CustomPerson = (props: MgtTemplateProps) => {
        const { person } = props.dataContext;
        return <div style={{ fontSize: "1.75rem" }}>{`Thanks ${person.displayName.split(" ")[0]}, you have reassigned ${updatedIncidentManagers.length} incidents`}</div>;
    };

    return (
        <div className={classes.container}>
            {!isSuccessMessage && (
                <Flex gap="gap.medium" column padding="padding.medium">
                    {props.myIncidents.map((incident, index) => {
                        return (
                            <Flex column>
                                <Text content={incident.title} />
                                <PeoplePicker
                                    selectionMode="single"
                                    placeholder={`Search for users to reassign your incidents.`}
                                    selectionChanged={(e) => {
                                        onUserChange(e, incident.id);
                                    }}
                                    showMax={25}
                                />
                            </Flex>
                        );
                    })}
                    <Flex hAlign="end" vAlign="end" padding="padding.medium" gap="gap.medium">
                        <Button primary content={t("reassignBtnLabel")} loading={isLoading} onClick={onConfirm} />
                    </Flex>
                </Flex>
            )}
            {isSuccessMessage && (
                <Flex gap="gap.medium" column padding="padding.medium">
                    <Flex
                        hAlign="center"
                        vAlign="center"
                        padding="padding.medium"
                        gap="gap.medium"
                        styles={({ theme: { siteVariables } }) => ({
                            color: "#13A10E",
                        })}
                    >
                        <CheckmarkCircleIcon size="largest" />
                    </Flex>
                    <Flex>
                        <Person personQuery="me">
                            <CustomPerson />
                        </Person>
                    </Flex>
                </Flex>
            )}
        </div>
    );
};

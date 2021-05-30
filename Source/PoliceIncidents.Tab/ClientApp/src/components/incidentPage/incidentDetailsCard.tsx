import * as React from "react";
import { Flex, Segment, Text } from "@fluentui/react-northstar";
import { useStyles } from "./incidentDetailsCard.styles";

export interface IIncidentDetailsCardProps {
    header: string;
    addButton?: JSX.Element;
}

export const IncidentDetailsCard = (props: React.PropsWithChildren<IIncidentDetailsCardProps>) => {
    const classes = useStyles();

    return (
        <Segment style={{ padding: 0, borderTopWidth: 0 }}>
            <Flex vAlign="center" className={classes.header}>
                <Text content={props.header}></Text>
                {props.addButton}
            </Flex>
            {props.children}
        </Segment>
    );
};

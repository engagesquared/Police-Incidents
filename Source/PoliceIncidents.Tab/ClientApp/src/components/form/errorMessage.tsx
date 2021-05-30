import * as React from "react";
import { useStyles } from "./errorMessage.styles";
import { Flex, Text, ExclamationCircleIcon } from "@fluentui/react-northstar";

export interface IErrorMessageProps {
    errorMessage?: string;
}

export const ErrorMessage = (props: IErrorMessageProps) => {
    const classes = useStyles();
    if (!props.errorMessage) return <></>;
    return (
        <Flex className={classes.container} gap="gap.medium" vAlign="center">
            <ExclamationCircleIcon title={props.errorMessage} />
            <Text size="small" content={props.errorMessage} />
        </Flex>
    );
};

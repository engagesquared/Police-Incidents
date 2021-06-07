import * as React from "react";
import { Flex, TextArea, Button } from "@fluentui/react-northstar";
import { useTranslation } from "react-i18next";
import { useStyles } from "./updateLocationForm.styles";
import { setIncidentLocation } from "../../apis/api-list";


export interface IUpdateLocationFormProps {
    incidentId: number;
    locationValue: string;
    onCancel(): void;
    onAdded(updatedLocation: string): void;
}

export const UpdateLocationForm = (props: React.PropsWithChildren<IUpdateLocationFormProps>) => {
    const { t } = useTranslation();
    const [isLoading, setIsLoading] = React.useState(false);
    const [locationValue, setLocationValue] = React.useState(props.locationValue);
    const classes = useStyles();

    const onConfirm = (async () => {
        try {
            setIsLoading(true);
            await setIncidentLocation(props.incidentId, locationValue);
            props.onAdded(locationValue);
            props.onCancel();
        } catch (ex) {
            console.log(ex);
        } finally {
            setIsLoading(false);
        }
    });

    return (
        <Flex className={classes.container} column gap="gap.medium">
            <Flex column>
                <TextArea value={locationValue} fluid
                    inverted onChange={(ev: any, p) => {
                        setLocationValue(p ? p.value as string : "");
                    }} />
            </Flex>
            <Flex gap="gap.medium">
                <Button content={t("cancelBtnLabel")} type="button" onClick={props.onCancel} />
                <Button primary content={t("editBtnLabel")} onClick={onConfirm}
                    type="button" loading={isLoading} />
            </Flex>
        </Flex>
    );
};

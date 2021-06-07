import * as React from "react";
import {
    Flex, Text, Button, Image, Layout, Divider, TextArea
} from "@fluentui/react-northstar";
import { useTranslation } from "react-i18next";
import { useStyles } from "./closeIncidentForm.styles";
import { addIncidentUpdate, closeIncident } from "../../apis/api-list";
import { IncidentUpdateType, IIncidentUpdateModel } from "../../models";

export interface ICloseIncidentFormProps {
    incidentId: number;
    onCancel(): void;
    onAdded(update: IIncidentUpdateModel, isClosed?: boolean): void;
}

export const CloseIncidentForm = (props: React.PropsWithChildren<ICloseIncidentFormProps>) => {
    const { t } = useTranslation();
    const [isLoading, setIsLoading] = React.useState(false);
    const [closingBody, setClosingBody] = React.useState('');
    //const classes = useStyles();

    const onConfirm = (async () => {
        try {
            setIsLoading(true);
            const update = await addIncidentUpdate(props.incidentId, {
                title: 'Incident Closed',
                body: closingBody,
                updateType: IncidentUpdateType.Manual,
            });
            await closeIncident(props.incidentId);
            props.onAdded(update, true);
            props.onCancel();
        } catch (ex) {
            console.log(ex);
        } finally {
            setIsLoading(false);
        }
    });

    return (
        <Flex column>
            <Flex gap="gap.medium" padding="padding.medium" style={{ margin: 15 }}>
                <Layout
                    styles={{
                        maxWidth: '50px',
                    }}
                    renderMainArea={() => (
                        <Image
                            fluid
                            src="https://in-api.asm.skype.com/v1/objects/0-sin-d1-c7800defda8c4c458f4981456a447f9d/views/imgpsh_fullsize"
                        />
                    )}
                />
                <Flex column>
                    <Text content="Incident App" weight="bold" size="larger" />
                    <Text content="Closing Information" /></Flex>
            </Flex>
            <Divider size={3} color="brand" />
            <Flex column padding="padding.medium" style={{ margin: 10 }}>
                <Flex column padding="padding.medium" style={{ margin: 10 }}>
                    <Text content="Information" />
                    <TextArea variables={{ height: "100px" }} value={closingBody}
                        fluid placeholder="Closing Information..."
                        onChange={(ev: any, p) => {
                            setClosingBody(p ? p.value as string : "");
                        }} />
                </Flex>
            </Flex>
            <Divider size={0} />
            <Flex gap="gap.medium" padding="padding.medium" style={{ margin: 10 }}>
                <Flex gap="gap.medium" padding="padding.medium" style={{ margin: 10 }}>
                    <Button content={t("closeIncidentBtnLabel")} primary loading={isLoading} onClick={onConfirm} />
                    <Button content={t("cancelBtnLabel")} onClick={props.onCancel} />
                </Flex>
            </Flex>
        </Flex>
    );
};

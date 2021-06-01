import * as React from "react";
import { Flex, TextArea, Text, Input, Button } from "@fluentui/react-northstar";
import { useStyles } from "./updateCreationForm.styles";
import { useForm } from "react-hook-form";
import { useTranslation } from "react-i18next";
import { ErrorMessage } from "../form/errorMessage";
import { addIncidentUpdate } from "../../apis/api-list";
import { IncidentUpdateType, IIncidentUpdateModel } from "../../models";

export interface IIncidentDetailsCardProps {
    incidentId: number;
    type: IncidentUpdateType;
    onAdded(update: IIncidentUpdateModel): void;
    onCancel(): void;
}

export const UpdateCreationForm = (props: React.PropsWithChildren<IIncidentDetailsCardProps>) => {
    const classes = useStyles();
    const { t } = useTranslation();
    const [isLoading, setIsLoading] = React.useState(false);

    const { handleSubmit, getValues, setValue, register, watch, errors } = useForm({
        defaultValues: {
            title: "",
            body: "",
        },
    });

    React.useEffect(() => {
        const validationRules = {
            title: {
                required: t("requiredValidationMessage"),
            },
            body: undefined,
        };

        register({ name: "title" }, validationRules.title);
        register({ name: "body" }, validationRules.body);
    }, [getValues, register, t]);

    const { title, body } = watch();

    const onConfirm = handleSubmit(async (data) => {
        try {
            setIsLoading(true);
            const update = await addIncidentUpdate(props.incidentId, {
                title: title,
                body: body,
                updateType: props.type,
            });
            props.onAdded(update);
            props.onCancel();
        } catch (ex) {
            console.log(ex);
        } finally {
            setIsLoading(false);
        }
    });

    return (
        <form onSubmit={onConfirm}>
            <Flex className={classes.container} column gap="gap.medium">
                <Flex column>
                    <Input
                        fluid
                        inverted
                        label={t("titleFieldLabel")}
                        value={title}
                        onChange={(ev: any, p) => {
                            setValue("title", p ? p.value : "", { shouldValidate: true });
                        }}
                    />
                    {!!errors.title && <ErrorMessage errorMessage={errors.title.message} />}
                </Flex>
                <Flex column>
                    <Text content={t("descriptionFieldLabel")} />
                    <TextArea
                        fluid
                        inverted
                        value={body}
                        rows={3}
                        onChange={(ev: any, p) => {
                            setValue("body", p ? p.value : "", { shouldValidate: true });
                        }}
                    />
                    {!!errors.body && <ErrorMessage errorMessage={errors.body.message} />}
                </Flex>
                <Flex gap="gap.medium">
                    <Button content={t("cancelBtnLabel")} type="button" onClick={props.onCancel} />
                    <Button primary content={t("newUpdateBtnLabel")} type="submit" loading={isLoading} />
                </Flex>
            </Flex>
        </form>
    );
};

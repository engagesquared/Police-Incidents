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
                            src="data:image/png;base64,R0lGODlhwADAAPcAANPk8KzN47zW6KHG4Iq52PL4+5bA3LTQ5YCy0+Lu9r7Y6aTI4J7E3rTQ5unz+Gaiza7O5GakzZS+24a21nyw0nGp0ICy1W6oz5zD3WekzWilzrDO5NTl8Hiu03yw1HSs0anM47TS5m6ozpC82oO01IS11sTb66jK4pS/3OXw98Xc687i7szg7prC3Hat0XGq0IK01pjA3LrW6ez0+bjU52Kgy6DG33as0tjo86LG33Sq0cbc7Nzr9I+82oK01Hqv0tDi73qw09Lk797s9bLQ5YS2136y1MTc7JvC3p7F34672Xiu0Y672sDZ6Ym418DZ6n6x0oy62cje7aPI4JjC3Ia21JzD38rf7bjV6G+p0IK006bL4pK92qrK46DF36LI4bDQ5Yi42Ia32H6x1LfS52yoz3+y09Dj78vg7nSs0vT5/Hau0szi75K+28Ha6ou52M7h7qzO5IGz1JK+3Iq62LbT53qu0abI4tDk8JC82YCz1ePv9trp89bn8oe314C01bvW57fU57TS5bLS5q7N46bK4aTH4Y272s3g7bHQ5azM5KvM41+fy5zE3ZjB3ZfA23yv0WOhzGOizGGgy2Khy6bJ4GajzWWjzPD2+sPb6oa11GWizPH3+8Pa6mumzvD3+mqmzm2nz9Lj76TH32ynz8Pb62mlznOr0WCgy7LP5fD3+2Ghy6bJ4Wymz3Kq0HKq0e72+nOr0GCfy+31+WSjzGymzmynznuw1Gumz/H3+mWizcLb622nzmunzrTR5Yi32Het0mSizcPa62qmzbrV6KfJ4nKr0HOq0GShzJnC3cHZ6oKz1oa11nqv1MPc7MLb6tfo8oG01bXR5p7E3WmmzoS008fd7LrV56bI4XKr0XOq0ajK4fD2+4O11sTb7JrB3nyv1Het0Y+72sPb7Ofx92KgzHyw0WSjzYSz02GhzPP4+/P5+8La62unz8je7rXT5mmlzcPc62Oiy2ejzez1+s/j8NXn8bzW53Or0u/2+tHj78La6qPH37PQ5YW11GSizCH/C1hNUCBEYXRhWE1QPD94cGFja2V0IGJlZ2luPSLvu78iIGlkPSJXNU0wTXBDZWhpSHpyZVN6TlRjemtjOWQiPz4gPHg6eG1wbWV0YSB4bWxuczp4PSJhZG9iZTpuczptZXRhLyIgeDp4bXB0az0iQWRvYmUgWE1QIENvcmUgNS42LWMxNDUgNzkuMTYzNDk5LCAyMDE4LzA4LzEzLTE2OjQwOjIyICAgICAgICAiPiA8cmRmOlJERiB4bWxuczpyZGY9Imh0dHA6Ly93d3cudzMub3JnLzE5OTkvMDIvMjItcmRmLXN5bnRheC1ucyMiPiA8cmRmOkRlc2NyaXB0aW9uIHJkZjphYm91dD0iIiB4bWxuczp4bXA9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC8iIHhtbG5zOnhtcE1NPSJodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvbW0vIiB4bWxuczpzdFJlZj0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wL3NUeXBlL1Jlc291cmNlUmVmIyIgeG1wOkNyZWF0b3JUb29sPSJBZG9iZSBQaG90b3Nob3AgQ0MgMjAxOSAoTWFjaW50b3NoKSIgeG1wTU06SW5zdGFuY2VJRD0ieG1wLmlpZDpFMzIwNEU3OTRFN0IxMUU5OTc3OThBNzBENDM2QzUxMyIgeG1wTU06RG9jdW1lbnRJRD0ieG1wLmRpZDpFMzIwNEU3QTRFN0IxMUU5OTc3OThBNzBENDM2QzUxMyI+IDx4bXBNTTpEZXJpdmVkRnJvbSBzdFJlZjppbnN0YW5jZUlEPSJ4bXAuaWlkOkUzMjA0RTc3NEU3QjExRTk5Nzc5OEE3MEQ0MzZDNTEzIiBzdFJlZjpkb2N1bWVudElEPSJ4bXAuZGlkOkUzMjA0RTc4NEU3QjExRTk5Nzc5OEE3MEQ0MzZDNTEzIi8+IDwvcmRmOkRlc2NyaXB0aW9uPiA8L3JkZjpSREY+IDwveDp4bXBtZXRhPiA8P3hwYWNrZXQgZW5kPSJyIj8+Af/+/fz7+vn49/b19PPy8fDv7u3s6+rp6Ofm5eTj4uHg397d3Nva2djX1tXU09LR0M/OzczLysnIx8bFxMPCwcC/vr28u7q5uLe2tbSzsrGwr66trKuqqainpqWko6KhoJ+enZybmpmYl5aVlJOSkZCPjo2Mi4qJiIeGhYSDgoGAf359fHt6eXh3dnV0c3JxcG9ubWxramloZ2ZlZGNiYWBfXl1cW1pZWFdWVVRTUlFQT05NTEtKSUhHRkVEQ0JBQD8+PTw7Ojk4NzY1NDMyMTAvLi0sKyopKCcm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"
                        />
                    )}
                />
                <Flex column>
                    <Text content="Incident App" weight="bold" size="larger" />
                    <Text content={t("closeIncidentTitle")} /></Flex>
            </Flex>
            <Divider size={3} color="brand" />
            <Flex column padding="padding.medium" style={{ margin: 10 }}>
                <Flex column padding="padding.medium" style={{ margin: 10 }}>
                    <Text content={t("closeIncidentTextAreaLabel")} />
                    <TextArea variables={{ height: "100px" }} value={closingBody}
                        fluid placeholder=""
                        onChange={(ev: any, p) => {
                            setClosingBody(p ? p.value as string : "");
                        }} />
                    <Text size={"small"} content={t("closeIncidentTextAreaHelperText")} />
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

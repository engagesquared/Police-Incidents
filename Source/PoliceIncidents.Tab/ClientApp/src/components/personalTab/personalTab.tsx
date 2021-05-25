import * as React from "react";
import { Button, Flex, Text, Segment, Divider, List, CallIcon, Image, Header, reactionSlotClassNames } from "@fluentui/react-northstar";
import { getUserIncidents } from "../../apis/api-list";
import { useTranslation } from "react-i18next";
import { useStyles } from "./personalTab.styles";

export const PersonalTab = () => {
    const { t } = useTranslation();
    const classes = useStyles();
    const [incidents, setIncidents] = React.useState<any[]>([]);
    React.useEffect(() => {
        (async () => {
            var incidents = (await getUserIncidents()).data;
            setIncidents(incidents);
        })();
    }, []);
    return (
        <div className={classes.container}>
            <Flex>
                <Header content="All incidents" as="h3" />
            </Flex>

            <Flex column>
                {incidents.map((inc) => (
                    <Segment color="brand">
                        <Flex gap="gap.smaller" vAlign="center">
                            <Text content={inc.title} as="h3" />

                            <Text content={inc.description} />

                            <Text content="Program the sensor to the SAS alarm through the haptic SQL card! Program the sensor to the SAS alarm through the haptic SQL card!" />
                        </Flex>
                        <Divider size={1} />
                        <List
                            data-builder-id="1y033svpvdz"
                            items={inc.incidentUpdates.map((iu: any) => ({
                                content: iu.body,
                                header: iu.title,
                                headerMedia: iu.createdAt,
                                key: iu.id,
                            }))}
                        />
                        <Flex gap="gap.smaller" vAlign="center">
                            <Button primary content="See details" />
                            <Button primary content="Go to thread" />
                        </Flex>
                    </Segment>
                ))}
            </Flex>
        </div>
    );
};

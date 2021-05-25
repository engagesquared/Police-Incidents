import * as React from "react";
import { Button, Flex, Text, Segment, Divider, List, CallIcon, Image, Header } from "@fluentui/react-northstar";
import { getAccessToken } from "../../apis/api-list";
import { useTranslation } from "react-i18next";
import { useStyles } from "./personalTab.styles";

export const PersonalTab = () => {
    const { t } = useTranslation();
    const classes = useStyles();
    const [token, setToken] = React.useState("");
    const onBtnClick = async () => {
        const value = await getAccessToken();
        setToken(value.data);
    };
    return (
        <div className={classes.container}>
            <Flex>
                <Header content="All incidents" as="h3" />
            </Flex>
            <Flex column>
                <Segment color="brand">
                    <Flex gap="gap.smaller" vAlign="center">
                        <Text content="Missing person ledge point" as="h3" />

                        <Text content="Program the sensor to the SAS alarm through the haptic SQL card! Program the sensor to the SAS alarm through the haptic SQL card!" />

                        <Text content="Program the sensor to the SAS alarm through the haptic SQL card! Program the sensor to the SAS alarm through the haptic SQL card!" />
                    </Flex>
                    <Divider size={1} />
                    <List
                        data-builder-id="1y033svpvdz"
                        items={[
                            {
                                content: "Program the sensor to the SAS alarm through the haptic SQL card!",
                                header: "Robert Tolbert",
                                headerMedia: "7:26:56 AM",
                                key: "robert",
                            },
                            {
                                content: "Use the online FTP application to input the multi-byte application!",
                                header: "Celeste Burton",
                                headerMedia: "11:30:17 PM",
                                key: "celeste",
                            },
                            {
                                content: "The GB pixel is down, navigate the virtual interface!",
                                header: "Cecil Folk",
                                headerMedia: "5:22:40 PM",
                                key: "cecil",
                            },
                        ]}
                    />
                    <Flex gap="gap.smaller" vAlign="center">
                        <Button primary content="See details" />
                        <Button primary content="Go to thread" />
                    </Flex>
                </Segment>
            </Flex>
        </div>
    );
};

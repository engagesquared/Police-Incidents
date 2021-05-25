import * as React from "react";
import { Button, Flex, Text } from "@fluentui/react-northstar";
import { getAccessToken } from "../../apis/api-list";
import { useTranslation } from "react-i18next";
import { useStyles } from "./home.styles";

export const Home = () => {
    const { t } = useTranslation();
    const classes = useStyles();
    const [token, setToken] = React.useState("");
    const onBtnClick = async () => {
        const value = await getAccessToken();
        setToken(value.data);
    };
    return (
        <div className={classes.container}>
            <Flex column hAlign="center">
                <Text content={t("welcomeMessage")} />
                <Text content={t("getStarted")} />
                <Button content={t("getTokenBtnText")} onClick={onBtnClick} primary />
                <Text content={token} />
            </Flex>
        </div>
    );
};

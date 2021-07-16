import { ITheme } from "../../providers/ITheme";

import { createUseStyles } from "react-jss";
export const useStyles = createUseStyles((theme: ITheme) => {
    return {
        container: {
            "& mgt-people-picker": {
                "--input-border": "none",
                "--input-background-color": theme.siteVariables.colorScheme.default.background1,
            },
        },
        row: {
            alignItems: "center",
            justifyContent: "space-between",
        },
        pickerColumn: {
            width: "40%",
        },
        ddContainer: {
            width: "16rem",
            "& .ui-dropdown__container, & .ui-list": {
                width: "16rem",
            },
        },
        removeBtnContainer: {
            ".ui-button": {
                maxWidth: "2rem",
            },
        },
    };
});

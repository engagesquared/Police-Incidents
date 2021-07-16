import { ITheme } from "../../providers/ITheme";

import { createUseStyles } from "react-jss";
export const useStyles = createUseStyles((theme: ITheme) => {
    return {
        container: {
            wordBreak: "break-word",
        },
        descriptionRow: {},
        contentGrid: {
            margin: "0 -0.75rem",
        },
        buttonFlex: {
            display: "inline-flex",
        },
        onlyInMobile: {
            display: "flex",
        },
        dialogMinWidth: {
            minWidth: "300px",
        },
        "@media (max-width: 800px)": {
            contentGrid: {
                display: "block !important",
            },
            column: {
                padding: "0 0.75rem",
                width: "calc(100% / 1) !important",
            },
            buttonFlex: {
                display: "block !important",
                marginBottom: "1%",
            },
            onlyInMobile: {
                display: "block !important",
            },
        },
        column: {
            padding: "0 0.75rem",
            width: "calc(100% / 3)",
        },
        locationItem: {
            padding: "0.75em",
        },
        userItem: {
            padding: "0.5rem 0.75rem 0.5rem 0.75rem",
        },
    };
});

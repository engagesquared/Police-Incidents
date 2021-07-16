import { ITheme } from "../../providers/ITheme";

import { createUseStyles } from "react-jss";
export const useStyles = createUseStyles((theme: ITheme) => {
    return {
        container: {
            wordBreak: "break-word",
            "&:not(:last-child)": {
                marginBottom: "1rem",
            },
        },
        text: {
            whiteSpace: "nowrap",
            overflow: "hidden",
            textOverflow: "ellipsis",
            flexBasis: "60%",
        },
        textTitle: {
            fontSize: "1.25rem",
            fontWeight: 600,
            whiteSpace: "nowrap",
            overflow: "hidden",
            textOverflow: "ellipsis",
            flexBasis: "25%",
        },
        segment: {
            borderTopWidth: "5px",
            paddingTop: "0.5em",
        },
        divider: {
            paddingTop: "0.5em",
            paddingBottom: "0.5em",
        },
        header: {
            alignItems: "center",
        },
        person: {
            textAlign: "left",
            marginLeft: "auto",
            flexBasis: "15%",
        },
        "@media screen and (max-width: 800px)": {
            header: {
                alignItems: "normal",
                flexDirection: "column",
                "& > :not(:last-child)": {
                    marginBottom: "0.625rem",
                    marginRight: "0",
                },
            },
            person: {
                textAlign: "unset",
                marginLeft: "unset",
            },
        },
    };
});

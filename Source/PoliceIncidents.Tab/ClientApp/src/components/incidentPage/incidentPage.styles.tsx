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

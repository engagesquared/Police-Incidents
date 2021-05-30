import { ITheme } from "../../providers/ITheme";

import { createUseStyles } from "react-jss";
export const useStyles = createUseStyles((theme: ITheme) => {
    return {
        content: {
            display: "flex",
            flexDirection: "column",
            padding: "0",
            "& > :not(:last-child)": {
                borderBottom: "1px solid",
                borderBottomColor: theme.siteVariables.colorScheme.default.background3,
            },
            "& > *": {
                padding: "0.7em",
            },
        },
        item: {},
        itemBottom: {
            justifyContent: "space-between",
        },
        showMore: {
            display: "flex",
            justifyContent: "center",
        },
        showLessIcon: {
            transform: "rotate(180deg)",
        },
    };
});

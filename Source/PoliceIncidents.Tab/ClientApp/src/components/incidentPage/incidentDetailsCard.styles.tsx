import { ITheme } from "../../providers/ITheme";

import { createUseStyles } from "react-jss";
export const useStyles = createUseStyles((theme: ITheme) => {
    return {
        container: {
            wordBreak: "break-word",
        },
        header: {
            justifyContent: "space-between",
            padding: "0.75em 0.5em",
            backgroundColor: theme.siteVariables.colorScheme.default.background3,
            borderBottom: "1px solid",
            borderBottomColor: theme.siteVariables.colorScheme.default.background5,
        },
        content: {
            padding: "0.75em",
        },
    };
});

import { ITheme } from "../../providers/ITheme";

import { createUseStyles } from "react-jss";
export const useStyles = createUseStyles((theme: ITheme) => {
    console.log(theme);
    return {
        container: {
            color: theme.siteVariables.colorScheme.brand.foreground,
            paddingTop: "12%",
            paddingBottom: "12%",
            paddingLeft: "15%",
            paddingRight: "15%",
            wordBreak: "break-word",
        },
    };
});

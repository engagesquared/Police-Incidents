import { ITheme } from "../../providers/ITheme";

import { createUseStyles } from "react-jss";
export const useStyles = createUseStyles((theme: ITheme) => {
    return {
        container: {
            backgroundColor: theme.siteVariables.colorScheme.brand.background1,
            padding: "0.7em",
        },
    };
});

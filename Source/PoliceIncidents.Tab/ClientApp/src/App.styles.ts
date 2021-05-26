import { ITheme } from "./providers/ITheme";

import { createUseStyles } from "react-jss";
export const useStyles = createUseStyles((theme: ITheme) => {
    return {
        root: {
            height: "100vh",
        },
        scrollRegion: {
            background: theme.siteVariables.colorScheme.default.background1,
            overflowY: "auto",
            overflowX: "hidden",
            padding: "12px",
            flexGrow: 1,
        },
        mobileGap: {
            padding: "16px",
        },
    };
});

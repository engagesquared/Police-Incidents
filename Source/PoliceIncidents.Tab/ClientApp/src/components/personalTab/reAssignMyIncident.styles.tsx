import { ITheme } from "../../providers/ITheme";

import { createUseStyles } from "react-jss";
export const useStyles = createUseStyles((theme: ITheme) => {
    return {
        container: {
            wordBreak: "break-word",
            "& mgt-people-picker": {
                "--input-border": "none",
                "--input-background-color": "#f5f5f5",
            }
        },
        menu: {
            borderBottom: "none",
        }
    };
});

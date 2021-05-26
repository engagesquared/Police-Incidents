import { ITheme } from "../../providers/ITheme";

import { createUseStyles } from "react-jss";
export const useStyles = createUseStyles((theme: ITheme) => {
    console.log(theme);
    return {
        container: {
            wordBreak: "break-word",
            "&:not(:last-child)": {
                marginBottom: "1rem",
            },
        },
    };
});

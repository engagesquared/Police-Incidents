import * as React from "react";
import { Provider, teamsDarkTheme, teamsHighContrastTheme, teamsTheme, ThemePrepared } from "@fluentui/react-northstar";
import { ThemeProvider } from "react-jss";
import * as microsoftTeams from "@microsoft/teams-js";
import { GlobalContext } from "./GlobalContextProvider";

function FluentThemeProvider(props: any) {
    const ctx = React.useContext(GlobalContext);
    const [themeName, setThemeName] = React.useState(ctx.teamsContext.theme);

    microsoftTeams.registerOnThemeChangeHandler((theme) => {
        setThemeName(theme);
    });

    let theme: ThemePrepared<any> = teamsTheme;
    if (themeName === "dark") {
        theme = teamsDarkTheme;
    } else if (themeName === "contrast") {
        theme = teamsHighContrastTheme;
    }
    return (
        <ThemeProvider theme={theme}>
            <Provider theme={theme}>{props.children}</Provider>
        </ThemeProvider>
    );
}

export default FluentThemeProvider;

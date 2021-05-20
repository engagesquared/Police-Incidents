import * as React from "react";
import { Provider, teamsDarkTheme, teamsHighContrastTheme, teamsTheme, ThemePrepared } from "@fluentui/react-northstar";
import * as microsoftTeams from "@microsoft/teams-js";

export interface IThemeProviderState {
    themeName: string;
}

class ThemeProvider extends React.Component<{}, IThemeProviderState> {
    constructor(props: {}) {
        super(props);
        this.state = {
            themeName: "",
        };
    }

    public componentDidMount = () => {
        microsoftTeams.initialize();
        microsoftTeams.getContext((context) => {
            let theme = context.theme || "";
            this.setState({
                themeName: theme,
            });
            microsoftTeams.registerOnThemeChangeHandler((theme) => {
                this.setState(
                    {
                        themeName: theme,
                    },
                    () => {
                        this.forceUpdate();
                    }
                );
            });
        });
    };

    public render(): JSX.Element {
        let theme: ThemePrepared<any> = teamsTheme;
        if (this.state.themeName === "dark") {
            theme = teamsDarkTheme;
        } else if (this.state.themeName === "contrast") {
            theme = teamsHighContrastTheme;
        }
        return <Provider theme={theme}>{this.props.children}</Provider>;
    }
}

export default ThemeProvider;

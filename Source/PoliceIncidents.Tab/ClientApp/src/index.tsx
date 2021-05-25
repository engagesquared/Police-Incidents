import React from "react";
import ReactDOM from "react-dom";
import App from "./App";
import { BrowserRouter as Router } from "react-router-dom";
import FluentThemeProvider from "./providers/FluentThemeProvider";
import GlobalContextProvider from "./providers/GlobalContextProvider";

import * as microsoftTeams from "@microsoft/teams-js";
microsoftTeams.initialize();

ReactDOM.render(
    <Router>
        <GlobalContextProvider>
            <FluentThemeProvider>
                <App />
            </FluentThemeProvider>
        </GlobalContextProvider>
    </Router>,
    document.getElementById("root")
);

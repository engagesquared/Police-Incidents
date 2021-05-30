import * as React from "react";
import "./localization/localization";
import { BrowserRouter, Route, Switch } from "react-router-dom";
import ErrorPage from "./components/error-page/error-page";
import SignInPage from "./components/sign-in-page/sign-in-page";
import SignInSimpleStart from "./components/sign-in-page/sign-in-simple-start";
import SignInSimpleEnd from "./components/sign-in-page/sign-in-simple-end";
import { Home } from "./components/home/home";
import { IncidentPage } from "./components/incidentPage/incidentPage";
import { NewIncidentPage } from "./components/newIncident/newIncidentPage";
import { Providers, MgtPersonCard, TeamsHelper } from "@microsoft/mgt-react";
import { MgtTokenProvider } from "./providers/MgtTokenProvider";
import { Routes } from "./common";

import { Flex } from "@fluentui/react-northstar";
import { useStyles } from "./App.styles";
import { GlobalContext } from "./providers/GlobalContextProvider";
import * as microsoftTeams from "@microsoft/teams-js";

TeamsHelper.microsoftTeamsLib = microsoftTeams;

MgtPersonCard.config.useContactApis = false;
MgtPersonCard.config.sections.mailMessages = false;
MgtPersonCard.config.sections.files = false;

Providers.globalProvider = new MgtTokenProvider();

const App = () => {
    const classes = useStyles();
    const { isMobileDevice } = React.useContext(GlobalContext);

    return (
        <Flex className={classes.root} column>
            <div className={[classes.scrollRegion, isMobileDevice ? classes.mobileGap : ""].join(" ")}>
                <React.Suspense fallback={<></>}>
                    <BrowserRouter>
                        <Switch>
                            {/* Do not remove this pseudo-contatiner. Routes on mobile don't work without it */}
                            <>
                                <Route exact path={Routes.home} component={Home} />
                                <Route exact path={Routes.incidentPage} component={IncidentPage} />
                                <Route exact path={Routes.newIncidentPage} component={NewIncidentPage} />
                                <Route exact path="/errorpage" component={ErrorPage} />
                                <Route exact path="/errorpage/:id" component={ErrorPage} />
                                <Route exact path="/signin" component={SignInPage} />
                                <Route exact path="/signin-simple-start" component={SignInSimpleStart} />
                                <Route exact path="/signin-simple-end" component={SignInSimpleEnd} />
                            </>
                        </Switch>
                    </BrowserRouter>
                </React.Suspense>
            </div>
        </Flex>
    );
};

export default App;

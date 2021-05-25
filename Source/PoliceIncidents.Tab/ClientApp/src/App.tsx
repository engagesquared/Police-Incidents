import * as React from "react";
import "./localization/localization";
import { BrowserRouter, Route, Switch } from "react-router-dom";
import ErrorPage from "./components/error-page/error-page";
import SignInPage from "./components/sign-in-page/sign-in-page";
import SignInSimpleStart from "./components/sign-in-page/sign-in-simple-start";
import SignInSimpleEnd from "./components/sign-in-page/sign-in-simple-end";
import { Home } from "./components/home/home";
import { PersonalTab } from "./components/personalTab/personalTab";

import { Flex } from "@fluentui/react-northstar";
import { useStyles } from "./App.styles";
import { GlobalContext } from "./providers/GlobalContextProvider";
import { HostClientType } from "@microsoft/teams-js";

const App = () => {
    const classes = useStyles();
    const { teamsContext } = React.useContext(GlobalContext);
    const clientDeviceType = teamsContext.hostClientType;
    const isMobileDevice = clientDeviceType === HostClientType.android || clientDeviceType === HostClientType.ios;
    return (
        <Flex className={classes.root} column>
            <div className={[classes.scrollRegion, isMobileDevice ? classes.mobileGap : ""].join(" ")}>
                <React.Suspense fallback={<></>}>
                    <BrowserRouter>
                        <Switch>
                            <Route exact path="/home" component={PersonalTab} />
                            <Route exact path="/personal" component={Home} />
                            <Route exact path="/errorpage" component={ErrorPage} />
                            <Route exact path="/errorpage/:id" component={ErrorPage} />
                            <Route exact path="/signin" component={SignInPage} />
                            <Route exact path="/signin-simple-start" component={SignInSimpleStart} />
                            <Route exact path="/signin-simple-end" component={SignInSimpleEnd} />
                        </Switch>
                    </BrowserRouter>
                </React.Suspense>
            </div>
        </Flex>
    );
};

export default App;

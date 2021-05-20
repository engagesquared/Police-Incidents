import * as React from "react";
import "./App.scss";
import "./localization";
import { BrowserRouter, Route, Switch } from "react-router-dom";
import ErrorPage from "./components/error-page/error-page";
import SignInPage from "./components/sign-in-page/sign-in-page";
import SignInSimpleStart from "./components/sign-in-page/sign-in-simple-start";
import SignInSimpleEnd from "./components/sign-in-page/sign-in-simple-end";
import Home from "./components/home/home";
import Personal from "./components/personal/personal";
import ThemeProvider from "./providers/ThemeProvider";
import { Flex } from "@fluentui/react-northstar";

class App extends React.Component<{}, {}> {
    public render(): JSX.Element {
        return (
            <ThemeProvider>
                <Flex className="appContainer" column>
                    <React.Suspense fallback={<></>}>
                        <BrowserRouter>
                            <Switch>
                                <Route exact path="/home" component={Home} />
                                <Route exact path="/personal" component={Personal} />
                                <Route exact path="/errorpage" component={ErrorPage} />
                                <Route exact path="/errorpage/:id" component={ErrorPage} />
                                <Route exact path="/signin" component={SignInPage} />
                                <Route exact path="/signin-simple-start" component={SignInSimpleStart} />
                                <Route exact path="/signin-simple-end" component={SignInSimpleEnd} />
                            </Switch>
                        </BrowserRouter>
                    </React.Suspense>
                </Flex>
            </ThemeProvider>
        );
    }
}

export default App;

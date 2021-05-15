import * as React from 'react';
import { Suspense } from 'react';
import { BrowserRouter, Route, Switch } from 'react-router-dom';
import ErrorPage from "../components/error-page/error-page";
import SignInPage from "../components/sign-in-page/sign-in-page";
import SignInSimpleStart from "../components/sign-in-page/sign-in-simple-start";
import SignInSimpleEnd from "../components/sign-in-page/sign-in-simple-end";
import Home from '../components/home/home';
import "../i18n";

export const AppRoute: React.FunctionComponent<{}> = () => {

    return (
        <Suspense fallback={<></>}>
            <BrowserRouter>
                <Switch>
                    <Route exact path="/home" component={Home} />
                    <Route exact path="/errorpage" component={ErrorPage} />
                    <Route exact path="/errorpage/:id" component={ErrorPage} />
                    <Route exact path="/signin" component={SignInPage} />
                    <Route exact path="/signin-simple-start" component={SignInSimpleStart} />
                    <Route exact path="/signin-simple-end" component={SignInSimpleEnd} />
                </Switch>
            </BrowserRouter>
        </Suspense>
    );
}
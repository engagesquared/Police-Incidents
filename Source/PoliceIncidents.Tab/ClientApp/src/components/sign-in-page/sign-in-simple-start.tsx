// <copyright file="sign-in-simple-start.tsx" company="Microsoft">
// Copyright (c) Engage Squared. All rights reserved.
// </copyright>

import React, { useEffect } from "react";
import * as microsoftTeams from "@microsoft/teams-js";
import { getAuthenticationMetadata } from "../../apis/api-list";

const SignInSimpleStart: React.FunctionComponent = () => {
    useEffect(() => {
        microsoftTeams.initialize();
        microsoftTeams.getContext((context) => {
            const windowLocationOriginDomain = window.location.origin.replace("https://", "");
            const loginHint = context.loginHint ? context.loginHint : "";
            getAuthenticationMetadata(windowLocationOriginDomain, loginHint).then((result) => {
                window.location.assign(result.data);
            });
        });
    });

    return <></>;
};

export default SignInSimpleStart;

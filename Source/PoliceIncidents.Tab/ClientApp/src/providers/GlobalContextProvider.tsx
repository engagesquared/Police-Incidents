import * as React from "react";
import * as microsoftTeams from "@microsoft/teams-js";
import { HostClientType } from "@microsoft/teams-js";
import { IGlobalContext } from "./IGlobalContext";

export const GlobalContext = React.createContext<IGlobalContext>({} as any);

function GlobalContextProvider(props: any) {
    const [ctx, setCtx] = React.useState<microsoftTeams.Context | undefined>();
    React.useEffect(() => {
        microsoftTeams.getContext((context) => {
            setCtx(context);
        });
    }, []);

    if (ctx !== undefined) {
        const globalCtx: IGlobalContext = {
            teamsContext: ctx,
            isPersonalTab: !ctx.groupId,
            isMobileDevice: ctx.hostClientType === HostClientType.android || ctx.hostClientType === HostClientType.ios,
        };

        return <GlobalContext.Provider value={globalCtx}>{props.children}</GlobalContext.Provider>;
    }
    return <></>;
}

export default GlobalContextProvider;

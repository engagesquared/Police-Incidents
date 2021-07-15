import * as React from "react";
import * as microsoftTeams from "@microsoft/teams-js";
import { HostClientType } from "@microsoft/teams-js";
import { IGlobalContext } from "./IGlobalContext";
import { IUserRoleModel } from "../models";

const defaultCtx: IGlobalContext = {
    userRoles: [],
    isMobileDevice: false,
    isPersonalTab: false,
    subEntityID: "",
    teamsContext: undefined as any,
};

export interface IGlobalStateContext {
    state: IGlobalContext;
    dispatch: React.Dispatch<React.ReducerAction<React.Reducer<IGlobalContext, IAction>>>;
}

export const GlobalContext = React.createContext<IGlobalStateContext>({ state: defaultCtx, dispatch: () => {} });

function GlobalContextProvider(props: any) {
    const [state, dispatch] = React.useReducer(globalStateReducer, defaultCtx);
    React.useEffect(() => {
        microsoftTeams.getContext((context) => {
            dispatch({ type: "updateCtx", payload: context });
        });
    }, []);

    if (!!state.teamsContext) {
        return <GlobalContext.Provider value={{ state, dispatch }}>{props.children}</GlobalContext.Provider>;
    }
    return <></>;
}

export default GlobalContextProvider;

export declare type ActionsSet = "updateCtx" | "updateRoles";
export interface IAction<T = any> {
    type: ActionsSet;
    payload: T;
}

export const globalStateReducer = (state: IGlobalContext, action: IAction): IGlobalContext => {
    switch (action.type) {
        case "updateCtx":
            const ctx = action.payload as microsoftTeams.Context;
            return {
                ...state,
                teamsContext: ctx,
                isPersonalTab: !ctx.groupId,
                isMobileDevice: ctx.hostClientType === HostClientType.android || ctx.hostClientType === HostClientType.ios,
                subEntityID: ctx.subEntityId ? ctx.subEntityId : "",
            };
        case "updateRoles":
            return {
                ...state,
                userRoles: (action.payload as IUserRoleModel[]) || [],
            };
        default:
            return state;
    }
};

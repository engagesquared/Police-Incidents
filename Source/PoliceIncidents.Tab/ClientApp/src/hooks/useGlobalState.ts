import * as React from "react";
import { GlobalContext, IGlobalStateContext, ActionsSet } from "../providers/GlobalContextProvider";

export const useGlobalState = () => {
    const { state, dispatch } = React.useContext(GlobalContext) as IGlobalStateContext;
    const dispatchFunc = React.useMemo(() => {
        return (type: ActionsSet, payload?: any) => {
            dispatch({ type, payload });
        };
    }, [dispatch]);
    return {
        state,
        dispatch: dispatchFunc,
    };
};

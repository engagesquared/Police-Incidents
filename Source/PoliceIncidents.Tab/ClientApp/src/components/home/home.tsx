import * as React from "react";
import { PersonalTab } from "../personalTab/personalTab";
import { TeamTab } from "../teamTab/teamTab";
import { useGlobalState } from "../../hooks/useGlobalState";

export const Home = () => {
    const { state } = useGlobalState();

    return <>{state.isPersonalTab ? <PersonalTab /> : <TeamTab />}</>;
};

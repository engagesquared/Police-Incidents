import * as React from "react";
import { GlobalContext } from "../../providers/GlobalContextProvider";
import { PersonalTab } from "../personalTab/personalTab";
import { TeamTab } from "../teamTab/teamTab";

export const Home = () => {
    const { isPersonalTab } = React.useContext(GlobalContext);
    return <>{isPersonalTab ? <PersonalTab /> : <TeamTab />}</>;
};

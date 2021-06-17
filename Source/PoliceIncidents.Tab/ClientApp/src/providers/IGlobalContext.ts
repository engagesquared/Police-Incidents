import * as microsoftTeams from "@microsoft/teams-js";

export interface IGlobalContext {
    teamsContext: microsoftTeams.Context;
    isPersonalTab: boolean;
    isMobileDevice: boolean;
    subEntityID: string;
}

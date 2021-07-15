import * as microsoftTeams from "@microsoft/teams-js";
import { IUserRoleModel } from "../models";

export interface IGlobalContext {
    teamsContext: microsoftTeams.Context;
    isPersonalTab: boolean;
    isMobileDevice: boolean;
    subEntityID: string;
    userRoles: IUserRoleModel[];
}

import { IncidentStatus } from "./IncidentStatus";
import { IIncidentUpdateModel } from "./IIncidentUpdateModel";
import { IIncidentMemberModel } from "./IIncidentMemberModel";

export interface IIncidentModel {
    id: number;
    title: string;
    description: string;
    externalLink?: string;
    chatThreadLink?: string;
    reportsFolderPath?: string;
    plannerLink?: string;
    managerId?: string;
    members: IIncidentMemberModel[];
    status: IncidentStatus;
    location: string;
    incidentUpdates: IIncidentUpdateModel[];
    created: string;
}

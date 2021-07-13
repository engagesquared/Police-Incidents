import { IncidentStatus } from "./IncidentStatus";
import { IIncidentUpdateModel } from "./IIncidentUpdateModel";

export interface IIncidentModel {
    id: number;
    title: string;
    description: string;
    externalLink?: string;
    chatThreadLink?: string;
    reportsFolderPath?: string;
    plannerLink?: string;
    managerId?: string;
    members: { item1: string; item2: number }[];
    status: IncidentStatus;
    location: string;
    incidentUpdates: IIncidentUpdateModel[];
    created: string;
}

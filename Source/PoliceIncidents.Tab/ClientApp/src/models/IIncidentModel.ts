import { IncidentStatus } from "./IncidentStatus";
import { IIncidentUpdateModel } from "./IIncidentUpdateModel";

export interface IIncidentModel {
    id: number;
    title: string;
    description: string;
    externalLink?: string;
    managerId?: string;
    members: string[];
    status: IncidentStatus;
    location: string;
    incidentUpdates: IIncidentUpdateModel[];
    created: string;
}

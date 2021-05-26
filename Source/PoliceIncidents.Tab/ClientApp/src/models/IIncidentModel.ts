import { IncidentStatus } from "./IncidentStatus";
import { IIncidentUpdateModel } from "./IIncidentUpdateModel";

export interface IIncidentModel {
    id: number;
    title: string;
    description: string;
    webEOCLink?: string;
    incidentManagerId?: string;
    status: IncidentStatus;
    location: string;
    incidentUpdates: IIncidentUpdateModel[];
    incidentRaised: Date;
}

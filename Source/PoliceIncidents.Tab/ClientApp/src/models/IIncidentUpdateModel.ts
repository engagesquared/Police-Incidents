import { IncidentUpdateType } from "./IncidentUpdateType";

export interface IIncidentUpdateModel {
    id: number;
    title: string;
    body: string;
    updateType: IncidentUpdateType;
    createdAt: Date;
    createdById: string;
}

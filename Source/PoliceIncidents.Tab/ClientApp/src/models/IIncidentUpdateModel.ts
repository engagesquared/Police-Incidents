import { IncedentUpdateType } from "./IncidentUpdateType";

export interface IIncidentUpdateModel {
    id: number;
    title: string;
    body: string;
    updateType: IncedentUpdateType;
    createdAt: Date;
    createdById: string;
}

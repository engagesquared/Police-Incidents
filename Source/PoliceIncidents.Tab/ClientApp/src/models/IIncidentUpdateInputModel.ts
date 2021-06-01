import { IncidentUpdateType } from "./IncidentUpdateType";

export interface IIncidentUpdateInputModel {
    title: string;
    body: string;
    updateType: IncidentUpdateType;
}

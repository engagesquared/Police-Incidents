import { IncidentUpdateType } from "./IncidentUpdateType";

export class IncidentUpdateIModel {
    public parentIncidentId: number | undefined;
    public title: string | undefined;
    public body: string | undefined;
    public meetingId: string | undefined;
    public createdByUserId: string | undefined;
    public updateType: IncidentUpdateType | undefined;
}

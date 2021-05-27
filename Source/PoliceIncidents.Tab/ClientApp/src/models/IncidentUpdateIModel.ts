export class IncidentUpdateIModel {
    public parentIncidentId: number | undefined;
    public title: string | undefined;
    public body: string | undefined;
    public meetingId: string | undefined;
    public createdByUserId: string | undefined;
    public updateType: IncidentUpdateType | undefined;
}

export enum IncidentUpdateType {
    /// <summary>
    /// Manual
    /// </summary>
    Manual = 1,

    /// <summary>
    /// WebEOC
    /// </summary>
    WebEOC = 2,

    /// <summary>
    /// Critical
    /// </summary>
    Critical = 3,
}
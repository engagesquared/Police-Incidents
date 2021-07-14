import axios from "./axios-jwt-decorator";
import { AxiosResponse } from "axios";
import { IIncidentInputModel, IIncidentModel, IIncidentUpdateModel, IIncidentUpdateInputModel, IIncidentTeamMemberInputModel, IAccessToken } from "../models";

let baseAxiosUrl = "/api";

export const getAccessToken = async (): Promise<IAccessToken> => {
    let url = baseAxiosUrl + "/user/getToken";
    const response = await axios.get(url);
    return response.data as IAccessToken;
};

export const getAuthenticationMetadata = async (windowLocationOriginDomain: string, loginHint: string): Promise<AxiosResponse<string>> => {
    let url = `${baseAxiosUrl}/authenticationMetadata/GetAuthenticationUrlWithConfiguration?windowLocationOriginDomain=${windowLocationOriginDomain}&loginhint=${loginHint}`;
    return await axios.get(url, undefined, false);
};

export const getClientId = async (): Promise<AxiosResponse<string>> => {
    let url = baseAxiosUrl + "/authenticationMetadata/getClientId";
    return await axios.get(url);
};

export const getAllUserIncidents = async (pageNumber: number): Promise<IIncidentModel[]> => {
    let url = baseAxiosUrl + `/incidents/user/all/${pageNumber}`;
    const response = await axios.get(url);
    return response.data as IIncidentModel[];
};

export const getManagedUserIncidents = async (pageNumber: number): Promise<IIncidentModel[]> => {
    let url = baseAxiosUrl + `/incidents/user/managed/${pageNumber}`;
    const response = await axios.get(url);
    return response.data as IIncidentModel[];
};

export const getActiveTeamIncidents = async (teamId: string, pageNumber: number): Promise<IIncidentModel[]> => {
    let url = baseAxiosUrl + `/incidents/team/${teamId}/active/${pageNumber}`;
    const response = await axios.get(url);
    return response.data as IIncidentModel[];
};

export const getClosedTeamIncidents = async (teamId: string, pageNumber: number): Promise<IIncidentModel[]> => {
    let url = baseAxiosUrl + `/incidents/team/${teamId}/closed/${pageNumber}`;
    const response = await axios.get(url);
    return response.data as IIncidentModel[];
};

export const getIncident = async (id: number): Promise<IIncidentModel> => {
    let url = baseAxiosUrl + `/incidents/${id}`;
    const response = await axios.get(url);
    return response.data as IIncidentModel;
};

export const setIncidentManager = async (id: number, managerId: string): Promise<void> => {
    let url = baseAxiosUrl + `/Incidents/${id}/SetManager`;
    await axios.post(url, { managerId });
    return;
};

export const setIncidentLocation = async (id: number, location: string): Promise<void> => {
    let url = baseAxiosUrl + `/Incidents/${id}/location`;
    await axios.post(url, { location });
    return;
};

export const closeIncident = async (id: number): Promise<Boolean> => {
    let url = baseAxiosUrl + `/incidents/${id}/close`;
    const response = await axios.get(url);
    return response.data as Boolean;
};

export const getIncidentUpdates = async (id: number): Promise<AxiosResponse<any>> => {
    let url = baseAxiosUrl + `/incidents/${id}/updates`;
    return await axios.get(url);
};

export const addIncidentUpdate = async (incidentId: number, incidentUpdate: IIncidentUpdateInputModel): Promise<IIncidentUpdateModel> => {
    let url = baseAxiosUrl + `/incidents/${incidentId}/updates`;
    const response = await axios.post(url, incidentUpdate);
    return response.data as IIncidentUpdateModel;
};

export const createIncident = async (incident: IIncidentInputModel): Promise<number> => {
    let url = baseAxiosUrl + `/incidents`;
    const response = await axios.post(url, incident);
    return response.data as number;
};

export const getScheduleMeetingLink = async (incidentId: number): Promise<string> => {
    let url = baseAxiosUrl + `/incidents/${incidentId}/newMeetingLink`;
    const response = await axios.get(url);
    return response.data as string;
};

export const updateTeamMember = async (incidentId: number, teamMember: IIncidentTeamMemberInputModel): Promise<Boolean> => {
    let url = baseAxiosUrl + `/Incidents/${incidentId}/updatemember`;
    const response = await axios.post(url, teamMember);
    return response.data as Boolean;
};

export const reAssignIncident = async (updatedIncidentManagers: { incidentId: number; incidentManagerId: number }[]): Promise<Boolean> => {
    let url = baseAxiosUrl + `/Incidents/reassignincident`;
    const response = await axios.post(url, updatedIncidentManagers);
    return response.data as Boolean;
};

export const generatePdf = async (incidentId: number): Promise<string> => {
    let url = baseAxiosUrl + `/incidents/${incidentId}/generatePdf`;
    const response = await axios.post(url);
    return response.data as string;
};

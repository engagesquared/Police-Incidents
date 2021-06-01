import axios from "./axios-jwt-decorator";
import { AxiosResponse } from "axios";
import { IIncidentInputModel, IIncidentModel, IIncidentUpdateModel, IIncidentUpdateInputModel } from "../models";

let baseAxiosUrl = "/api";

export const getAccessToken = async (): Promise<string> => {
    let url = baseAxiosUrl + "/user/getToken";
    const response = await axios.get(url);
    return response.data as string;
};

export const getAuthenticationMetadata = async (windowLocationOriginDomain: string, loginHint: string): Promise<AxiosResponse<string>> => {
    let url = `${baseAxiosUrl}/authenticationMetadata/GetAuthenticationUrlWithConfiguration?windowLocationOriginDomain=${windowLocationOriginDomain}&loginhint=${loginHint}`;
    return await axios.get(url, undefined, false);
};

export const getClientId = async (): Promise<AxiosResponse<string>> => {
    let url = baseAxiosUrl + "/authenticationMetadata/getClientId";
    return await axios.get(url);
};

export const getAllUserIncidents = async (): Promise<IIncidentModel[]> => {
    let url = baseAxiosUrl + "/incidents/user/all";
    const response = await axios.get(url);
    return response.data as IIncidentModel[];
};

export const getManagedUserIncidents = async (): Promise<IIncidentModel[]> => {
    let url = baseAxiosUrl + "/incidents/user/managed";
    const response = await axios.get(url);
    return response.data as IIncidentModel[];
};

export const getActiveTeamIncidents = async (teamId: string): Promise<IIncidentModel[]> => {
    let url = baseAxiosUrl + `/incidents/team/${teamId}/active`;
    const response = await axios.get(url);
    return response.data as IIncidentModel[];
};

export const getClosedTeamIncidents = async (teamId: string): Promise<IIncidentModel[]> => {
    let url = baseAxiosUrl + `/incidents/team/${teamId}/closed`;
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

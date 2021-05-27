import axios from "./axios-jwt-decorator";
import { AxiosResponse } from "axios";
import { IncidentUpdateIModel } from "../models/IncidentUpdateIModel";

let baseAxiosUrl = "/api";

export const getAccessToken = async (): Promise<AxiosResponse<string>> => {
    let url = baseAxiosUrl + "/user/getToken";
    console.log(url);
    return await axios.get(url);
};

export const getAuthenticationMetadata = async (windowLocationOriginDomain: string, loginHint: string): Promise<AxiosResponse<string>> => {
    let url = `${baseAxiosUrl}/authenticationMetadata/GetAuthenticationUrlWithConfiguration?windowLocationOriginDomain=${windowLocationOriginDomain}&loginhint=${loginHint}`;
    return await axios.get(url, undefined, false);
};

export const getClientId = async (): Promise<AxiosResponse<string>> => {
    let url = baseAxiosUrl + "/authenticationMetadata/getClientId";
    return await axios.get(url);
};

export const getUserIncidents = async (): Promise<AxiosResponse<any>> => {
    let url = baseAxiosUrl + "/Incidents/UserIncidents";
    return await axios.get(url);
};

export const getIncident = async (id: number): Promise<AxiosResponse<any>> => {
    let url = baseAxiosUrl + `/Incidents/${id}`;
    return await axios.get(url);
};

export const setIncidentManager = async (id: number, managerId: string): Promise<AxiosResponse<void>> => {
    let url = baseAxiosUrl + `/Incidents/${id}/SetManager`;
    return await axios.post(url, { managerId });
};

export const getIncidentUpdates = async (id: number): Promise<AxiosResponse<any>> => {
    let url = baseAxiosUrl + `/Incidents/${id}/Updates`;
    return await axios.get(url);
};

export const addIncidentUpdates = async (id: number, incidentUpdate: IncidentUpdateIModel): Promise<AxiosResponse<void>> => {
    let url = baseAxiosUrl + `/Incidents/${id}/AddUpdate`;
    console.log(url);
    return await axios.post(url, incidentUpdate);
};
import axios from "./axios-jwt-decorator";
import { AxiosResponse } from "axios";

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

export const getIncident = async (id: number): Promise<any> => {
    //let url = baseAxiosUrl + `/Incidents/${id}`;
    //var data = (await axios.get(url)).data;
    var data = (await getUserIncidents()).data;
    return data.filter((x: any) => x.id === id)[0];
};

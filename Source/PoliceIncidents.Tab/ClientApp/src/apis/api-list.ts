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
    console.log(url);
    return await axios.get(url);
};

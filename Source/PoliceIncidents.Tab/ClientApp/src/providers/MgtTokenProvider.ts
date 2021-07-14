import { SimpleProvider, ProviderState } from "@microsoft/mgt-react";
import { getAccessToken as getAccessTokenAsync } from "../apis/api-list";
import { IAccessToken } from "../models";

let tokenCached: string = "";
let expired: Date | undefined;
let getTokenFunc: Promise<IAccessToken> | undefined;

export class MgtTokenProvider extends SimpleProvider {
    constructor() {
        super(async (scopes) => "");
        this.setState(ProviderState.SignedIn);
    }

    public async getAccessToken(options?: { scopes?: string[] }): Promise<string> {
        if (!expired || +expired - +new Date() < 1 * 60 * 1000) {
            if (!getTokenFunc) {
                getTokenFunc = (async () => {
                    var data = await getAccessTokenAsync();
                    getTokenFunc = undefined;
                    return data;
                })();
            }
            const token = await getTokenFunc;
            tokenCached = token.token;
            expired = new Date(token.expiresOn);
        }
        return tokenCached;
    }
}

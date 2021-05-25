import { ColorScheme } from "@fluentui/react-northstar";

//https://fluentsite.z22.web.core.windows.net/0.56.0/color-schemes

export interface ITheme {
    siteVariables: {
        colorScheme: {
            default: ColorScheme;
            brand: ColorScheme;
        };
    };
}

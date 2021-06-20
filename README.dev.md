## Prerequisites

 
## Step 1: Register Azure AD application

Register an Azure AD application in your tenant's directory.
1.  Log in to the Azure Portal for your subscription, and go to the "App registrations" blade [here](https://portal.azure.com/#blade/Microsoft_AAD_IAM/ActiveDirectoryMenuBlade/RegisteredApps).

2.  Click on "New registration" to create an Azure AD application.
    - **Name**: The name of your Teams app - if you are following the template for a default deployment, we recommend "Police Incidents".
    - **Supported account types**: Select "	Accounts in this organizational directory only"
     - Leave the "Redirect URI" field blank for now.
 
3.  Click on the "Register" button.

4.  When the app is registered, you'll be taken to the app's "Overview" page. Copy the **Application (client) ID, Directory (tenant) ID**; we will need it later. Verify that the "Supported account types" is set to **My organization only**.
  
5.  On the side rail in the Manage section, navigate to the "Certificates & secrets" section. In the Client secrets section, click on "+ New client secret". Add a description for the secret and select an expiry time. Click "Add".
  
6. Once the client secret is created, copy its **Value**; we will need it later.

At this point you have 3 unique values:

-   Application (client) ID for the bot
-   Client secret for the bot
-   Directory (tenant) ID, which is the same for both apps

We recommend that you copy these values into a text file, using an application like Notepad. We will need these values later.

## Step 2: Start VS project

1.  Clone repo

2. Install NGROK and run it with command: `ngrok http 51717 -host-header=localhost:51717`. It will generate url like  https://xxxxxxxxxx.ngrok.io. Use it as %appDomain% value for the next step.

3. Update values in appsettings.json file with values from step 1 and actual ngrok url.

4. Run project in VS 2019 (F5 button)

5. Open "Source\PoliceIncidents.Tab\ClientApp" folder in VS Code, run "npm i" and "npm run start".

6. Verify it is opened in browser on http://localhost:51717/home and the same on ngrok url  https://xxxxxxx.ngrok.io/home


## Step 3: Set up authentication

1. Note that you have the `%appId%` (step 1) and `%appDomain%` (step 2) values from the previous steps.

   If you have lost these, see this section of the Troubleshooting guide.

2. Go back to the "App Registrations" page [here](https://portal.azure.com/#blade/Microsoft_AAD_IAM/ActiveDirectoryMenuBlade/RegisteredApps).

3. Click on the app you previously created in the application list. Under "Manage", click on "Authentication" to bring up authentication settings.

4. Under "Platform configurations" click on "Add a platform".

5. Select "Web" from the options and now you will see a list of fields to fill in. In the field "Redirect URIs" input the value based on the following:
    -   **Redirect URI**: Enter "%appDomain%/signin-simple-end" and “%appDomain%/signin-oidc”for in Redirect URL field e.g.
 [https://xxxxxxxxxx.ngrok.io/signin-simple-end](https://xxxxxxxxxx.ngrok.io/signin-simple-end),

6. Under "Implicit grant", check both “**Access tokens**” and "**ID tokens**". Click the "Configure: button to add the platform to the Authentication.

6. Click the "Add URI" link to add in the second URI in the format:
 -   **Redirect URI**: Enter “%appDomain%/signin-oidc”for in Redirect URL field
URL field e.g.
 [https://xxxxxxxxxx.ngrok.io/signin-oidc](https://xxxxxxxxxx.ngrok.io/signin-oidc).

8. Click "Save" to commit your changes.

9. Back under "Manage", click on "Expose an API".

10. Click on the "Set" link next to "Application ID URI", and change the value to "`api://%appDomain%/%appId%`" 
e.g. api://xxxxxxxxxx.ngrok.io/12345678-87e3-46a0-b28d-a202db2a7e86.

11. Click "Save and continue" to commit your changes. The view will direct you to "Add a scope"

12. In the "Add a scope" flyout that appears, enter the following values:

      -   **Scope name:**  access_as_user
      -   **Who can consent?:** Admins and users
      -   **Admin and user consent display name:**  [Provide a display name]
      -   **Admin and user consent description:**  Enter “Allow the application to access on behalf of the signed-in user”.

13. Leave remaining fields as it is and click on "Add scope" to commit your changes.

14. Click "Add a client application", under "Authorized client applications". In the flyout that appears, enter the following values:

     -   **Client ID**: `5e3ce6c0-2b1f-4285-8d4b-75ee78787346`
     -   **Authorized scopes**: Select the scope that ends with `access_as_user` . (There should only be 1 scope in the list.)

15. Click "Add application" to commit your changes.

16. Repeat the previous two steps, but with client ID = `1fec8e78-bce4-4aaf-ab1b-5451cc387264`. After this step you should have **two** client applications (`5e3ce6c0-2b1f-4285-8d4b-75ee78787346` and `1fec8e78-bce4-4aaf-ab1b-5451cc387264`) listed under "Authorized client applications".


## Step 4: Assign Permissions to your app
1.  Continuation to Step3 (Above steps).

2.  Select “**API Permissions**” blade from the left-hand side.

3.  Click on “**Add a permission**” button to add permission to your app.

4.  In Microsoft APIs under Select an API label, select the particular service and give the following permissions,

    Under “Commonly used Microsoft APIs”
   
    Select “**Microsoft Graph**”, then select “**Delegated permissions**”
   and check the following permissions,
   
     - openid
     - profile
     - offline_access
     - People.Read
     - User.Read
     - Tasks.ReadWrite
     - User.Read.All
          
    Click on “**Add Permissions**” to commit your changes.

5.  If you are logged in as the Global Administrator, click on the “**Grant admin consent for %tenant-name%**” button to grant admin consent else, inform your Admin to do the same through the portal or follow the steps provided [here](https://docs.microsoft.com/en-us/azure/active-directory/manage-apps/configure-user-consent#grant-admin-consent-through-a-url-request) to create a link and sent it to your Admin for consent.
6. Global Administrator can also grant consent using following link:
https://login.microsoftonline.com/common/adminconsent?client_id=<%appId%>

## Step 5:  Create the Teams app packages

1. Open [Teams admin portal](https://admin.teams.microsoft.com/policies/app-setup) and it will show setup policies.

2. Click on “Global (Org-wide default)” to the policies. Make sure “**Upload custom apps**” toggle is ON in “Global” else change it and save the change.

NOTE: It might take 24 hours for this change to take place organization wide.

3. Open Manifest folder from downloaded Source code folder and make following changes in manifest file.

4. Update the placeholders indicated with <<>> in the manifest to values appropriate for your organization

-   `<<App-Client-ID>>: The application (client) ID which was copied to notepad`
-   `<<Appdomain>>: Replace App domain value.` `e.g."`xxxxxxxxxx.ngrok.io`".`
-  `<<TenantName>>under validDomains: Replace with Tenant name. For help with finding your Tenant name, see instructions` [`here`](https://www.bing.com/search?q=azure+find+tenant+name). `E.g:msteamspoc.onmicrosoft.com`

5. Create a ZIP package with the `manifest.json`, `color.png` and `outline.png`. The two image files are the icons for your app in Teams.
   - Name this package “`manifest.zip”`.
   - Make sure that the 3 files are the _top level_ of the ZIP package, with no nested folders.
       
## Step 6: Run the apps in Microsoft Teams

1. Open Microsoft teams desktop app and click on “Apps” in left navigation.

2. Click on “Upload custom app” which will pop up with two options:
   -  Upload for me or my teams
   - Upload for tenant name (e.g:msteamspoc): Option will be displayed in case if you have permission. Otherwise login with admin account.
   
3. Click on “Upload for tenant name (e.g:msteamspoc)” and browse the zipped package.
4. Uploaded package will be shown under “Built for msteamspoc(tenant name)”. Click on “Police Incidents” and click on “Add”. App will be shown.



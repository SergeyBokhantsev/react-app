export const msal_config = {
    appId: process.env.REACT_APP_AZ_ENTRA_APP_ID,
    redirectUrl: process.env.REACT_APP_AZ_ENTRA_APP_REDIRECT_URL,
    scopes: [
        'https://api.loganalytics.io/data.read'
    ],
    authority: process.env.REACT_APP_AZ_ENTRA_APP_AUTHORITY
};
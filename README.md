# Architecture

1. Frontend UI WebApp (ReactJS)
2. Backend API (Azure Functions)

# Workflow

1. Frontens UI App authorizes the user with organization's MS Entra account. The authorization requires only 'api.loganalytics.io/data.read' permission. Successful authorization provides user's access token to the Frontend app. This token is stored in a session storage only.

2. Frontend UI App makes requests to backend API, providing users access token. Backend API performs requests to Azure Log Analytics API with user's token. The token is not stored on backend.

# App configuration in Azure

1. Create new Azure Entra App Registration
	- In 'API Permissions' tab click on 'Add Permission', open 'APIs my organization uses' tab and select 'Log Analytics API' then check 'Data.Read' scope.
	- Also in 'API Permissions' you can grand admin consent to avoid users to request access from admins.
	- In 'Authentication' tab click 'Add Platform' and add SPA, then set Redirect URL with front UI app URL
	- Also in 'Authentication' tab check the 'Access tokens (for implicit flow)' checkbox
2. Assign 'Reader' role for the Entra App in all required AI Workspaces.
3. Same way, assign 'Reader' role in AI Workspaces for Users that mut be permitted to access the logs.

# References

The login flow (implicit grant OAuth2 flow): https://learn.microsoft.com/en-us/entra/identity-platform/v2-oauth2-implicit-grant-flow

Register AD App, set Data.Read permission and grant access to AI workspace: https://learn.microsoft.com/en-us/azure/azure-monitor/logs/api/access-api
(Then we're going via implicit flow)

How to define the required scope correctly: https://blog.skehan.me/index.php/2022/01/06/react-msal-aad-errors-aadsts650053-or-aadsts500011/

MSAL-browser js lib FAQ: https://github.com/AzureAD/microsoft-authentication-library-for-js/blob/dev/lib/msal-browser/FAQ.md

# Getting Started with Create React App

This project was bootstrapped with [Create React App](https://github.com/facebook/create-react-app).

## Available Scripts

In the project directory, you can run:

### `npm start`

Runs the app in the development mode.\
Open [http://localhost:3000](http://localhost:3000) to view it in your browser.

The page will reload when you make changes.\
You may also see any lint errors in the console.

### `npm test`

Launches the test runner in the interactive watch mode.\
See the section about [running tests](https://facebook.github.io/create-react-app/docs/running-tests) for more information.

### `npm run build`

Builds the app for production to the `build` folder.\
It correctly bundles React in production mode and optimizes the build for the best performance.

The build is minified and the filenames include the hashes.\
Your app is ready to be deployed!

See the section about [deployment](https://facebook.github.io/create-react-app/docs/deployment) for more information.

### `npm run eject`

**Note: this is a one-way operation. Once you `eject`, you can't go back!**

If you aren't satisfied with the build tool and configuration choices, you can `eject` at any time. This command will remove the single build dependency from your project.

Instead, it will copy all the configuration files and the transitive dependencies (webpack, Babel, ESLint, etc) right into your project so you have full control over them. All of the commands except `eject` will still work, but they will point to the copied scripts so you can tweak them. At this point you're on your own.

You don't have to ever use `eject`. The curated feature set is suitable for small and middle deployments, and you shouldn't feel obligated to use this feature. However we understand that this tool wouldn't be useful if you couldn't customize it when you are ready for it.

## Learn More

You can learn more in the [Create React App documentation](https://facebook.github.io/create-react-app/docs/getting-started).

To learn React, check out the [React documentation](https://reactjs.org/).

### Code Splitting

This section has moved here: [https://facebook.github.io/create-react-app/docs/code-splitting](https://facebook.github.io/create-react-app/docs/code-splitting)

### Analyzing the Bundle Size

This section has moved here: [https://facebook.github.io/create-react-app/docs/analyzing-the-bundle-size](https://facebook.github.io/create-react-app/docs/analyzing-the-bundle-size)

### Making a Progressive Web App

This section has moved here: [https://facebook.github.io/create-react-app/docs/making-a-progressive-web-app](https://facebook.github.io/create-react-app/docs/making-a-progressive-web-app)

### Advanced Configuration

This section has moved here: [https://facebook.github.io/create-react-app/docs/advanced-configuration](https://facebook.github.io/create-react-app/docs/advanced-configuration)

### Deployment

This section has moved here: [https://facebook.github.io/create-react-app/docs/deployment](https://facebook.github.io/create-react-app/docs/deployment)

### `npm run build` fails to minify

This section has moved here: [https://facebook.github.io/create-react-app/docs/troubleshooting#npm-run-build-fails-to-minify](https://facebook.github.io/create-react-app/docs/troubleshooting#npm-run-build-fails-to-minify)

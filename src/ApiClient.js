import {api_config} from "./config.js"

class ApiClient {

    credentials = {};
    workspaces = {};

    constructor(credentials, workspaces) {
        this.credentials = credentials;
        this.workspaces = workspaces;
    }

    async getWorkspaces() {
        const response = await fetch(api_config.url + 'get-workspaces');
        const data = await response.json();
        return data;
    }
}

export default ApiClient;
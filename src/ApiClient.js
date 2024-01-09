import {api_config} from "./config.js"

class ApiClient {

    credentials = {};

    constructor(credentials) {
        this.credentials = credentials;
    }

    getFetchParams() {
        return {
            method: 'GET',
            headers: {
                'Authorization': this.credentials.type + ' ' + this.credentials.accessToken
            }
        };        
    }

    async getWorkspaces() {
        console.log('API getWorkspaces');
        const response = await fetch(api_config.url + 'get-workspaces');
        const data = await response.json();
        return data;
    }

    async searchJobById(id, workspaces, range, related) {
        const response = await fetch(api_config.url + 'search-by-job-id?jobId=' + id +'&workspaceId=' + workspaces.join(':') + '&days=' + range + '&related=' + related, 
                                     this.getFetchParams());

        const data = await response.json();
        return data;
    }
}

export default ApiClient;
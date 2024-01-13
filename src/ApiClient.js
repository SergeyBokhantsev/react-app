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

    stringToByteArray(str) {
        const encoder = new TextEncoder();
        return encoder.encode(str);
      }
      
    byteArrayToBase64(byteArray) {
        let binary = '';
        byteArray.forEach(byte => {
          binary += String.fromCharCode(byte);
        });
        return btoa(binary);
      }

    async getWorkspaces() {
        const response = await fetch(api_config.url + 'get-workspaces');
        const data = await response.json();
        return data;
    }

    async searchJobById(id, workspaces, range, related) {
        const response = await fetch(api_config.url + 'search-by-job-id?jobId=' + id +'&workspaceId=' + workspaces.join(':') + '&days=' + range + '&related=' + related, 
                                     this.getFetchParams());

        if (response.status === 404)
            throw new Error("Job was not found");

        if (response.status !== 200)
            throw new Error(`Unexpected server responce: ${response.status}`);

        const data = await response.json();
        return data;
    }

    async downloadJobLog(url) {
        const bytes = this.stringToByteArray(url);
        const base64 = this.byteArrayToBase64(bytes);
        const response = await fetch(api_config.url + 'get-job-log-by-url?url=' + base64);
        const lines = await response.json();
        return lines;
    }
}

export default ApiClient;
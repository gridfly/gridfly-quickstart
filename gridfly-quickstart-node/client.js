import fs from 'fs-extra';
import path from 'path';
import { v4 as uuidv4 } from 'uuid';
import ApiClient from './gridfly_api_client/src/ApiClient';
import AuthenticationApi from './gridfly_api_client/src/api/AuthenticationApi';
import SyncJobCreationApi from './gridfly_api_client/src/api/SyncJobCreationApi';
import AsyncJobCreationApi from './gridfly_api_client/src/api/AsyncJobCreationApi';
import AsyncJobStatusApi from './gridfly_api_client/src/api/AsyncJobStatusApi';

class GridflyClient {
    constructor(baseUrl = "https://dev.api.gridfly.io/v1") {
        this.apiClient = new ApiClient(baseUrl);
        this.authApi = new AuthenticationApi(this.apiClient);
        this.syncApi = new SyncJobCreationApi(this.apiClient);
        this.asyncApi = new AsyncJobCreationApi(this.apiClient);
        this.statusApi = new AsyncJobStatusApi(this.apiClient);
    }

    async authenticate(clientId, clientSecret) {
        console.log("Authenticating...");
        const opts = {
            clientId: clientId,
            clientSecret: clientSecret,
            grantType: 'client_credentials'
        };

        try {
            const data = await this.authApi.getAccessToken(opts);
            const accessToken = data.access_token;
            if (!accessToken) {
                throw new Error("Failed to authenticate: No token received");
            }

            // Set token for future requests
            this.apiClient.authentications['bearerAuth'].accessToken = accessToken;
            console.log("Authenticated successfully.");
            return accessToken;
        } catch (error) {
            throw new Error(`Authentication failed: ${error.message || error}`);
        }
    }

    async generateExcelSync(gzippedHtmlPath, outputDirectory) {
        console.log(`Generating Excel synchronously for ${gzippedHtmlPath}...`);
        const fileBuffer = await fs.readFile(gzippedHtmlPath);

        const opts = {
            body: fileBuffer
        };

        try {
            const responseAndData = await this.syncApi.createSyncJobWithHttpInfo(opts);
            const response = responseAndData.response;

            await fs.ensureDir(outputDirectory);
            const filename = `${uuidv4()}.xlsx`;
            const outputPath = path.join(outputDirectory, filename);
            
            // response.body for superagent contains the binary data if it was able to parse it
            await fs.writeFile(outputPath, response.body);
            console.log(`Excel generated and saved to ${outputPath}`);
            return outputPath;
        } catch (error) {
            const detail = error.response ? error.response.text : (error.message || error);
            throw new Error(`Failed to generate Excel (sync): ${detail}`);
        }
    }

    async generateExcelAsync(gzippedHtmlPath) {
        console.log(`Generating Excel asynchronously for ${gzippedHtmlPath}...`);
        const fileBuffer = await fs.readFile(gzippedHtmlPath);

        const opts = {
            body: fileBuffer
        };

        try {
            const data = await this.asyncApi.createAsyncJob(opts);
            const jobId = data.jobId;
            console.log(`Job created with ID: ${jobId}. Polling for status...`);
            await this._pollForStatus(jobId);
            return jobId;
        } catch (error) {
            let statusCode = "Unknown";
            let responseBody = error.message || error;

            if (error.response) {
                statusCode = error.status || error.response.status;
                try {
                    responseBody = JSON.parse(error.response.text);
                } catch (e) {
                    responseBody = error.response.text || error.response.body || error.message;
                }
            }

            console.log(`API returned error with status code ${statusCode}:
${JSON.stringify(responseBody, null, 2)}`);
            
            process.exit(1);
        }
    }

    async _pollForStatus(jobId) {
        while (true) {
            let statusData;
            try {
                statusData = await this.statusApi.getJobStatus(jobId);
            } catch (error) {
                console.warn(`Warning: Error polling job status: ${error.message || error}`);
            }

            if (statusData) {
                const { status } = statusData;

                if (status === "PENDING" || status === "RUNNING") {
                    console.log(`Status: ${status}`);
                } else {
                    console.log(`Job completed:
${JSON.stringify(statusData, null, 2)}`);
                    break;
                }
            }

            await new Promise(resolve => setTimeout(resolve, 1000));
        }
    }
}

export default GridflyClient;

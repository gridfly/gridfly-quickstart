import os
import uuid
import time
import json
from io import BytesIO
from gridfly_api_client.client import AuthenticatedClient, Client
from gridfly_api_client.models.token_request import TokenRequest
from gridfly_api_client.api.authentication import get_access_token
from gridfly_api_client.api.sync_job_creation import create_sync_job
from gridfly_api_client.api.async_job_creation import create_async_job
from gridfly_api_client.api.async_job_status import get_job_status
from gridfly_api_client.types import File

class GridflyClient:
    def __init__(self, base_url="https://dev.api.gridfly.io/v1"):
        self.base_url = base_url
        self.client = None

    def authenticate(self, client_id, client_secret):
        print("Authenticating...")
        # Initial client to call /token
        initial_client = Client(base_url=self.base_url)
        token_request = TokenRequest(client_id=client_id, client_secret=client_secret)
        
        token_response = get_access_token.sync(client=initial_client, body=token_request)
        if not token_response:
            raise Exception("Failed to authenticate: No token received")
        
        self.client = AuthenticatedClient(
            base_url=self.base_url, 
            token=token_response.access_token,
            prefix="Bearer",
            auth_header_name="Authorization"
        )
        print("Authenticated successfully.")

    def generate_excel_sync(self, gzipped_html_path, output_directory):
        if not self.client:
            raise Exception("Client not authenticated")

        print(f"Generating Excel synchronously for {gzipped_html_path}...")
        with open(gzipped_html_path, "rb") as f:
            file_data = f.read()
        
        file_to_upload = File(payload=BytesIO(file_data))
        
        # We use sync_detailed because the generator omitted the 200 response (binary) from the parsed output
        response = create_sync_job.sync_detailed(client=self.client, body=file_to_upload)
        
        if response.status_code == 200:
            filename = f"{uuid.uuid4()}.xlsx"
            output_path = os.path.join(output_directory, filename)
            with open(output_path, "wb") as f:
                f.write(response.content)
            print(f"Excel generated and saved to {output_path}")
        else:
            raise Exception(f"Failed to generate Excel: {response.status_code} - {response.content.decode('utf-8', errors='ignore')}")

    def generate_excel_async(self, gzipped_html_path):
        if not self.client:
            raise Exception("Client not authenticated")

        print(f"Generating Excel asynchronously for {gzipped_html_path}...")
        with open(gzipped_html_path, "rb") as f:
            file_data = f.read()
        
        file_to_upload = File(payload=BytesIO(file_data))
        
        # Using detailed because response was omitted from parsed output
        response = create_async_job.sync_detailed(client=self.client, body=file_to_upload)
        
        if response.status_code in [201, 202]:
            job_data = json.loads(response.content)
            job_id = job_data.get("jobId")
            print(f"Job created with ID: {job_id}. Polling for status...")
            self._poll_for_status(job_id)
        else:
            try:
                response_body = json.loads(response.content)
                print(f"API returned error with status code {response.status_code}:\n{json.dumps(response_body, indent=2)}")
            except:
                print(f"API returned error with status code {response.status_code}:\n{response.content.decode('utf-8', errors='ignore')}")
            import sys
            sys.exit(1)

    def _poll_for_status(self, job_id):
        while True:
            response = get_job_status.sync_detailed(client=self.client, job_id=job_id)
            if response.status_code == 200:
                status_data = json.loads(response.content)
                status = status_data.get("status")
                
                if status in ["PENDING", "RUNNING"]:
                    print(f"Status: {status}")
                else:
                    print(f"Job completed:\n{json.dumps(status_data, indent=2)}")
                    break
            else:
                print(f"Warning: Failed to get job status: {response.status_code}")
            
            time.sleep(1)

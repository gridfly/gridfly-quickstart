package io.gridfly.quickstart.client;

import com.fasterxml.jackson.databind.ObjectMapper;
import io.gridfly.client.generated.ApiClient;
import io.gridfly.client.generated.ApiException;
import io.gridfly.client.generated.ApiResponse;
import io.gridfly.client.generated.api.AsyncJobCreationApi;
import io.gridfly.client.generated.api.AsyncJobStatusApi;
import io.gridfly.client.generated.model.JobCreationResponse;
import io.gridfly.client.generated.model.JobStatusResponse;
import java.io.File;
import java.io.IOException;

public class AsyncApiClient extends BaseApiClient {

  public void generateExcel(String clientId, String clientSecret, File sourceFile)
      throws IOException {

    try {
      ApiClient client = authenticate(clientId, clientSecret);
      AsyncJobCreationApi jobCreationApi = new AsyncJobCreationApi(client);
      AsyncJobStatusApi jobStatusApi = new AsyncJobStatusApi(client);

      ApiResponse<JobCreationResponse> jobCreationResponse =
          jobCreationApi.createAsyncJobWithHttpInfo(true, true, sourceFile);

      boolean finished = false;

      while (!finished) {
        ApiResponse<JobStatusResponse> statusResponse =
            jobStatusApi.getJobStatusWithHttpInfo(jobCreationResponse.getData().getJobId());

        if (statusResponse.getData().getStatus() == JobStatusResponse.StatusEnum.PENDING
            || statusResponse.getData().getStatus() == JobStatusResponse.StatusEnum.RUNNING) {
          System.out.println("Status: " + statusResponse.getData().getStatus());
          try {
            Thread.sleep(1000);
          } catch (InterruptedException e) {
            throw new RuntimeException(e);
          }
        } else {
          ObjectMapper mapper = new ObjectMapper();
          System.out.println(
              "Job completed:\n"
                  + mapper
                      .writerWithDefaultPrettyPrinter()
                      .writeValueAsString(statusResponse.getData()));
          finished = true;
        }
      }
    } catch (ApiException ex) {
      ObjectMapper mapper = new ObjectMapper();
      System.out.println(
          "API returned error with status code "
              + ex.getCode()
              + ":\n"
              + mapper
                  .writerWithDefaultPrettyPrinter()
                  .writeValueAsString(mapper.readValue(ex.getResponseBody(), Object.class)));

      System.exit(1);
    }
  }
}

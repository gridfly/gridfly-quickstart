package io.gridfly.quickstart.client;

import com.fasterxml.jackson.databind.ObjectMapper;
import io.gridfly.client.generated.ApiClient;
import io.gridfly.client.generated.ApiException;
import io.gridfly.client.generated.api.SyncJobCreationApi;
import java.io.File;
import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.StandardCopyOption;

public class SyncApiClient extends BaseApiClient {

  public void generateExcel(
      String clientId, String clientSecret, File sourceFile, File outputDirectory)
      throws IOException {

    try {
      ApiClient client = authenticate(clientId, clientSecret);
      SyncJobCreationApi syncJobCreationApi = new SyncJobCreationApi(client);
      File generated = syncJobCreationApi.createSyncJob(sourceFile);
      File permanentFile = new File(outputDirectory, generated.getName());
      Files.move(generated.toPath(), permanentFile.toPath(), StandardCopyOption.REPLACE_EXISTING);

      System.out.println("Export completed: file saved to: " + permanentFile.getAbsolutePath());

    } catch (ApiException ex) {
      ObjectMapper mapper = new ObjectMapper();
      String responseBody = ex.getResponseBody();
      String formattedResponseBody =
          responseBody == null || responseBody.isEmpty()
              ? ""
              : (mapper
                  .writerWithDefaultPrettyPrinter()
                  .writeValueAsString(mapper.readValue(ex.getResponseBody(), Object.class)));

      System.out.println(
          "API returned error with status code " + ex.getCode() + ":\n" + formattedResponseBody);
    }
  }
}

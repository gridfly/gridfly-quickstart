package io.gridfly.quickstart;

import io.gridfly.client.generated.ApiException;
import io.gridfly.quickstart.client.AsyncApiClient;
import io.gridfly.quickstart.client.SyncApiClient;

import java.io.ByteArrayOutputStream;
import java.io.File;
import java.io.IOException;
import java.io.OutputStream;
import java.nio.charset.StandardCharsets;
import java.nio.file.Files;
import java.nio.file.Path;
import java.util.List;
import java.util.Map;
import java.util.zip.GZIPOutputStream;

public class QuickStartRunner {

  private final List<String> examples = List.of("example-1", "example-2", "example-3");

  private final ConfigurationLoader configurationLoader = new ConfigurationLoader();
  private final DataLoader dataLoader = new DataLoader();
  private final HtmlGenerator htmlGenerator = new HtmlGenerator();
  private final AsyncApiClient asyncApiClient = new AsyncApiClient();
  private final SyncApiClient syncApiClient = new SyncApiClient();

  public QuickStartRunner() throws IOException, ApiException {
    Map<String, String> configuration = configurationLoader.loadConfiguration();
    String clientId = configuration.get("client_id");
    String clientSecret = configuration.get("client_secret");

    for (String example : examples) {
      Map<String, Object> data = dataLoader.loadData("../data/" + example + ".json");
      String html = htmlGenerator.generateHTML(example + ".vm", data);
      File gzipped = gzipCompress(html.getBytes(StandardCharsets.UTF_8));

      System.out.println("Beginning Excel generation for " + example);

      if (configuration.get("mode").equals("async")) {
        asyncApiClient.generateExcel(clientId, clientSecret, gzipped);
      } else {
        File outputDirectory = new File(configuration.get("output_directory"));
        if (!Files.exists(outputDirectory.toPath())) {
          outputDirectory.mkdir();
        }
        syncApiClient.generateExcel(clientId, clientSecret, gzipped, outputDirectory);
      }
    }
  }

  public static File gzipCompress(byte[] uncompressedData) throws IOException {
    ByteArrayOutputStream bos = new ByteArrayOutputStream(uncompressedData.length);
    Path temp = Files.createTempFile("gridfly-quickstart", ".gz");

    try (OutputStream fos = Files.newOutputStream(temp);
        GZIPOutputStream gzip = new GZIPOutputStream(fos)) {
      gzip.write(uncompressedData);
    }

    return temp.toFile();
  }

  public static void main(String[] args) throws IOException, ApiException {
    new QuickStartRunner();
    System.exit(0);
  }

  static class Example {
    String name;
    boolean usingAsyncApi;

    public Example(String name, boolean usingAsyncApi) {
      this.name = name;
      this.usingAsyncApi = usingAsyncApi;
    }
  }
}

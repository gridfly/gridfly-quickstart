package io.gridfly.quickstart;

import com.fasterxml.jackson.core.type.TypeReference;
import com.fasterxml.jackson.databind.ObjectMapper;
import java.io.IOException;
import java.io.InputStream;
import java.nio.file.Files;
import java.nio.file.Paths;
import java.util.Map;

public class DataLoader {

  public Map<String, Object> loadData(String path) throws IOException {

    try (InputStream in = Files.newInputStream(Paths.get(path))) {
      return new ObjectMapper().readValue(in, new TypeReference<Map<String, Object>>() {});
    }
  }
}

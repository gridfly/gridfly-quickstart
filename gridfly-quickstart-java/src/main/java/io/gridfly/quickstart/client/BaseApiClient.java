package io.gridfly.quickstart.client;

import io.gridfly.client.generated.ApiClient;
import io.gridfly.client.generated.ApiException;
import io.gridfly.client.generated.api.AuthenticationApi;
import io.gridfly.client.generated.model.TokenResponse;

public class BaseApiClient {

  public ApiClient authenticate(String clientId, String clientSecret) throws ApiException {
    ApiClient client = new ApiClient();

    AuthenticationApi authenticationApi = new AuthenticationApi(client);
    TokenResponse tokenResponse =
        authenticationApi.getAccessToken("client_credentials", clientId, clientSecret);

    client
        .setConnectTimeout(30000)
        .setReadTimeout(30000)
        .addDefaultHeader("Authorization", "Bearer " + tokenResponse.getAccessToken());

    return client;
  }
}

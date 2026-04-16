### QuickStart .Net Project

This project contains a simple example of how to call the GridFlyApi from .NET.

It will generate an API client from the GridFly API specification

It uses canned JSON as the datasource and Razor as the templating library, however any datasource and templating
library can be used.

Esnure your API client id and secret are set in the `config.json` file before running the project.

Requirements:

- .NET Framework 8+

```bash
dotnet build
dotnet run
```

The HTML generated from the data and templates will be sent to the API. In synchronous mode the generated Excel files will be saved in
the folder specified in the `config.json` file. In asynchronous mode the generated link can be invoked manually within 30 seconds.

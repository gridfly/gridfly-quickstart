using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Gridfly.Client;
using Gridfly.QuickStart;
using Microsoft.Extensions.Configuration;

namespace Gridfly.QuickStart
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var configLoader = new ConfigurationLoader();
                var configPath = Path.Combine("..", "config.json");
                var config = configLoader.LoadConfig(configPath);

                var clientId = config["client_id"];
                var clientSecret = config["client_secret"];
                var mode = config["mode"]?.ToLower() ?? "sync";

                if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
                {
                    Console.WriteLine($"Please provide clientId and clientSecret in {Path.GetFullPath(configPath)}");
                    return;
                }

                var dataLoader = new DataLoader();
                var dataDir = Path.Combine("..", "data");

                var outputDir = config["output_directory"] ?? "downloads";
                if (!Directory.Exists(outputDir))
                {
                    Directory.CreateDirectory(outputDir);
                }

                var syncClient = new SyncApiClient(clientId, clientSecret);
                var asyncClient = new AsyncApiClient(clientId, clientSecret);

                for (int i = 1; i <= 3; i++)
                {
                    Console.WriteLine($"Generating Example {i}...");
                    var data = dataLoader.LoadData(Path.Combine(dataDir, $"example-{i}.json"));
                    var helper = new ReportHelper(data);
                    var generator = new HtmlGenerator($"example-{i}", helper);
                    var html = await generator.GenerateHtml();
                    // File.WriteAllText($"example-{i}.html", html);

                    byte[] gzippedBytes;
                    using (var ms = new MemoryStream())
                    {
                        using (var gzip = new GZipStream(ms, CompressionLevel.Optimal, true))
                        {
                            var bytes = Encoding.UTF8.GetBytes(html);
                            gzip.Write(bytes, 0, bytes.Length);
                        }
                        gzippedBytes = ms.ToArray();
                    }

                    var outputFileName = Path.Combine(outputDir, $"example-{i}.xlsx");
                    using (var ms = new MemoryStream(gzippedBytes))
                    {
                        if (mode == "async")
                        {
                            await asyncClient.Export(ms);
                        }
                        else
                        {
                            var response = await syncClient.Export(ms);
                            File.WriteAllBytes(outputFileName, response);
                            Console.WriteLine($"Example {i} XLSX saved to {outputFileName}");
                        }
                    }
                }

                Console.WriteLine("Done!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                if (ex is ApiException apiEx)
                {
                    Console.WriteLine($"API returned error with status code {apiEx.StatusCode}:");
                    try {
                        var parsed = Newtonsoft.Json.JsonConvert.DeserializeObject(apiEx.Response!);
                        Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(parsed, Newtonsoft.Json.Formatting.Indented));
                    } catch {
                        Console.WriteLine(apiEx.Response);
                    }
                }
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Error: {ex.InnerException.Message}");
                }
            }
        }

        static async Task DownloadFile(string url, string fileName)
        {
            using (var httpClient = new HttpClient())
            {
                var data = await httpClient.GetByteArrayAsync(url);
                await File.WriteAllBytesAsync(fileName, data);
            }
        }
    }
}

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Gridfly.Client;

namespace Gridfly.QuickStart
{
    public class AsyncApiClient : BaseApiClient
    {
        public AsyncApiClient(string clientId, string clientSecret) : base(clientId, clientSecret)
        {
        }

        public async Task Export(Stream htmlStream)
        {
            var client = await Authenticate();
            JobCreationResponse asyncResponse;
            try
            {
                asyncResponse = await client.CreateAsyncJobAsync(false, true, htmlStream);
            }
            catch (ApiException ex) when (ex.StatusCode == 201)
            {
                asyncResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<JobCreationResponse>(ex.Response!)!;
            }
            catch (ApiException ex)
            {
                Console.WriteLine($"API returned error with status code {ex.StatusCode}:");
                try {
                    var parsed = Newtonsoft.Json.JsonConvert.DeserializeObject(ex.Response!);
                    Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(parsed, Newtonsoft.Json.Formatting.Indented));
                } catch {
                    Console.WriteLine(ex.Response);
                }
                Environment.Exit(1);
                return;
            }
            
            var jobId = asyncResponse!.JobId;
            Console.WriteLine($"Job created with ID: {jobId}. Polling for status...");

            while (true)
            {
                try
                {
                    var jobStatus = await client.GetJobStatusAsync(jobId);
                    if (jobStatus.Status == JobStatusResponseStatus.PENDING || jobStatus.Status == JobStatusResponseStatus.RUNNING)
                    {
                        Console.WriteLine($"Status: {jobStatus.Status}");
                    }
                    else
                    {
                        Console.WriteLine("Job completed:");
                        Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(jobStatus, Newtonsoft.Json.Formatting.Indented));
                        break;
                    }
                    await Task.Delay(1000);
                }
                catch (ApiException ex)
                {
                    Console.WriteLine($"API returned error with status code {ex.StatusCode}:");
                    try {
                        var parsed = Newtonsoft.Json.JsonConvert.DeserializeObject(ex.Response!);
                        Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(parsed, Newtonsoft.Json.Formatting.Indented));
                    } catch {
                        Console.WriteLine(ex.Response);
                    }
                    Environment.Exit(1);
                    return;
                }
            }
        }
    }
}

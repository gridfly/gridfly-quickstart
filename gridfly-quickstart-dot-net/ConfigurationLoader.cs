using Microsoft.Extensions.Configuration;
using System.IO;

namespace Gridfly.QuickStart
{
    public class ConfigurationLoader
    {
        public IConfiguration LoadConfig(string path)
        {
            var fullPath = Path.GetFullPath(path);
            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException($"Configuration file not found: {fullPath}", fullPath);
            }
            return new ConfigurationBuilder()
                .AddJsonFile(fullPath)
                .Build();
        }
    }
}

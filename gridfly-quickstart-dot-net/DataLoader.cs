using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace Gridfly.QuickStart
{
    public class DataLoader
    {
        public IDictionary<string, object> LoadData(string path)
        {
            var json = File.ReadAllText(path);
            var token = JToken.Parse(json);
            return ToObject(token) as IDictionary<string, object> ?? new Dictionary<string, object>();
        }

        private object ToObject(JToken token)
        {
            switch (token.Type)
            {
                case JTokenType.Object:
                    return token.Children<JProperty>()
                                .ToDictionary(prop => prop.Name, prop => ToObject(prop.Value));
                case JTokenType.Array:
                    return token.Select(ToObject).ToList();
                default:
                    return ((JValue)token).Value ?? string.Empty;
            }
        }
    }
}

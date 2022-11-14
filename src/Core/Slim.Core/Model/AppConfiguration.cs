

using Newtonsoft.Json;

namespace Slim.Core.Model
{
    public class AppConfiguration
    {
        public const string ConnectionStringsOptions = "ConnectionStrings";


        [JsonProperty("ConnectionStrings")] public ConnectionStrings ConnectionStrings { get; set; } = new();
    }
    public class ConnectionStrings
    {
        [JsonProperty("DefaultConnection")] public string DefaultConnection { get; set; } = string.Empty;

        [JsonProperty("AppConfiguration")] public string AppConfiguration { get; set; } = string.Empty;

        [JsonProperty("ProductionConnection")] public string ProductionConnection { get; set; } = string.Empty;
    }
}

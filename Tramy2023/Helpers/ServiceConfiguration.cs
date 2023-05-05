using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Tramy.Backend.Helpers
{
    public class ServiceConfiguration
    {
        public string MongoConnection { get; set; }
        public string MongoMainDb { get; set; }
        public string MongoLogsDb { get; set; }
    }

    public static class ConfigurationFactory
    {
        private static readonly IDeserializer Deserializer =
            new DeserializerBuilder()
            .IgnoreUnmatchedProperties()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .Build();

        public static ServiceConfiguration FromYaml(string fileName)
        {
            using var stream = File.OpenRead(fileName); ;
            using var reader = new StreamReader(stream);
            return Deserializer.Deserialize<ServiceConfiguration>(reader.ReadToEnd());
        }
    }
}

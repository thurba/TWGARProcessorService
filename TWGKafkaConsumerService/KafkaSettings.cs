
namespace TWGKafkaConsumerService;

public class KafkaSettings
{
    public string BootstrapServers { get; set; } = string.Empty;
    public string SecurityProtocol { get; set; } = "PLAINTEXT"; // Default
    public string SaslMechanism { get; set; } = string.Empty;
    public string SaslUsername { get; set; } = string.Empty;
    public string SaslPassword { get; set; } = string.Empty;
    public string GroupId { get; set; } = "laserfiche-group";
    public string Topic { get; set; } = string.Empty;
    public int ApiClientRetries { get; set; } = 2;
    public int ApiClientRetryDelay { get; set; } = 60; // in seconds

}

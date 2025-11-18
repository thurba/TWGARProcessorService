namespace TWGKafkaConsumerService;

using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using LFApiClient;
public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IOptions<KafkaSettings> _settings;
    private readonly ApiClient _apiClient;
    private readonly IHostApplicationLifetime _hostApplicationLifetime;

    public Worker(ILogger<Worker> logger, IOptions<KafkaSettings> options, ApiClient apiClient, IHostApplicationLifetime hostApplicationLifetime)
    {
        _logger = logger;
        _settings = options;
        _apiClient = apiClient;
        _hostApplicationLifetime = hostApplicationLifetime;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Kafka Consumer Service Starting");

        var config = new ConsumerConfig
        {
            BootstrapServers = _settings.Value.BootstrapServers,
            GroupId = _settings.Value.GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoOffsetStore = false,
            EnableAutoCommit = true,
            SecurityProtocol = _settings.Value.SecurityProtocol switch
            {
                "SASL_SSL" => Confluent.Kafka.SecurityProtocol.SaslSsl,
                "PLAINTEXT" => Confluent.Kafka.SecurityProtocol.Plaintext,
                _ => throw new InvalidOperationException("Unsupported security protocol")
            },
            SaslUsername = _settings.Value.SecurityProtocol == "SASL_SSL" ? _settings.Value.SaslUsername : null,
            SaslPassword = _settings.Value.SecurityProtocol == "SASL_SSL" ? _settings.Value.SaslPassword : null,
            SaslMechanism = _settings.Value.SecurityProtocol == "SASL_SSL" ? (
               _settings.Value.SaslMechanism switch
               {
                   "PLAIN" => Confluent.Kafka.SaslMechanism.Plain,
                   _ => throw new InvalidOperationException("Unsupported security mechanism")
               }) : null
        };

        _logger.LogInformation("Bootstrap Servers: {BootstrapServers}", config.BootstrapServers);

        using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
        {
            try
            {
                _logger.LogInformation("Subscribing to topic {topic}", _settings.Value.Topic);
                consumer.Subscribe(_settings.Value.Topic);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error subscribing to topic {topic}, error detail: {detail}", _settings.Value.Topic, ex.Message);
                return;
            }

            _logger.LogInformation("Starting Kafka consume loop");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var cr = consumer.Consume(stoppingToken);
                    log4net.LogicalThreadContext.Properties["X-Correlation-ID"] = Guid.NewGuid().ToString();
                    _logger.LogInformation("Processing message with key {Key} at partition {Partition}, offset {Offset}, timestamp {timestamp}", cr.Message.Key, cr.Partition, cr.Offset, cr.Message.Timestamp.UtcDateTime);
                    _logger.LogInformation("Received: {Message}", cr.Message.Value);


                    var kafkaMessage = JsonSerializer.Deserialize<KafkaMessage>(cr.Message.Value);

                    if (kafkaMessage != null)
                    {
                        if (kafkaMessage != null)
                        {
                            string invoiceFileName = kafkaMessage?.Barcode?.ToString() ?? "UnknownInvoice";
                            var addressLinesVendor = kafkaMessage?.Parties?.Find(p => p.Code == "INVOICE FROM")?.Location?.Address;
                            var addressLinesCustomer = kafkaMessage?.Parties?.Find(p => p.Code == "BILL TO")?.Location?.Address;
                            var addressLinesDelivery = kafkaMessage?.Parties?.Find(p => p.Code == "SHIP TO")?.Location?.Address;

                            var metadata = new LaserficheMetadata
                            {
                                // Map fields from kafkaMessage.Value to LaserficheMetadata
                                InvoiceNumber = kafkaMessage?.Identifier != null ? kafkaMessage.Identifier : "UnknownInvoiceNumber",
                                PONumber = kafkaMessage?.PurchaseOrderIdentifier != null ? kafkaMessage.PurchaseOrderIdentifier : "",
                                InvoiceDate = DateTime.TryParse(kafkaMessage?.InvoiceDate, out var invoiceDate) ? invoiceDate : default,
                                InvoiceAmount = kafkaMessage.GrossTotalAmount,
                                BarcodeNumber = long.TryParse(kafkaMessage?.Barcode, out var invoiceBarcode) ? invoiceBarcode : 0,
                                VendorName = kafkaMessage.Parties?.Find(p => p.Code == "INVOICE FROM")?.Name != null ? kafkaMessage.Parties?.Find(p => p.Code == "INVOICE FROM")?.Name : "UnknownVendor",
                                VendorCode = kafkaMessage.Parties?.Find(p => p.Code == "INVOICE FROM")?.Location?.Identifier,
                                CustomerNumber = kafkaMessage.Parties?.Find(p => p.Code == "BILL TO")?.Location?.Identifier,
                                CustomerName = kafkaMessage.Parties?.Find(p => p.Code == "BILL TO")?.Name,
                                DeliveryName = kafkaMessage.Parties?.Find(p => p.Code == "SHIP TO")?.Location?.Name,
                                DeliveryLocationID = kafkaMessage.Parties?.Find(p => p.Code == "SHIP TO")?.Location?.Identifier,
                                TotalNetAmount = kafkaMessage.NetTotalAmount,
                                TotalTaxAmount = kafkaMessage.Tax?.Amount,
                                VendorGST = kafkaMessage.Parties?.Find(p => p.Code == "INVOICE FROM")?.TaxIdentifier,
                                VendorAddress = (addressLinesVendor != null && addressLinesVendor.AddressLines != null) ? string.Join(", ", addressLinesVendor.AddressLines) : null,
                                CustomerAddress = (addressLinesCustomer != null && addressLinesCustomer.AddressLines != null) ? string.Join(", ", addressLinesCustomer.AddressLines) : null,
                                DeliveryAddress = (addressLinesDelivery != null && addressLinesDelivery.AddressLines != null) ? string.Join(", ", addressLinesDelivery.AddressLines) : null,
                                SupplierNote = kafkaMessage?.SupplierNote != null ? kafkaMessage.SupplierNote : "",

                                // Assign multi-value fields 
                                LineNumber = kafkaMessage.Lines?.Select(l => l.Number).ToList() ?? [],
                                LineBarcode = kafkaMessage.Lines?.Select(l => l.GlobalTradeItemNumber).ToList() ?? [],
                                LineDescription = kafkaMessage.Lines?.Select(l => l.Description).ToList() ?? [],
                                LineQuantity = kafkaMessage.Lines?.Select(l => l.Quantity).ToList() ?? [],
                                LineUnitPrice = kafkaMessage.Lines?.Select(l => l.UnitPrice).ToList() ?? [],
                                LineDiscount = kafkaMessage.Lines?.Select(l => l.DiscountPercentage).ToList() ?? [],
                                LineDiscountAmount = kafkaMessage.Lines?.Select(l => l.DiscountAmount).ToList() ?? [],
                                LineAmount = kafkaMessage.Lines?.Select(l => l.Total).ToList() ?? []

                            };

                            _logger.LogInformation("Deserialized metadata for invoice {InvoiceFileName}: {Metadata}", invoiceFileName, JsonSerializer.Serialize(metadata));

                            int attempt = 0;
                            int maxRetryAttempts = _settings.Value.ApiClientRetries;
                            while (attempt < maxRetryAttempts && !stoppingToken.IsCancellationRequested)
                            {

                                bool success = await _apiClient.SendInvoiceMetadataToLF(invoiceFileName, metadata, stoppingToken);

                                if (success)
                                {
                                    _logger.LogInformation("Call to SendInvoiceMetadataToLF succeeded for {FileName}", invoiceFileName);
                                    break; // Exit retry loop on success
                                }
                                else
                                {
                                    attempt++;
                                    _logger.LogWarning("Call to SendInvoiceMetadataToLF failed (attempt {attempt}). Retrying in {delay} seconds. Filename: {FileName}", attempt, _settings.Value.ApiClientRetryDelay, invoiceFileName);
                                    if (attempt < maxRetryAttempts)
                                    {
                                        await Task.Delay(_settings.Value.ApiClientRetryDelay * 1000, stoppingToken); // Delay in milliseconds
                                    }
                                    else
                                    {
                                        _logger.LogError("Max retry attempts reached for {FileName}. Giving up.", invoiceFileName);
                                        _hostApplicationLifetime.StopApplication(); // Exit the service if max retries reached
                                        return;
                                    }

                                }

                            }

                        }
                        else
                        {
                            _logger.LogWarning("Received null Kafka message or value.");
                        }

                        consumer.StoreOffset(cr);
                        _logger.LogInformation("Stored offset for message with key {Key} at partition {Partition}, offset {Offset}, timestamp {timestamp}", cr.Message.Key, cr.Partition, cr.Offset, cr.Message.Timestamp.UtcDateTime);


                    }
                    else
                    {
                        _logger.LogWarning("No messages deserialized from Kafka payload.");
                    }
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Operation cancelled, stopping consumer.");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error deserialising  Kafka message or assigning metadata, error detail: {detail}", ex.Message);
                    _hostApplicationLifetime.StopApplication(); // Exit the service if metadata error occurs - unrecoverable as the message will never get processed correctly
                    break;
                }
            }

            consumer.Close();
            return;
        }
    }
}
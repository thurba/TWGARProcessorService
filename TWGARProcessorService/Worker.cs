namespace TWGARProcessorService;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using LFApiClient;

using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.IO;


public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IOptions<ARProcessorSettings> _settings;
    private readonly ApiClient _apiClient;
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly string _watchPath = string.Empty;
    private FileSystemWatcher _watcher;
    private CancellationToken _stoppingToken;


    public Worker(ILogger<Worker> logger, IOptions<ARProcessorSettings> options, ApiClient apiClient, IHostApplicationLifetime hostApplicationLifetime)
    {
        _logger = logger;
        _settings = options;
        _apiClient = apiClient;
        _hostApplicationLifetime = hostApplicationLifetime;
        

        _watcher = new FileSystemWatcher(_settings.Value.MonitorFilePath, "*.zip")
        {
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite
        };
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _stoppingToken = stoppingToken;

        _logger.LogInformation("AR Processor Service Starting");

        _logger.LogInformation("Information: {Info}", "");

        _watcher.Created += OnNewFileReceived;
        _watcher.EnableRaisingEvents = true;

        await Task.Delay(Timeout.Infinite, stoppingToken);
        return; // Service runs until stopped
  
    }

    
    private async void OnNewFileReceived(object sender, FileSystemEventArgs e)
    {

         Console.WriteLine($"New file detected: {e.FullPath}");
         _logger.LogInformation("New file detected: {FullPath}", e.FullPath);
        // decrypt - unzip - process

        // TODO: Decrypt

        // TODO: Unzip

        // Find csv file and process
       
        using var reader = new StreamReader(@"C:\Temp\ARProcessor_Monitor\TWG_Laserfiche_2025-10-23-03-33-28_Metadata_Email - Copy.csv");
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            MissingFieldFound = null // Ignore missing fields
        });
        var records = csv.GetRecords<InvoiceMetadata>().ToList();

        foreach (var record in records)
        {
            Console.WriteLine($"Processing Invoice: {record.INVOICE_NUMBER}, Amount: {record.INVOICE_AMOUNT}, Invoice Date: {record.INVOICE_DATE}");
            _logger.LogInformation("Processing Invoice: {InvoiceNumber}, Amount: {Amount}, Due Date: {DueDate}", record.INVOICE_NUMBER, record.INVOICE_AMOUNT, record.INVOICE_DATE);
            
            var metadata = new LaserficheMetadata
            {
                // Map fields CSV values to LaserficheMetadata
                InvoiceNumber = record.INVOICE_NUMBER != null ? record.INVOICE_NUMBER : "UnknownInvoiceNumber",
                PONumber = record.PO_NUMBER,
                InvoiceDate = DateTime.TryParse(record.INVOICE_DATE, out var invoiceDate) ? invoiceDate : default,
                CustomerName = record.CUSTOMER_NAME,
                CustomerNumber = record.CUSTOMER_NUMBER,
                CustomerEmail = record.CUSTOMER_EMAIL,
                BarcodeNumber = long.TryParse(record.BARCODE, out var barcode) ? barcode : 0,
                VendorName = record.VENDOR_NAME,
                VendorCode = record.VENDOR_CODE,
                DeclaredRecord = record.DECLARED_RECORD,
                DocumentSource = record.DOCUMENT_SOURCE,
                GLCode = record.GL_CODE,
                GLDate = DateTime.TryParse(record.GL_DATE, out var glDate) ? glDate : default,
                ProcessedDate = DateTime.TryParse(record.PROCESSED_DATE, out var processedDate) ? processedDate : default,
                TradeIndicator = record.TRADE_INDICATOR,
                TotalNetAmount = record.TOTAL_NET_AMOUNT,
                TotalTaxAmount = record.TOTAL_TAX_AMOUNT,
                FreightCharge = record.FREIGHT_CHARGE,
                CustomerAddress = record.CUSTOMER_ADDRESS,
                HandlingCharge = record.HANDLING_CHARGE,
                VendorAddress = record.VENDOR_ADDRESS,
                VendorGST = record.VENDOR_GST,
                InvoiceAmount = record.INVOICE_AMOUNT

            };

             bool success = await _apiClient.UploadFileAndMetadataToLF(metadata.InvoiceNumber, metadata, _stoppingToken);

        }


    }

    public override void Dispose()
    {
        _watcher?.Dispose();
        base.Dispose();
    }

}
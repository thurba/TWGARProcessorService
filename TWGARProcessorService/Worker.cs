namespace TWGARProcessorService;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using LFApiClient;
public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IOptions<ARProcessorSettings> _settings;
    private readonly ApiClient _apiClient;
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly string _watchPath = string.Empty;
    private FileSystemWatcher _watcher;


    public Worker(ILogger<Worker> logger, IOptions<ARProcessorSettings> options, ApiClient apiClient, IHostApplicationLifetime hostApplicationLifetime)
    {
        _logger = logger;
        _settings = options;
        _apiClient = apiClient;
        _hostApplicationLifetime = hostApplicationLifetime;

        _watcher = new FileSystemWatcher(_settings.Value.MonitorFilePath, "*.*")
        {
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite
        };
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("AR Processor Service Starting");

        _logger.LogInformation("Information: {Info}", "");

        _watcher.Created += OnNewFileReceived;
        _watcher.EnableRaisingEvents = true;

        return; // Service runs until stopped
  
    }

    
    private void OnNewFileReceived(object sender, FileSystemEventArgs e)
    {
        Console.WriteLine($"New file detected: {e.FullPath}");
         _logger.LogInformation("New file detected: {FullPath}", e.FullPath);

    }

    public override void Dispose()
    {
        _watcher?.Dispose();
        base.Dispose();
    }

}
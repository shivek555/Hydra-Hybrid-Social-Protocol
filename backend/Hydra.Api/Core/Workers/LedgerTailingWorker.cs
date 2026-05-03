using System.Text;
using Hydra.Api.Core.Hubs;
using Hydra.Api.Core.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Hydra.Api.Core.Workers;

public class LedgerTailingWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IHubContext<ChatHub> _hubContext;
    private readonly string _ledgerPath;

    public LedgerTailingWorker(IServiceProvider serviceProvider, IHubContext<ChatHub> hubContext, IConfiguration config)
    {
        _serviceProvider = serviceProvider;
        _hubContext = hubContext;
        // Point to the C++ node's ledger file via mounted volume in Docker
        _ledgerPath = config["Blockchain:LedgerPath"] ?? "/node-data/ledger.dat";
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine($"Starting LedgerTailingWorker on {_ledgerPath}");
        long lastPosition = 0;

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (!File.Exists(_ledgerPath))
                {
                    await Task.Delay(5000, stoppingToken);
                    continue;
                }

                using var stream = new FileStream(_ledgerPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                if (stream.Length > lastPosition)
                {
                    stream.Position = lastPosition;
                    using var reader = new StreamReader(stream, Encoding.UTF8);
                    var newContent = await reader.ReadToEndAsync(stoppingToken);
                    lastPosition = stream.Position;

                    await ProcessNewLedgerDataAsync(newContent);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error tailing ledger: {ex.Message}");
            }

            await Task.Delay(2000, stoppingToken); // Poll every 2 seconds
        }
    }

    private async Task ProcessNewLedgerDataAsync(string data)
    {
        var lines = data.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        using var scope = _serviceProvider.CreateScope();
        var postRepo = scope.ServiceProvider.GetRequiredService<IPostRepository>();

        foreach (var line in lines)
        {
            var parts = line.Split('|');
            if (parts.Length >= 4)
            {
                var contentHash = parts[1]; // author|hash|sig|time
                var post = await postRepo.GetByContentHashAsync(contentHash);
                
                if (post != null && post.Status != "Verified")
                {
                    await postRepo.UpdateStatusAsync(contentHash, "Verified");
                    await _hubContext.Clients.All.SendAsync("BroadcastVerified", contentHash);
                    Console.WriteLine($"Verified broadcast: {contentHash}");
                }
            }
        }
    }
}

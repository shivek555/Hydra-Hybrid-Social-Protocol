using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace Hydra.Api.Core.Services;

public class BlockchainRelayService
{
    private readonly IConfiguration _config;
    private readonly string _systemKey;

    public BlockchainRelayService(IConfiguration config)
    {
        _config = config;
        _systemKey = _config["Blockchain:SystemKey"] ?? "HYDRA_SYSTEM_SECRET_KEY";
    }

    public async Task RelayToNodeAsync(string authorWallet, string contentHash, string userSignature, long timestamp)
    {
        var payloadToSign = $"{authorWallet}|{contentHash}|{userSignature}|{timestamp}";
        var systemSignature = GenerateSystemSignature(payloadToSign);
        
        var nodeHost = _config["Blockchain:NodeHost"] ?? "127.0.0.1";
        var binaryPayload = Encoding.UTF8.GetBytes($"{systemSignature}|{payloadToSign}");

        using var client = new TcpClient();
        try
        {
            await client.ConnectAsync(nodeHost, 8333);
            await using var stream = client.GetStream();
            await stream.WriteAsync(binaryPayload, 0, binaryPayload.Length);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to relay to C++ Node: {ex.Message}");
        }
    }

    private string GenerateSystemSignature(string payload)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_systemKey));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        return Convert.ToBase64String(hash);
    }
}

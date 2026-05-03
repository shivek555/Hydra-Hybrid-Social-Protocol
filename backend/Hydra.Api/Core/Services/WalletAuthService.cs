using Nethereum.Signer;

namespace Hydra.Api.Core.Services;

public class WalletAuthService
{
    public bool VerifySignature(string message, string signature, string expectedWalletAddress)
    {
        try
        {
            var signer = new EthereumMessageSigner();
            var recoveredAddress = signer.EncodeUTF8AndEcRecover(message, signature);
            return recoveredAddress.Equals(expectedWalletAddress, StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return false;
        }
    }
}

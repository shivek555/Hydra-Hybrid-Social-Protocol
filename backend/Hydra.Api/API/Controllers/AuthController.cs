using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Hydra.Api.Core.Entities;
using Hydra.Api.Core.Interfaces;
using Hydra.Api.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Hydra.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly WalletAuthService _walletAuthService;
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;

    public AuthController(WalletAuthService walletAuthService, IUserRepository userRepository, IConfiguration configuration)
    {
        _walletAuthService = walletAuthService;
        _userRepository = userRepository;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var isValid = _walletAuthService.VerifySignature(request.Message, request.Signature, request.WalletAddress);
        
        if (!isValid)
        {
            return Unauthorized("Invalid signature.");
        }

        var user = await _userRepository.GetByWalletAddressAsync(request.WalletAddress);
        if (user == null)
        {
            user = new User
            {
                WalletAddress = request.WalletAddress
            };
            await _userRepository.CreateUserAsync(user);
        }

        var token = GenerateJwtToken(user);
        return Ok(new { Token = token });
    }

    private string GenerateJwtToken(User user)
    {
        var secretKey = _configuration["JwtSettings:SecretKey"] ?? "HydraSuperSecretKey1234567890!@#$HydraSuperSecretKey1234567890!@#$";
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim("WalletAddress", user.WalletAddress)
        };

        var token = new JwtSecurityToken(
            issuer: "Hydra.Api",
            audience: "Hydra.Web",
            claims: claims,
            expires: DateTime.Now.AddHours(2),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public class LoginRequest
{
    public string WalletAddress { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Signature { get; set; } = string.Empty;
}

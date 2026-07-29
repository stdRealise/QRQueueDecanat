using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace QRQueueDecanat.Authentication;

public class JwtTokenService
{
    private readonly JwtOptions _options;

    public JwtTokenService(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public (string AccessToken, DateTime ExpiresAtUtc) CreateToken(
        Guid userId, string roleName)
    {
        var createdAtUtc = DateTime.UtcNow;
        var expiresAtUtc = createdAtUtc.AddMinutes(
            _options.AccessTokenMinutes);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Role, roleName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_options.Key));
        var credentials = new SigningCredentials(
            securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: createdAtUtc,
            expires: expiresAtUtc,
            signingCredentials: credentials
        );
        var accessToken = new JwtSecurityTokenHandler()
                .WriteToken(token);
        return (accessToken, expiresAtUtc);
    }
}
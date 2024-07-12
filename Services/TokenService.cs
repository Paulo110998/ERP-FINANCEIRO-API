using ERP_Financeiro_API.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ERP_Financeiro_API.Services;

public class TokenService
{
    private IConfiguration _configuration;
    private TokenValidationResult _tokenValidationResult;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(Usuario usuario)
    {

        if (usuario == null)
        {
            throw new ArgumentNullException(nameof(usuario), "Usuario cannot be null");
        }

        Claim[] claims = new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim("loginTimestamp", DateTime.UtcNow.ToString())
        };

        // Gerando chave
        var key = new SymmetricSecurityKey
            (Encoding.UTF8.GetBytes(_configuration
            ["SymmetricSecurityKey_ApiFinanceiro"]));

        // Gerando credenciais a partir da chave
        var signingCredentials = new SigningCredentials(key,
         SecurityAlgorithms.HmacSha256);

        // Gerando token
        var token = new JwtSecurityToken(

            // Tempo de expiração
            expires: DateTime.Now.AddDays(1),

            // Reeinvidicações (claims)
            claims: claims,

            // Credenciais
            signingCredentials: signingCredentials
            );

        // Retornando/convertendo o token
        return new JwtSecurityTokenHandler().WriteToken(token);
    }



}


using Microsoft.IdentityModel.Tokens;
using QuizBytes2.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace QuizBytes2.Service;

public class JwtTokenCreation : IJwtTokenCreation
{
    public string CreateToken(User user, IConfiguration configuration)
    {
        List<Claim> claims = new List<Claim>
        {
           new Claim(ClaimTypes.Name, user.Username),
           new Claim("Id", user.Id),
           new Claim("SpendablePoints", user.SpendablePoints.ToString())
        };

        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(configuration.GetSection("JwtToken:Key").Value));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var token = new JwtSecurityToken(
            claims: claims,

            expires: DateTime.Now.AddMinutes(15),
            issuer: configuration.GetSection("JwtToken:Issuer").Value,
            audience: configuration.GetSection("JwtToken:Audience").Value,
            signingCredentials: credentials);

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return jwt;
    }
}

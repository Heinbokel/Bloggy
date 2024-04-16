using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Bloggy.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;

namespace Bloggy.Services;

/// <summary>
/// Manages logic regarding JWT's.
/// </summary>
public class JwtService {

    private readonly IConfiguration Configuration;

    /// <summary>
    /// Constructor for dependency injection.
    /// </summary>
    /// <param name="configuration">The IConfiguration to provide for this class.</param>
    public JwtService(IConfiguration configuration) {
        this.Configuration = configuration;
    }

    /// <summary>
    /// Generates a JWT as a string from the given user.
    /// </summary>
    /// <param name="user">The user to generate a JWT from.</param>
    /// <returns>The JWT as a string to return.</returns>
    public string GenerateJwtSecurityTokenFromUser(User user) {
        List<Claim> jwtClaims = [
            new Claim("dateOfBirth", user.DateOfBirth.ToString("yyyy-MM-dd")),
            new Claim("email", user.Email),
            new Claim("userRoleId", user.UserRole.Id.ToString()),
            new Claim("firstName", user.FirstName),
            new Claim("lastName", user.LastName),
            new Claim("userName", user.UserName),
            new Claim("id", user.Id.ToString()),
        ];

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: Configuration["Jwt:Issuer"],
            audience: Configuration["Jwt:Audience"],
            claims: jwtClaims,
            expires: DateTime.Now.AddHours(8), // Token expires in 8 hours, must be refreshed or reobtained afterwards.
            signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
    }

}
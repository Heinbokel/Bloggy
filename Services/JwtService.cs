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
public class JwtService
{

    private readonly IConfiguration Configuration;

    /// <summary>
    /// Constructor for dependency injection.
    /// </summary>
    /// <param name="configuration">The IConfiguration to provide for this class.</param>
    public JwtService(IConfiguration configuration)
    {
        this.Configuration = configuration;
    }

    /// <summary>
    /// Generates a JWT as a string from the given user.
    /// </summary>
    /// <param name="user">The user to generate a JWT from.</param>
    /// <returns>The JWT as a string to return.</returns>
    public string GenerateJwtSecurityTokenFromUser(User user)
    {
        // Each claim represents a piece of information about the user that will be encoded into the JWT token.
        // The Claim class comes from the System.Security package.
        List<Claim> jwtClaims = [
            new Claim("dateOfBirth", user.DateOfBirth.ToString("yyyy-MM-dd")),
            new Claim("email", user.Email),
            new Claim("userRoleId", user.UserRole.Id.ToString()),
            new Claim("firstName", user.FirstName),
            new Claim("lastName", user.LastName),
            new Claim("userName", user.UserName),
            new Claim("id", user.Id.ToString()),
        ];

        /* The secret key is used to sign the JWT token to ensure its integrity.
         * SymmetricSecurityKey: This class represents a symmetric security key,
         * meaning the same key is used for both encryption and decryption.
         * This is just the way to "sign" the token with our secret key, defined in
         * appsettings.Development.json or appsettings.json when deployed to the internet.
         */
        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]));
        SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        /*
        * Create a new instance of JwtSecurityToken with the specified issuer, audience, claims, expiration time, and signing credentials.
        * The issuer and audience identify the parties that are intended to participate in the token exchange.
        * The claims represent the user's information encoded into the token.
        * The expiration time determines how long the token will be valid before it expires.
        * The signing credentials are used to sign the token to ensure its authenticity.
        */
        JwtSecurityToken token = new JwtSecurityToken(
            issuer: Configuration["Jwt:Issuer"],
            audience: Configuration["Jwt:Audience"],
            claims: jwtClaims,
            expires: DateTime.Now.AddHours(8), // Token expires in 8 hours, must be refreshed or reobtained afterwards.
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}
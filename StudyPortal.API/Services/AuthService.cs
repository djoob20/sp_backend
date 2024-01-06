using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StudyPortal.API.Configs;
using StudyPortal.API.Models;

namespace StudyPortal.API.Services;

public interface IAuthService
{
    public static readonly int NORMAL_ACCOUNT = 1;
    public static readonly int GOOGLE_ACCOUNT = 2;
    public static readonly int FB_ACCOUNT = 3;
    public static readonly int TWITTER_ACCOUNT = 4;
    public dynamic JwtGenerator(AuthUser user, List<AuthUser> authUsers);
    bool CheckPassword(string modelPassword, AuthUser authUser);

    public string GetGoogleClientId();
}
public class AuthService : AbstractService, IAuthService
{
    private readonly IOptions<StudyPortalDatabaseSettings> _settings;
    public AuthService(IOptions<StudyPortalDatabaseSettings> settings) : base(settings)
    {
        _settings = settings;
        
    }

    public string GetGoogleClientId()
    {
        return _settings.Value.GoogleClientId;
    }


    public dynamic JwtGenerator(AuthUser user, List<AuthUser> authUsers)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_settings.Value.GoogleSecret);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("id", user.Firstname),
                new Claim(ClaimTypes.Role, user.Role)
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var encryptedToken = tokenHandler.WriteToken(token);


        SetJwt(encryptedToken);

        var refreshToken = GenerateRefreshToken();

        SetRefreshToken(refreshToken, user, authUsers);

        return new
        {
            firstname = user.Firstname,
            lastname = user.Lastname,
            token = encryptedToken,
        };
    }

    private RefreshToken GenerateRefreshToken()
    {
        var refreshToken = new RefreshToken
        {
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            Expires = DateTime.Now.AddDays(7),
            Created = DateTime.Now
        };

        return refreshToken;
    }
    
    public bool CheckPassword(string password, AuthUser user)
    {
        bool result;

        using (var hmac = new HMACSHA512(user.PasswordSalt))
        {
            var compute = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            result = compute.SequenceEqual(user.PasswordHash);
        }

        return result;
    }
    
    private void SetJwt(string encrypterToken)
    {
        HttpContext context = new DefaultHttpContext();

        context.Response.Cookies.Append("X-Access-Token", encrypterToken,
            new CookieOptions
            {
                Expires = DateTime.Now.AddMinutes(15),
                HttpOnly = true,
                Secure = true,
                IsEssential = true,
                SameSite = SameSiteMode.None
            });
    }
    
    private void SetRefreshToken(RefreshToken refreshToken, AuthUser user, List<AuthUser> authUsers)
    {
        HttpContext context = new DefaultHttpContext();
        context.Response.Cookies.Append("X-Refresh-Token", refreshToken.Token,
            new CookieOptions
            {
                Expires = refreshToken.Expires,
                HttpOnly = true,
                Secure = true,
                IsEssential = true,
                SameSite = SameSiteMode.None
            });

        var authUser = authUsers.First(x => x.Email == user.Email);
        authUser.Token = refreshToken.Token;
        authUser.TokenCreated = refreshToken.Created;
        authUser.TokenExpires = refreshToken.Expires;
    }
}
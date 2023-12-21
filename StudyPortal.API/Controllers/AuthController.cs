using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StudyPortal.API.Configs;
using StudyPortal.API.Models;
using StudyPortal.API.Services;

namespace StudyPortal.API.Controllers;

/// <summary>
///     Controller for Authenticating with social user account.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private static readonly List<AuthUser> AuthUsers = new();

    private readonly IOptions<StudyPortalDatabaseSettings> _settings;
    private readonly List<User> _users;
    private readonly IUserService _userService;

    public AuthController(IOptions<StudyPortalDatabaseSettings> settings, IUserService userService)
    {
        _settings = settings;
        _userService = userService;
        _users = _userService.GetAsync().Result;
    }

    [HttpPost(template: "Login", Name = "Login")]
    public IActionResult Login([FromBody] Login model)
    {
        var user = AuthUsers.Where(x => x.Firstname == model.UserName).FirstOrDefault();

        if (user == null) return BadRequest("Username Or password was invalid");

        var match = CheckPassword(model.Password, user);

        if (!match) return BadRequest("Username Or Password Was Invalid");

        JwtGenerator(user);

        return Ok();
    }

    private dynamic JwtGenerator(AuthUser user)
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
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var encryptedToken = tokenHandler.WriteToken(token);


        SetJwt(encryptedToken);

        var refreshToken = GenerateRefreshToken();

        SetRefreshToken(refreshToken, user);

        return new
        {
            firstname = user.Firstname,
            lastname = user.Lastname,
            token = encryptedToken, 
            
        };
    }

    private static RefreshToken GenerateRefreshToken()
    {
        var refreshToken = new RefreshToken
        {
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            Expires = DateTime.Now.AddDays(7),
            Created = DateTime.Now
        };

        return refreshToken;
    }

    [HttpGet("RefreshToken")]
    public async Task<ActionResult<string>> RefreshToken()
    {
        var refreshToken = Request.Cookies["X-Refresh-Token"];

        var user = AuthUsers.Where(x => x.Token == refreshToken).FirstOrDefault();

        if (user == null || user.TokenExpires < DateTime.Now) return Unauthorized("Token has expired");

        JwtGenerator(user);

        return Ok();
    }

    protected void SetRefreshToken(RefreshToken refreshToken, AuthUser user)
    {
        HttpContext.Response.Cookies.Append("X-Refresh-Token", refreshToken.Token,
            new CookieOptions
            {
                Expires = refreshToken.Expires,
                HttpOnly = true,
                Secure = true,
                IsEssential = true,
                SameSite = SameSiteMode.None
            });

        var authUser = AuthUsers.First(x => x.Email == user.Email);
        authUser.Token = refreshToken.Token;
        authUser.TokenCreated = refreshToken.Created;
        authUser.TokenExpires = refreshToken.Expires;
    }

    private void SetJwt(string encrypterToken)
    {
        HttpContext.Response.Cookies.Append("X-Access-Token", encrypterToken,
            new CookieOptions
            {
                Expires = DateTime.Now.AddMinutes(15),
                HttpOnly = true,
                Secure = true,
                IsEssential = true,
                SameSite = SameSiteMode.None
            });
    }

    [HttpDelete("RevokeToken/{username}")]
    public async Task<IActionResult> RevokeToken(string username)
    {
        AuthUsers.First(x => x.Firstname == username).Token = "";

        return Ok();
    }


    [HttpPost("LoginWithGoogle")]
    public async Task<IActionResult> LoginWithGoogle([FromBody] string credential)
    {
        var settings = new GoogleJsonWebSignature.ValidationSettings
        {
            Audience = new List<string> { _settings.Value.GoogleClientId }
        };

        var payload = await GoogleJsonWebSignature.ValidateAsync(credential, settings);

        var authUser = AuthUsers.FirstOrDefault(x => x.Email == payload.Email);

        if (authUser == null)
        {
            authUser = new AuthUser
            {
                Firstname = payload.GivenName,
                Lastname = payload.FamilyName,
                Role = "user"
            };
            AuthUsers.Add(authUser);
        }

        var result = _users.Where(u => u.Email == payload.Email && u.Password == payload.JwtId);
        
        if (!result.Any())
        {
            var user = new User
            {
                Firstname = payload.GivenName,
                Lastname = payload.FamilyName,
                Email = payload.Email,
                Password = payload.JwtId,
                Role = "user"
            };

            await _userService.CreateAsync(user);
        }


        var token = JwtGenerator(authUser);


        return token.Equals("") ? BadRequest() : Ok(token);
    }

    private static bool CheckPassword(string password, AuthUser user)
    {
        bool result;

        using (var hmac = new HMACSHA512(user.PasswordSalt))
        {
            var compute = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            result = compute.SequenceEqual(user.PasswordHash);
        }

        return result;
    }

    [HttpPost("Register", Name = "RegisterNewUser")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] Register model)
    {
        var result = _users.Where(u => u.Email == model.Email);
        if (result.Any())
        {
            return BadRequest("User already exist!");
        }
        
        if (model.ConfirmPassword != model.Password)
        {
            return BadRequest("Passwords Dont Match");
        }
        
        //Create new user
        var user = new User
        {
            Firstname = model.Firstname,
            Lastname = model.Lastname,
            Email = model.Email,
            Password = model.Password,
            Role = "utilisateur"
        };
        
        //Add user into  the  database.
        await _userService.CreateAsync(user);
        _users.Add(user);
        
        //Create authenticated user
        var authUser = new AuthUser
        {
            Firstname = model.Firstname,
            Lastname = model.Lastname,
            Role = model.Role
        };
        
        using (var hmac = new HMACSHA512())
        {
            authUser.PasswordSalt = hmac.Key;
            authUser.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(model.Password));
        }
        AuthUsers.Add(authUser);
        
        var token = JwtGenerator(authUser);
               
        return token.Equals("") ? BadRequest() : Ok(token);
    }
}
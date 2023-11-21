using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using StudyPortal.API.Configs;
using StudyPortal.API.Models;
using StudyPortal.API.Services;

namespace StudyPortal.API.Controllers;

/// <summary>
/// Controller for Authenticating with social user account.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private static List<AuthUser> userList = new List<AuthUser>();

    private readonly IOptions<StudyPortalDatabaseSettings> _settings;
    private readonly IUserService _userService;
    private readonly List<User> _users;

    public AuthController(IOptions<StudyPortalDatabaseSettings> settings, IUserService userService)
    {
        _settings = settings;
        _userService = userService;
        _users = _userService.GetAsync().Result;
    }

    [HttpPost(Name = "Login")]
    public IActionResult Login([FromBody] Login model)
    {
        var user = userList.Where(x => x.UserName == model.UserName).FirstOrDefault();

        if (user == null)
        {
            return BadRequest("Username Or Password Was Invalid");
        }

        var match = CheckPassword(model.Password, user);

        if (!match)
        {
            return BadRequest("Username Or Password Was Invalid");
        }

        JwtGenerator(user);

        return Ok();
    }

    protected dynamic JwtGenerator(AuthUser user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(this._settings.Value.GoogleSecret);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("id", user.UserName), new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.DateOfBirth, user.BirthDay)
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var encrypterToken = tokenHandler.WriteToken(token);


        SetJWT(encrypterToken);

        var refreshToken = GenerateRefreshToken();

        SetRefreshToken(refreshToken, user);

        return new { token = encrypterToken, username = user.UserName };
    }

    private RefreshToken GenerateRefreshToken()
    {
        var refreshToken = new RefreshToken()
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

        var user = userList.Where(x => x.Token == refreshToken).FirstOrDefault();

        if (user == null || user.TokenExpires < DateTime.Now)
        {
            return Unauthorized("Token has expired");
        }

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

        userList.Where(x => x.UserName == user.UserName).First().Token = refreshToken.Token;
        userList.Where(x => x.UserName == user.UserName).First().TokenCreated = refreshToken.Created;
        userList.Where(x => x.UserName == user.UserName).First().TokenExpires = refreshToken.Expires;
    }

    protected void SetJWT(string encrypterToken)
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
        userList.Where(x => x.UserName == username).First().Token = "";

        return Ok();
    }


    [HttpPost("LoginWithGoogle")]
    public async Task<IActionResult> LoginWithGoogle([FromBody] string credential)
    {
        var settings = new GoogleJsonWebSignature.ValidationSettings()
        {
            Audience = new List<string> { _settings.Value.GoogleClientId }
        };

        var payload = await GoogleJsonWebSignature.ValidateAsync(credential, settings);

        var authUser = userList.Where(x => x.UserName == payload.Name).FirstOrDefault();

        if (authUser == null)
        {
            authUser = new AuthUser { UserName = payload.Name, Role = "Admin", BirthDay = "01/01/1900" };
            userList.Add(authUser);
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
                Role = "unknown"
            };

            await _userService.CreateAsync(user);
        }


        var token = JwtGenerator(authUser);


        return token.Equals("") ? BadRequest():  Ok(token) ;
    }

    private bool CheckPassword(string password, AuthUser user)
    {
        bool result;

        using (HMACSHA512? hmac = new HMACSHA512(user.PasswordSalt))
        {
            var compute = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            result = compute.SequenceEqual(user.PasswordHash);
        }

        return result;
    }

    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] Register model)
    {
        var authUser = new AuthUser { UserName = model.Firstname, Role = model.Role, BirthDay = model.BirthDay };

        if (model.ConfirmPassword == model.Password)
        {
            using (HMACSHA512? hmac = new HMACSHA512())
            {
                authUser.PasswordSalt = hmac.Key;
                authUser.PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(model.Password));
            }
            var result = _users.Where(u => u.Email == model.Email);
            if (!result.Any())
            {
                var user = new User
                {
                    Firstname = model.Firstname,
                    Lastname = model.Lastname,
                    Email = model.Email,
                    Password = model.Password,
                    Role = "user"
                };

                await _userService.CreateAsync(user);
                
                userList.Add(authUser);

                return Ok(user);
            }
            else
            {
                return BadRequest("User already exist");
            }
        }
        else
        {
            return BadRequest("Passwords Dont Match");
        }

    
    }
}
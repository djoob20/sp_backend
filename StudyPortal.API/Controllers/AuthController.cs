using System.Security.Cryptography;
using System.Text;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;

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


    private readonly List<User> _users;
    private readonly IUserService _userService;
    private readonly IAuthService _authService;

    public AuthController(IUserService userService, IAuthService authService)
    {

        _userService = userService;
        _authService = authService;
        
        _users = _userService.GetAsync().Result;
    }

    [HttpPost(template: "Login", Name = "Login")]
    public IActionResult Login([FromBody] Login model)
    {
        try
        {
            var user = _users.FirstOrDefault(x => x.Email == model.Email);

            if (user == null)
            {
                return BadRequest("User dos not exist!");
            }

            var authUser = AuthUsers.FirstOrDefault(x => x.Email == model.Email);

            if (authUser == null)
            {
                authUser = new AuthUser
                {
                    Firstname = user.Firstname,
                    Lastname = user.Lastname,
                    Email = model.Email,
                    Role = user.Role
                };

                using (var hmac = new HMACSHA512())
                {
                    authUser.PasswordSalt = hmac.Key;
                    authUser.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(model.Password));
                }
                
                AuthUsers.Add(authUser);
            }

            var match = _authService.CheckPassword(model.Password, authUser);

            if (!match)
            {
                return BadRequest("Email Or Password was Invalid");
            }

            var token = _authService.JwtGenerator(authUser, AuthUsers);

            return token.Equals("") ? BadRequest() : Ok(token);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                "Error retrieving data from the database");
        }
    }

    
    [HttpGet("RefreshToken")]
    public async Task<ActionResult<string>> RefreshToken()
    {
        var refreshToken = Request.Cookies["X-Refresh-Token"];

        var user = AuthUsers.FirstOrDefault(x => x.Token == refreshToken);

        if (user == null || user.TokenExpires < DateTime.Now)
        {
            return Unauthorized("Token has expired");
        }

        _authService.JwtGenerator(user, AuthUsers);

        return Ok();
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
        try
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new List<string> { _authService.GetGoogleClientId() }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(credential, settings);

            var authUser = AuthUsers.FirstOrDefault(x => x.Email == payload.Email);

            if (authUser == null)
            {
                authUser = new AuthUser
                {
                    Firstname = payload.GivenName,
                    Lastname = payload.FamilyName,
                    Email = payload.Email,
                    Role = "utilisateur",
                    TokenCreated = DateTime.Now
                };
                AuthUsers.Add(authUser);
            }

            var result = _users.Where(u => u.Email == payload.Email);

            if (!result.Any())
            {
                var user = new User
                {
                    Firstname = payload.GivenName,
                    Lastname = payload.FamilyName,
                    Email = payload.Email,
                    Password = payload.JwtId,
                    Role = "utilisateur",
                    AccountType = IAuthService.GOOGLE_ACCOUNT
                };
                

                await _userService.CreateAsync(user);
                _users.Add(user);
            }


            var token = _authService.JwtGenerator(authUser, AuthUsers);
            
            return token.Equals("") ? BadRequest() : Ok(token);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                "Error retrieving data from the database");
        }
    }

    [HttpPost("Register", Name = "RegisterNewUser")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] Register model)
    {
        try
        {
            if (model.ConfirmPassword != model.Password)
            {
                return BadRequest("Passwords Dont Match");
            }

            var result = _users.FirstOrDefault(u => u.Email == model.Email);
            
            if (result != null)
            {
                if (result.AccountType != IAuthService.NORMAL_ACCOUNT)
                {
                    result.AccountType = IAuthService.NORMAL_ACCOUNT;
                    await _userService.UpdateAsync(result.Id, result);
                }
            }
            else
            {
                var user = _userService.CreateNewUser(model, IAuthService.NORMAL_ACCOUNT);

                //Add user into  the  database.
                await _userService.CreateAsync(user);
                //Add user into the current list of user
                _users.Add(user);
            }

            var authUser = AuthUsers.FirstOrDefault(au => au.Email == model.Email);

            if (authUser == null)
            {
                //Create authenticated user
                authUser = new AuthUser
                {
                    Firstname = model.Firstname,
                    Lastname = model.Lastname,
                    Email = model.Email,
                    Role = model.Role
                };
                using (var hmac = new HMACSHA512())
                {
                    authUser.PasswordSalt = hmac.Key;
                    authUser.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(model.Password));
                }

                AuthUsers.Add(authUser);
                
            }

            var token = _authService.JwtGenerator(authUser, AuthUsers);

            return token.Equals("") ? BadRequest() : Ok(token);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                "Error retrieving data from the database");
        }
    }

   
}
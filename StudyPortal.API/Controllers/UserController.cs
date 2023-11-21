using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyPortal.API.Configs;
using StudyPortal.API.Models;
using StudyPortal.API.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StudyPortal.API.Controllers
{
    [Route( "api/[controller]" )]
    [ApiController]
    [Produces( MediaTypeNames.Application.Json )]
    [Consumes( MediaTypeNames.Application.Json )]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        //private readonly JwtAuthentificationManager _jwtAuthentificationManager;

        public UserController(IUserService userService)
        {
            _userService = userService;
            //_jwtAuthentificationManager = jwtAuthentificationManager;
        }

        /// <summary>
        /// Returns all users.
        /// </summary>
        /// <returns></returns>
        ///[Authorize]
        [HttpGet( Name = "GetAllUsers" )]
        [ProducesResponseType( StatusCodes.Status200OK )]
        [ProducesResponseType( StatusCodes.Status404NotFound )]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAsync();
                if (users.Any())
                {
                    return Ok( users );
                }

                return NotFound();

            }
            catch (Exception)
            {
                return StatusCode( StatusCodes.Status500InternalServerError,
                 "Error retrieving data from the database" );
            }
            //return null;
        }

        /// <summary>
        /// Returns user for the given Id.
        /// </summary>
        /// <returns></returns>
        [HttpGet( "{id}", Name = "GetUserById" )]
        [ProducesResponseType( StatusCodes.Status200OK )]
        [ProducesResponseType( StatusCodes.Status404NotFound )]
        public async Task<IActionResult> GetUserById(string id)
        {
            try
            {
                var user = await _userService.GetAsync( id );
                if (user != null)
                {
                    return Ok( user );
                }

                return NotFound( $"User with Id = {id} not found" );
            }
            catch (Exception)
            {
                return StatusCode( StatusCodes.Status500InternalServerError,
                "Error retrieving data from the database" );
            }
        }

        /// <summary>
        /// Add a new course.
        /// </summary>
        /// <param name="newCourse"></param>
        /// <returns></returns>
        [HttpPost( Name = "CreateNewUser" )]
        [ProducesResponseType( StatusCodes.Status201Created )]
        [ProducesResponseType( StatusCodes.Status404NotFound )]
        public async Task<IActionResult> CreateUser([FromBody] User newUser)
        {
            try
            {
                if (newUser == null)
                {
                    return BadRequest();
                }

                await _userService.CreateAsync( newUser );

                return CreatedAtAction( nameof( GetAllUsers), new { id = newUser.Id }, newUser);


            }
            catch (Exception)
            {
                return StatusCode( StatusCodes.Status500InternalServerError,
                   "Error creating user" );
            }

        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        //[AllowAnonymous]
        //[HttpPost( "Authorize", Name = "AuthorizeActions" )]
        //public IActionResult AuthUser([FromBody] User user)
        //{
        //    var token = _jwtAuthentificationManager.Authenticate( user.Firstname, user.Password );
        //    if (token == null)
        //    {
        //        return Unauthorized();
        //    }

        //    return Ok( token );
        //}
    }
}


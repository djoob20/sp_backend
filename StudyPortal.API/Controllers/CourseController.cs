
using Microsoft.AspNetCore.Mvc;
using StudyPortal.API.Services;
using StudyPortal.API.Models;

using System.Net.Mime;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace StudyPortal.API.Controllers
{
    /// <summary>
    /// Controller for managing actions applying for course.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public class CourseController : ControllerBase
    {
        private readonly ISubjectsService<Course> _courseService;

        public CourseController(ISubjectsService<Course> courseService)
        {
            _courseService = courseService;
        }

        /// <summary>
        /// Returns all course.
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet( Name = "GetAllCourses" )]
        [ProducesResponseType( StatusCodes.Status200OK )]
        [ProducesResponseType( StatusCodes.Status404NotFound )]
        public async Task<IActionResult> GetAllCourses()
        {
            try
            {
                var courses = await _courseService.GetAsync();
                if (courses.Any())
                {
                    return Ok( courses );

                }
                return NotFound();
            }
            catch (Exception)
            {
                return StatusCode( StatusCodes.Status500InternalServerError,
                  "Error retrieving data from the database" );
            }
        }


        /// <summary>
        /// Returns a course for a given Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:length(24)}", Name ="GetCourseById")]
        [ProducesResponseType( StatusCodes.Status200OK )]
        [ProducesResponseType( StatusCodes.Status404NotFound )]
        public async Task<IActionResult> GetCourseById(string id)
        {
            try
            {
                var course = await _courseService.GetAsync( id );

                if (course == null)
                {
                    return NotFound( $"Course with Id = {id} not found" );
                }

                return Ok(course);

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
        [HttpPost("Add", Name = "CreateNewCourse")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateCourse(Course newCourse)
        {
            try
            {
                if (newCourse == null)
                {
                    return BadRequest();
                }
                await _courseService.CreateAsync( newCourse );

                return CreatedAtAction( nameof( GetAllCourses ), new { id = newCourse.Id }, newCourse );
            }
            catch (Exception)
            {
                return StatusCode( StatusCodes.Status500InternalServerError,
                   "Error creating course" );
            }
        }


        /// <summary>
        /// Update an existing course for the given Id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updatedCourse"></param>
        /// <returns></returns>
        [HttpPut( "{id:length(24)}" , Name = "UpdateCourse")]
        [ProducesResponseType( StatusCodes.Status200OK )]
        [ProducesResponseType( StatusCodes.Status204NoContent )]
        [ProducesResponseType( StatusCodes.Status404NotFound )]
        [ProducesResponseType( StatusCodes.Status400BadRequest )]
        public async Task<IActionResult> UpdateCourse(string id, Course updatedCourse)
        {
            try
            {
                if (!id.Equals( updatedCourse.Id ))
                {
                    return BadRequest( "Course ID does mismatch" );
                }

                var courseToUpdate = await _courseService.GetAsync( id );
                if (courseToUpdate == null)
                {
                    return NotFound( $"Course with Id = {id} not found" );
                }

                updatedCourse.Id = courseToUpdate.Id;

                await _courseService.UpdateAsync( id, updatedCourse );

                return NoContent();


            }
            catch (Exception)
            {
                return StatusCode( StatusCodes.Status500InternalServerError,
                    "Error updating data" );
            }
           
        }

        /// <summary>
        /// Delete a course for the given.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete( "{id:length(24)}" , Name = "DeleteCourse")]
        [ProducesResponseType( StatusCodes.Status204NoContent)]
        [ProducesResponseType( StatusCodes.Status404NotFound )]
        public async Task<IActionResult> DeleteCourse(string id)
        {
            try
            {
                var course = await _courseService.GetAsync( id );

                if (course == null)
                {
                    return NotFound( $"Course with Id = {id} not found" );
                }

                await _courseService.DeleteAsync( id );

                return NoContent();

            }
            catch (Exception)
            {
                return StatusCode( StatusCodes.Status500InternalServerError,
                "Error deleting data" );
            }
        }
    }
}

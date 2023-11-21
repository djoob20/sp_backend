using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudyPortal.API.Services;
using StudyPortal.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using StudyPortal.API.Configs;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace StudyPortal.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ArticleController : ControllerBase
    {
        private readonly ISubjectsService<Article> _articleService;

        public ArticleController(ISubjectsService<Article> articleService)
        {
            _articleService = articleService;
        }

        /// <summary>
        /// Returns all articles.
        /// </summary>
        /// <returns>List of articles</returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet(Name = "GetAllArticles" )]
        [ProducesResponseType( StatusCodes.Status200OK )]
        [ProducesResponseType( StatusCodes.Status404NotFound )]
        public async Task <IActionResult> GetAllArticles()
        {
            try
            {
                var articles = await _articleService.GetAsync();
           
                if (articles.Any())
                {
                    return Ok( articles );
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
        /// Returns a article for a given Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet( "{id:length(24)}" , Name = "GetArticleById" )]
        [ProducesResponseType( StatusCodes.Status200OK )]
        [ProducesResponseType( StatusCodes.Status404NotFound )]
        public async Task<ActionResult<Article>> GetArticleById(string id)
        {
            try
            {
                var article = await _articleService.GetAsync( id );

                if (article == null)
                {
                    return NotFound( $"Article with Id = {id} not found" );
                }

                return Ok( article );

            }
            catch (Exception)
            {
                return StatusCode( StatusCodes.Status500InternalServerError,
                  "Error retrieving data from the database" );
            }
        }

        /// <summary>
        /// Add a new article.
        /// </summary>
        /// <param name="newArticle"></param>
        /// <returns></returns>
        [HttpPost("Add", Name = "CreateNewArticle" )]
        [ProducesResponseType( StatusCodes.Status201Created )]
        [ProducesResponseType( StatusCodes.Status404NotFound )]
        public async Task<IActionResult> CreateArticle (Article newArticle)
        {
            try
            {
                if (newArticle == null)
                {
                    return BadRequest();
                }
                await _articleService.CreateAsync( newArticle );

                return CreatedAtAction( nameof( GetAllArticles), new { id = newArticle.Id }, newArticle );
            }
            catch (Exception)
            {
                return StatusCode( StatusCodes.Status500InternalServerError,
                   "Error creating article" );
            }
        }

        /// <summary>
        /// Update an existing article for the given Id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updatedArticle"></param>
        /// <returns></returns>
        [HttpPut( "{id:length(24)}" , Name = "UpdateArticle" )]
        [ProducesResponseType( StatusCodes.Status200OK )]
        [ProducesResponseType( StatusCodes.Status204NoContent )]
        [ProducesResponseType( StatusCodes.Status404NotFound )]
        public async Task<IActionResult> UpdateArticle(string id, Article updatedArticle)
        {
            try
            {
                if (!id.Equals( updatedArticle.Id ))
                {
                    return BadRequest( "Course ID does mismatch" );
                }

                var articleToUpdate = await _articleService.GetAsync( id );
                if (articleToUpdate == null)
                {
                    return NotFound( $"Course with Id = {id} not found" );
                }

                updatedArticle.Id = articleToUpdate.Id;

                await _articleService.UpdateAsync( id, updatedArticle );

                return NoContent();


            }
            catch (Exception)
            {
                return StatusCode( StatusCodes.Status500InternalServerError,
                    "Error updating data" );
            }
        }

        /// <summary>
        /// Delete a article for the given.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete( "{id:length(24)}" , Name = "DeleteArticle" )]
        [ProducesResponseType( StatusCodes.Status204NoContent )]
        [ProducesResponseType( StatusCodes.Status404NotFound )]
        public async Task<IActionResult> DeleteArticle(string id)
        {
            try
            {
                var article = await _articleService.GetAsync( id );

                if (article == null)
                {
                    return NotFound( $"Course with Id = {id} not found" );
                }

                await _articleService.DeleteAsync( id );

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

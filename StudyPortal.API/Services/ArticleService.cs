using System;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using StudyPortal.API.Models;
using StudyPortal.API.Configs;


namespace StudyPortal.API.Services
{
    public class ArticleService:AbstractService,ISubjectsService<Article>
    {
        private readonly IMongoCollection<Article> _articleCollection;

        public ArticleService(IOptions<StudyPortalDatabaseSettings> studyPortalDatabaseSettings):base(studyPortalDatabaseSettings)
        {

            _articleCollection = mongoDatabase.GetCollection<Article>( studyPortalDatabaseSettings.Value.ArticlesCollectionName );
        }

        //Return all articles
        public async Task<List<Article>> GetAsync() =>
            await _articleCollection.Find( _ => true ).ToListAsync();

        //Get an article with the given Id
        public async Task<Article?> GetAsync(string id) =>
            await _articleCollection.Find( x => x.Id == id ).FirstOrDefaultAsync();


        //Create a new article
        public async Task CreateAsync(Article newArticle) =>
            await _articleCollection.InsertOneAsync( newArticle );


        // Update an existing article
        public async Task UpdateAsync(string id, Article updatedArticle) =>
            await _articleCollection.ReplaceOneAsync( x => x.Id == id, updatedArticle );

        //Remove an existing article
        public async Task DeleteAsync(string id) =>
            await _articleCollection.DeleteOneAsync( x => x.Id == id );

        
    }
}


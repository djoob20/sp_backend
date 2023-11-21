using System;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using StudyPortal.API.Configs;
using StudyPortal.API.Models;

namespace StudyPortal.API.Services
{
    public class AbstractService
    {
        protected MongoClient mongoClient;
        protected IMongoDatabase mongoDatabase;

        public AbstractService(IOptions<StudyPortalDatabaseSettings> settings)
        {
            mongoClient = new MongoClient( settings.Value.ConnectionString );

            mongoDatabase = mongoClient.GetDatabase( settings.Value.DatabaseName );


        }
    }
}


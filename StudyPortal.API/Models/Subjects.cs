using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace StudyPortal.API.Models
{
    public class Subjects
    {
        [BsonId]
        [BsonRepresentation( BsonType.ObjectId )]
        [JsonProperty( Required = Required.DisallowNull )]
        [BsonElement( "id" )]
        public string? Id { get; set; }

        [BsonRequired]
        [JsonProperty( Required = Required.DisallowNull )]
        [BsonElement("title")]
        public string Title { get; set; }

        [BsonElement( "description" )]
        public string Description { get; set; }

        [BsonElement( "summary" )]
        public string Summary { get; set; }

        [BsonElement( "authors" )]
        public string [] Authors { get; set; }

        [BsonElement( "dateAdded" )]
        public DateTime DateAdded { get; set; }

        [BsonElement( "viewCount" )]
        public int ViewCount { get; set; }

        public Subjects(string id,
                        string titel,
                        string description,
                        string summary,
                        string [] authors
                        )
        {
            this.Id = id;
            this.Title = titel;
            this.Description = description;
            this.Summary = summary;
            this.Authors = authors;
            this.DateAdded = new DateTime();
            this.ViewCount = 0;

        }
    }
}


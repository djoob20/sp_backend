using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace StudyPortal.API.Models;

public class Subjects
{
    public Subjects(string id,
        string titel,
        string description,
        string summary,
        string[] authors
    )
    {
        Id = id;
        Title = titel;
        Description = description;
        Summary = summary;
        Authors = authors;
        DateAdded = new DateTime();
        ViewCount = 0;
    }

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [JsonProperty(Required = Required.DisallowNull)]
    [BsonElement("id")]
    public string? Id { get; set; }

    [BsonRequired]
    [JsonProperty(Required = Required.DisallowNull)]
    [BsonElement("title")]
    public string Title { get; set; }

    [BsonElement("description")] public string Description { get; set; }

    [BsonElement("summary")] public string Summary { get; set; }

    [BsonElement("authors")] public string[] Authors { get; set; }

    [BsonElement("dateAdded")] public DateTime DateAdded { get; set; }

    [BsonElement("viewCount")] public int ViewCount { get; set; }
}
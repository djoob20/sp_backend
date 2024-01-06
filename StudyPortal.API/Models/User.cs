using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace StudyPortal.API.Models;

public class User
{
    public User()
    {
    }

    public User(string id,
        string firstname,
        string lastname,
        string email,
        string password,
        string role
    )
    {
        Id = id;
        Firstname = firstname;
        Lastname = lastname;
        Email = email;
        Password = password;
        Role = role;
    }

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [JsonProperty(Required = Required.DisallowNull)]
    [BsonElement("id")]
    public string Id { get; set; }

    [BsonElement("firstname")] public string Firstname { get; set; }

    [BsonElement("lastname")] public string Lastname { get; set; }

    [BsonElement("email")] public string Email { get; set; }

    [BsonElement("password")] public string Password { get; set; }

    [BsonElement("role")] public string Role { get; set; }
    
    [BsonElement("account_type")] public int AccountType { get; set; }
}
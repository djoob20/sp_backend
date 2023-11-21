using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace StudyPortal.API.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation( BsonType.ObjectId )]
        [JsonProperty( Required = Required.DisallowNull )]
        [BsonElement( "id" )]
        public string Id { get; set; }

        [BsonElement( "firstname" )]
        public string Firstname { get; set; }

        [BsonElement( "lastname" )]
        public string Lastname { get; set; }

        [BsonElement( "email" )]
        public string Email { get; set; }

        [BsonElement( "password" )]
        public string Password { get; set; }

        [BsonElement( "role" )]
        public string Role { get; set; }
        

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
            this.Id = id;
            this.Firstname = firstname;
            this.Lastname = lastname;
            this.Email = email;
            this.Password = password;
            this.Role = role;

        }
    }
}


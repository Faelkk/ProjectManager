using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProjectManager.Infrastructure.Mongo
{
    public class UserDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = default!;

        [BsonElement("username")]
        public string Username { get; set; } = default!;

        [BsonElement("email")]
        public string Email { get; set; } = default!;

        [BsonElement("password")]
        public string Password { get; set; } = default!;

        [BsonElement("role")]
        public string Role { get; set; } = "User";
    }
}

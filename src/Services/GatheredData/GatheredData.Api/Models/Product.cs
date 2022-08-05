using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GatheredData.Api.Models;
public class Product
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }


    [BsonElement("ProductName")]
    public string ProductName { get; set; } = null!;
}
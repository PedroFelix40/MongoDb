using ApiMongoDb.Domains;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Text.Json.Serialization;

namespace ApiMongoDb.ViewModel
{
    public class OrderViewModel
    {
       
        public string? Id { get; set; }

        public DateTime Date { get; set; }

        public string? Status { get; set; }

        // Referência aos produtos pedidos
        [BsonElement("productId")]
        public List<string>? ProductId { get; set; }
        [BsonIgnore]
        [JsonIgnore]
        public List<Product>? Products { get; set; }


        // Referência aos clientes
        [BsonElement("clientId")]
        public string? ClientId { get; set; }
        [BsonIgnore]
        [JsonIgnore]
        public Client? Client { get; set; }
    }
}

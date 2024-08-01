using ApiMongoDb.Domains;
using MongoDB.Bson.Serialization.Attributes;

namespace ApiMongoDb.ViewModel
{
    public class OrderViewModelGet
    {
        public string? Id { get; set; }

        public DateTime Date { get; set; }

        public string? Status { get; set; }

        // Referência aos produtos pedidos
        [BsonElement("productId")]
        public List<string>? ProductId { get; set; }
        public List<Product> Products { get; set; }

        // Referência aos clientes
        [BsonElement("clientId")]
        public string? ClientId { get; set; }
        public Client? Client { get; set; }
    }
}

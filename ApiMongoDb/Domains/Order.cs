﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ApiMongoDb.Domains
{
    public class Order
    {
        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("date")]
        public DateTime Date { get; set; }

        [BsonElement("status")]
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

using MongoDB.Bson.Serialization.Attributes;

namespace ApiMongoDb.ViewModel
{
    public class OrderViewModelPut
    {
        public DateTime Date { get; set; }
        public string? Status { get; set; }

    }
}

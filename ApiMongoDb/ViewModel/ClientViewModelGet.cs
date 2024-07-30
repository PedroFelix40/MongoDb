using MongoDB.Bson.Serialization.Attributes;

namespace ApiMongoDb.ViewModel
{
    public class ClientViewModelGet
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Cpf { get; set; }
        public string? Phone { get; set; }
        public string? Adress { get; set; }
    }
}

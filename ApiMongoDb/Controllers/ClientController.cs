using ApiMongoDb.Domains;
using ApiMongoDb.Services;
using ApiMongoDb.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ApiMongoDb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]

    public class ClientController : ControllerBase
    {
        private readonly IMongoCollection<Client> _client;
        private readonly IMongoCollection<User> _user;

        public ClientController(MongoDbService mongoDbService)
        {
            _client = mongoDbService.GetDatabase.GetCollection<Client>("client");
            _user = mongoDbService.GetDatabase.GetCollection<User>("user");
        }

        /*
        [HttpGet]
        public async Task<ActionResult<List<Client>>> Get()
        {
            try
            {
                List<ClientViewModelGet> ClientFull = [];
                var clients = await _client.Find(FilterDefinition<Client>.Empty).ToListAsync();
                var user = await _user.Find(FilterDefinition<User>.Empty).ToListAsync();

                foreach (var c in clients)
                {
                    
                    foreach (var u in user)
                    {

                        ClientViewModelGet clientView = new()
                        {
                            Id = c.Id,
                            Name = u.Name,
                            Cpf = c.Cpf,
                            Phone = c.Phone,
                            Adress = c.Adress

                        };
                    
                        ClientFull.Add(clientView);
                    }
                }


                return ClientFull is not null ? Ok(ClientFull) : NotFound("Não foi encontrado nenhum produto!");
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        */

        [HttpGet]
        public async Task<ActionResult<List<ClientViewModelGet>>> Get()
        {
            try
            {
                // Primeiro, obtenha todos os usuários
                var users = await _user.Find(FilterDefinition<User>.Empty).ToListAsync();

                // Crie um dicionário para mapear IDs de usuários para usuários
                Dictionary<string, User> userMap = users.ToDictionary(u => u.Id.ToString(), u => u);

                // Encontre todos os clientes
                var clients = await _client.Find(FilterDefinition<Client>.Empty).ToListAsync();

                List<ClientViewModelGet> ClientFull = new List<ClientViewModelGet>();

                foreach (var c in clients)
                {
                    // Encontre o usuário correspondente ao cliente
                    if (userMap.TryGetValue(c.UserId.ToString(), out User correspondingUser))
                    {
                        ClientViewModelGet clientView = new()
                        {
                            Id = c.Id,
                            Name = correspondingUser.Name,
                            Cpf = c.Cpf,
                            Phone = c.Phone,
                            Adress = c.Adress
                        };

                        ClientFull.Add(clientView);
                    }
                }

                return ClientFull.Count > 0 ? Ok(ClientFull) : NotFound("Não foi encontrado nenhum cliente!");
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }


        [HttpGet("{idClient}")]
        public async Task<IActionResult> GetByName(string idClient)
        {
            try
            {
                var findObj = Builders<Client>.Filter.Eq(p => p.Id, idClient);
                var client = await _client.Find(findObj).FirstOrDefaultAsync();

                var findUser = Builders<User>.Filter.Eq(u => u.Id, client.UserId);
                var user = await _user.Find(findUser).FirstOrDefaultAsync();

                ClientViewModelGet clientViewModel = new ClientViewModelGet{ 
                    Id = client.Id,
                    Name = user.Name,
                    Cpf = client.Cpf,
                    Phone = client.Phone,
                    Adress = client.Adress
                };



                return clientViewModel is not null ? Ok(clientViewModel) : NotFound("Objeto não encontrado!");

            }
            catch (Exception)
            {

                throw;
            }
        }


        [HttpPost]
        public async Task<ActionResult<Client>> Post(ClientViewModelPost clientViewModel)
        {
            try
            {

                var client = new Client
                {
                    // Mapeie os campos necessários aqui
                    UserId = clientViewModel.userId,
                    Cpf = clientViewModel.Cpf, 
                    Phone = clientViewModel.Phone, 
                    Adress = clientViewModel.Adress
                    // Outros campos...
                };

                // Adiciona o produto ao banco de dados.
                await _client.InsertOneAsync(client);

                // Retorna o produto criado com sucesso.
                return Ok(client);
            }
            catch (Exception)
            {
                // Em caso de erro, retorna um status de resposta apropriado.
                return BadRequest("Ocorreu um erro");
            }
        }

        [HttpDelete("{idClient}")]
        public async Task<IActionResult> Delete(string idClient)
        {
            try
            {
                var findObj = Builders<Client>.Filter.Eq(p => p.Id, idClient);
                var result = await _client.DeleteOneAsync(findObj);

                if (result.DeletedCount > 0)
                {
                    return Ok("Objeto apagado");
                }
                else
                {
                    return NotFound("Produto não encontrado");
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPut("{idClient}")]
        public async Task<IActionResult> Put(string idClient, Client client)
        {
            try
            {
                var findObj = Builders<Client>.Filter.Eq(p => p.Id, idClient);

                if (findObj != null)
                {

                    // Define a atualização
                    var update = Builders<Client>.Update.Set(p => p.Cpf, client.Cpf)
                                                         .Set(p => p.Phone, client.Phone)
                                                         .Set(p => p.Adress, client.Adress);

                    // Aplica a atualização
                    await _client.UpdateOneAsync(findObj, update);

                    return Ok("Objeto atualizado");
                }

                return NotFound("Objeto nao encontrado");
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}

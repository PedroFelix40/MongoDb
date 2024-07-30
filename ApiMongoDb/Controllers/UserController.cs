using ApiMongoDb.Domains;
using ApiMongoDb.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace ApiMongoDb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private readonly IMongoCollection<User> _user;

        //public ProductController(MongoDbService mongoDbService)
        //{
        //  _product = mongoDbService.GetDatabase.GetCollection<Product>("product");
        //}

        public UserController(MongoDbService mongoDbService)
        {
            _user = mongoDbService.GetDatabase.GetCollection<User>("user"); 
        }

        [HttpGet]
        public async Task<ActionResult<List<User>>> Get()
        {
            try
            {
                var users = await _user.Find(FilterDefinition<User>.Empty).ToListAsync();

                return users is not null ? Ok(users) : NotFound("Não foi encontrado nenhum produto!");
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("{idUser}")]
        public async Task<IActionResult> GetByName(string idUser)
        {
            try
            {
                var findObj = Builders<User>.Filter.Eq(p => p.Id, idUser);
                var ObjReturn = await _user.Find(findObj).FirstOrDefaultAsync();

                return ObjReturn is not null ? Ok(ObjReturn) : NotFound("Objeto não encontrado!");

            }
            catch (Exception)
            {

                throw;
            }
        }


        [HttpPost]
        public async Task<ActionResult<User>> Post(User user)
        {
            try
            {

                // Adiciona o produto ao banco de dados.
                await _user.InsertOneAsync(user);

                // Retorna o produto criado com sucesso.
                return Ok(user);
            }
            catch (Exception)
            {
                // Em caso de erro, retorna um status de resposta apropriado.
                return BadRequest("Ocorreu um erro");
            }
        }

        [HttpDelete("{idUser}")]
        public async Task<IActionResult> Delete(string idUser)
        {
            try
            {
                var findObj = Builders<User>.Filter.Eq(p => p.Id, idUser);
                var result = await _user.DeleteOneAsync(findObj);

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

        [HttpPut("{idUser}")]
        public async Task<IActionResult> Put(string idUser, User user)
        {
            try
            {
                var findObj = Builders<User>.Filter.Eq(p => p.Id, idUser);

                if (findObj != null)
                {

                    // Define a atualização
                    var update = Builders<User>.Update.Set(p => p.Name, user.Name)
                                                         .Set(p => p.Email, user.Email)
                                                         .Set(p => p.Password, user.Password);

                    // Aplica a atualização
                    await _user.UpdateOneAsync(findObj, update);

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

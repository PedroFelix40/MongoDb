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
    public class ProductController : ControllerBase
    {
        private readonly IMongoCollection<Product> _product;

        public ProductController(MongoDbService mongoDbService)
        {
            _product = mongoDbService.GetDatabase.GetCollection<Product>("product");
        }

        [HttpGet]
        public async Task<ActionResult<List<Product>>> Get()
        {
            try
            {
                var products = await _product.Find(FilterDefinition<Product>.Empty).ToListAsync();

                return products is not null ? Ok(products) : NotFound("Não foi encontrado nenhum produto!");
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("{nomeProduto}")]
        public async Task<IActionResult> GetByName(string nomeProduto)
        {
            try
            {
                var findObj = Builders<Product>.Filter.Eq(p => p.Name, nomeProduto);
                var ObjReturn = await _product.Find(findObj).FirstOrDefaultAsync();            
                
                return ObjReturn is not null? Ok(ObjReturn) : NotFound("Objeto não encontrado!");

            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        public async Task<ActionResult<Product>> Post(Product product)
        {
            try
            {

                // Adiciona o produto ao banco de dados.
                await _product.InsertOneAsync(product);

                // Retorna o produto criado com sucesso.
                return Ok(product);
            }
            catch (Exception)
            {
                // Em caso de erro, retorna um status de resposta apropriado.
                return BadRequest("Ocorreu um erro");
            }
        }

        [HttpDelete("{nomeProduto}")]
        public async Task<IActionResult> Delete(string nomeProduto)
        {
            try
            {
                var findObj = Builders<Product>.Filter.Eq(p => p.Name, nomeProduto);
                var result = await _product.DeleteOneAsync(findObj);

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

        [HttpPut("{nomeProduto}")]
        public async Task<IActionResult> Put(string nomeProduto, Product product)
        {
            try
            {
                var findObj = Builders<Product>.Filter.Eq(p => p.Name, nomeProduto);

                if (findObj != null)
                {

                    // Define a atualização
                    var update = Builders<Product>.Update.Set(p => p.Name, product.Name)
                                                         .Set(p => p.Price, product.Price);

                    // Aplica a atualização
                    await _product.UpdateOneAsync(findObj, update);

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

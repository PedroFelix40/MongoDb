using ApiMongoDb.Domains;
using ApiMongoDb.Services;
using ApiMongoDb.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Net.Sockets;

namespace ApiMongoDb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class OrderController : ControllerBase
    {
        private readonly IMongoCollection<Order> _order;
        private readonly IMongoCollection<Client> _client;
        private readonly IMongoCollection<User> _user;
        private readonly IMongoCollection<Product> _product;

        public OrderController(MongoDbService mongoDbService)
        {
            _client = mongoDbService.GetDatabase.GetCollection<Client>("client");
            _user = mongoDbService.GetDatabase.GetCollection<User>("user");
            _order = mongoDbService.GetDatabase.GetCollection<Order>("order");
            _product = mongoDbService.GetDatabase.GetCollection<Product>("product");
        }

        [HttpPost]
        public async Task<ActionResult<Order>> Create(OrderViewModel orderViewModel)
        {
            try
            {
                Order order = new Order();

                order.Id = orderViewModel.Id;
                order.Date = orderViewModel.Date;
                order.Status = orderViewModel.Status;
                order.ProductId = orderViewModel.ProductId;
                order.ClientId = orderViewModel.ClientId;

                var client = await _client.Find(x => x.Id == order.ClientId).FirstOrDefaultAsync();

                if (client == null)
                {
                    return NotFound("Cliente não existe!");
                }

                order.Client = client;

                await _order.InsertOneAsync(order);

                return StatusCode(201, order);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<Order>>> Get()
        {
            try
            {
                var orders = await _order.Find(FilterDefinition<Order>.Empty).ToListAsync();

                foreach (var order in orders)
                {
                    if (order.ProductId != null)
                    {
                        var filter = Builders<Product>.Filter.In(p => p.Id, order.ProductId);

                        order.Products = await _product.Find(filter).ToListAsync();
                    }

                    if (order.ClientId != null)
                    {
                        order.Client = await _client.Find(x => x.Id == order.ClientId).FirstOrDefaultAsync();
                    }
                }

                return Ok(orders);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet("idOrder")]
        public async Task<ActionResult<Order>> GetByIdOrder(string idOrder)
        {
            try
            {
                var findOrder = Builders<Order>.Filter.Eq(o => o.Id, idOrder);

                var order = await _order.Find(findOrder).FirstOrDefaultAsync();

                var findProduct = Builders<Product>.Filter.In(p => p.Id, order.ProductId);

                var product = await _product.Find(findProduct).ToListAsync();


                var filter = Builders<Product>.Filter.In(p => p.Id, order.ProductId);

                order.Products = await _product.Find(filter).ToListAsync();




                return Ok(order);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet("idProduct")]
        public async Task<ActionResult<Order>> GetByIdProduct(string idClient)
        {
            try
            {
                var findOrder = Builders<Order>.Filter.Eq(o => o.ClientId, idClient);

                var order = await _order.Find(findOrder).FirstOrDefaultAsync();

                var findProduct = Builders<Product>.Filter.In(p => p.Id, order.ProductId);

                var product = await _product.Find(findProduct).ToListAsync();


                var filter = Builders<Product>.Filter.In(p => p.Id, order.ProductId);

                order.Products = await _product.Find(filter).ToListAsync();




                return Ok(order);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpDelete("idOrder")]
        public async Task<IActionResult> Delete(string idOrder)
        {
            try
            {
                var findObj = Builders<Order>.Filter.Eq(p => p.Id, idOrder);
                var result = await _order.DeleteOneAsync(findObj);

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

        [HttpPut("{idOrder}")]
        public async Task<IActionResult> Put(string idOrder, OrderViewModelPut order)
        {
            try
            {
                var findObj = Builders<Order>.Filter.Eq(p => p.Id, idOrder);

                if (findObj != null)
                {             
                    var update = Builders<Order>.Update.Set(p => p.Status, order.Status)
                                                       .Set(p => p.Date, order.Date);

                    // Aplica a atualização
                    await _order.UpdateOneAsync(findObj, update);

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
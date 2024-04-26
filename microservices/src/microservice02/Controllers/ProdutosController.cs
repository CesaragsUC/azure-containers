using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System.Text.Json;

namespace Service_02.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutosController : Controller
    {
        private readonly IDatabase _redis;
        public ProdutosController(IDistributedCache cache, IConnectionMultiplexer muxer)
        {
            _redis = muxer.GetDatabase();
        }

        [HttpGet]
        [Route("all")]
        public async Task<IActionResult> Index()
        {
            var cacheKey = "Products";
            var _listaProdutos = new List<Produtos>();

            var produtos = new Produtos();
          

            var json = await _redis.StringGetAsync(cacheKey);
            if (!string.IsNullOrEmpty(json))
            {
                _listaProdutos = JsonSerializer.Deserialize<List<Produtos>>(json);
            }
            else
            {
                _listaProdutos = produtos.CriarListaProduto(10);
                json = JsonSerializer.Serialize<List<Produtos>>(_listaProdutos);

                await _redis.StringSetAsync(cacheKey, json, TimeSpan.FromMinutes(2));

            }

            return Ok(_listaProdutos);
        }

        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IActionResult> Details(Guid id)
        {
            // obter produto pelo id do cache produtos

            var cacheKey = "Products";
            var produtos = new Produtos();

            var json = await _redis.StringGetAsync(cacheKey);
            if (!string.IsNullOrEmpty(json))
            {
               var  _listaProdutos = JsonSerializer.Deserialize<List<Produtos>>(json);
                var produto =  _listaProdutos.Find(p => p.Id == id);
                return Ok(produto);
            }


            return BadRequest("Falha na leitura do cache.");
        }

    }
}

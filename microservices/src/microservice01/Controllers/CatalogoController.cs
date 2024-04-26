using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Text.Json;

namespace Service01.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CatalogoController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IDatabase _redis;
        public CatalogoController(HttpClient httpClient, AppServicesSettings settings, IConnectionMultiplexer muxer)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(settings.ProdutosApiUrl);
            _redis = muxer.GetDatabase();
        }


        [HttpGet("all")]
        public async Task<IActionResult> Index()
        {
            var cacheKey = "Products";
            var _listaProdutos = new List<ProdutoDto>();


            var json = await _redis.StringGetAsync(cacheKey);
            if (!string.IsNullOrEmpty(json))
            {
                _listaProdutos = JsonSerializer.Deserialize<List<ProdutoDto>>(json);
            }
            else
            {
                var response = await _httpClient.GetAsync("all");
                if (response.IsSuccessStatusCode)
                {
                    _listaProdutos = await response.Content.ReadFromJsonAsync<List<ProdutoDto>>();

                    json = JsonSerializer.Serialize<List<ProdutoDto>>(_listaProdutos);

                    await _redis.StringSetAsync(cacheKey, json, TimeSpan.FromMinutes(2));
                }

            }

            return Ok(_listaProdutos);

        }

        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IActionResult> Details(Guid id)
        {
            var response = await _httpClient.GetAsync($"{id}");
            if (response.IsSuccessStatusCode)
            {
                var produto = await response.Content.ReadFromJsonAsync<ProdutoDto>();
                return produto is not null ?  Ok(produto) : BadRequest("Produto nao existe.");
            }
            return BadRequest("Falha na requisicao");
        }


    }
}

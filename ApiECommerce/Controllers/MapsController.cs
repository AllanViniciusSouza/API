using Microsoft.AspNetCore.Mvc;
using ApiECommerce.DTOs;

namespace ApiECommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MapsController : ControllerBase
    {
        private readonly GoogleMapsService _mapsService;

        public MapsController(GoogleMapsService mapsService)
        {
            _mapsService = mapsService;
        }

        [HttpGet("calcular")]
        public async Task<ActionResult<DistanciaResult>> CalcularDistancia([FromQuery] string origem, [FromQuery] string destino)
        {
            var resultado = await _mapsService.CalcularDistanciaAsync(origem, destino);

            if (resultado == null)
                return BadRequest("Não foi possível calcular a distância.");

            return Ok(resultado);
        }
    }
}

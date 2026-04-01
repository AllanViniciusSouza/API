using ApiECommerce.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ApiECommerce.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    public class WhatsAppController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public WhatsAppController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] WhatsappMessageDto dto)
        {
            Console.WriteLine($"Recebido DTO: {JsonSerializer.Serialize(dto)}");

            try
            {
                var url = $"http://localhost:3000/client/sendMessage/{dto.Session}";

                var payload = new
                {
                    chatId = dto.Phone,
                    contentType = "string",
                    content = dto.Message
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("x-api-key", "comunidadezdg.com.br");

                var response = await _httpClient.PostAsync(url, content);

                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                    return Ok(responseBody);
                else
                    return StatusCode((int)response.StatusCode, responseBody);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao enviar mensagem: {ex.Message}");
            }
        }
    }
}

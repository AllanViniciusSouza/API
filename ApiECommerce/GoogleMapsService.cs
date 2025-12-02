using ApiECommerce.DTOs;

namespace ApiECommerce
{
    public class GoogleMapsService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey = "AIzaSyDndgjqAzLyXbgbdH1V0dLknyJYLJ974_Q"; // Proteja isso em config depois

        public GoogleMapsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<DistanciaResult?> CalcularDistanciaAsync(string origem, string destino)
        {
            var url = $"https://maps.googleapis.com/maps/api/distancematrix/json?origins={
                Uri.EscapeDataString(origem)}&destinations={Uri.EscapeDataString(destino)}&key={_apiKey}&language=pt-BR&region=br";

            var resposta = await _httpClient.GetFromJsonAsync<DistanceMatrixResponse>(url);
            var raw = await _httpClient.GetStringAsync(url);
            Console.WriteLine(raw);

            var element = resposta?.rows?.FirstOrDefault()?.elements?.FirstOrDefault();

            if (element?.status == "OK")
            {
                // Extrai apenas os números
                int minutos = int.Parse(new string(element.duration.text.Where(char.IsDigit).ToArray()));

                // Soma 30
                minutos += 30;

                // Formata de volta
                var duracao = $"{minutos} minutos";

                return new DistanciaResult
                {
                    Distancia = element.distance.text,
                    Duracao = duracao
                };
            }

            return null;
        }
    }
}

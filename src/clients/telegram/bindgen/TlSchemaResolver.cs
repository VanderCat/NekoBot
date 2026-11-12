using System.Net.Http.Headers;
using System.Text.Json;

namespace NekoBot.Telegram.Bindgen;

public class TlSchemaResolver : IDisposable {
    private HttpClientHandler _clientHandler;
    private HttpClient _client;
    private JsonSerializerOptions _jsonSerializerOptions;
    public TlSchemaResolver(HttpClientHandler client) {
        _clientHandler = client;
        _jsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        _clientHandler.UseCookies = false;
        _client = new HttpClient(_clientHandler);
        _client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("NekoBot.Telegram.Bindgen", "0.1.0"));
    }

    public async ValueTask<TLSchemaModel?> FetchLayerAsync(int layer) {
        var msg = new HttpRequestMessage(HttpMethod.Get, TlSchemaConstants.Schema);
        msg.Headers.Add("Cookie", $"stel_dev_layer={layer}");
        var resp = await _client.SendAsync(msg);
        if (resp.IsSuccessStatusCode) {
            var stream = await resp.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<TLSchemaModel>(stream, _jsonSerializerOptions);
        }
        return null;
    }

    public void Dispose() {
        _client.Dispose();
        _clientHandler.Dispose();
    }
}
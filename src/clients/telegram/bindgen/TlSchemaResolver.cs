// Copyright 2026 VanderCat

// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at

//     http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

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
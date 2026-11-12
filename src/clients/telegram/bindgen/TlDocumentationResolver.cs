using System.Collections.Concurrent;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Markdig.Syntax.Inlines;

namespace NekoBot.Telegram.Bindgen;

public class TlDocumentationResolver {
    private HttpClientHandler _clientHandler;
    private HttpClient _client;
    private JsonSerializerOptions _jsonSerializerOptions;
    public TlDocumentationResolver(HttpClientHandler clientHandler) {
        _clientHandler = clientHandler;
        _jsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        _clientHandler.UseCookies = false;
        _client = new HttpClient(_clientHandler);
        _client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("NekoBot.Telegram.Bindgen", "0.1.0"));
    }
    
    private ConcurrentDictionary<string, TlDocumentationModel?> _cache = new ();

    public ValueTask<TlDocumentationModel?> GetDocumentationForTypeAsync(string typeName, int? layer = null)
        => GetDocumentationAsync(TlSchemaConstants.TypeDoc, typeName, layer);
    public ValueTask<TlDocumentationModel?> GetDocumentationForConstructorAsync(string typeName, int? layer = null)
        => GetDocumentationAsync(TlSchemaConstants.ConstructorDoc, typeName, layer);
    public ValueTask<TlDocumentationModel?> GetDocumentationForMethodAsync(string typeName, int? layer = null)
        => GetDocumentationAsync(TlSchemaConstants.ConstructorDoc, typeName, layer);

    private async ValueTask<TlDocumentationModel?> GetDocumentationAsync(Uri baseurl, string name, int? layer = null) {
        var cacheName = $"{baseurl} {name}"; //TODO: as soon as we support layers for docs this should be versioned.
        if (_cache.TryGetValue(cacheName, out var cachedDocumentation)) {  
            return cachedDocumentation;   
        }
        
        var msg = new HttpRequestMessage(HttpMethod.Get, new Uri(baseurl, "./"+name+".md"));
        var resp = await _client.SendAsync(msg);
        if (resp.IsSuccessStatusCode) {
            var str = await resp.Content.ReadAsStringAsync();
            return _cache[cacheName] = ParseMarkdown(str); 
           // TODO: currently markdown are resolved to the current schema, so documentation
           //       for the parameters will be inaccurate for older stuff.
           //       should parse html instead, since it responses depending on layer set in
           //       cookie, just like json schema.
        }

        return _cache[cacheName] = null;
    }

    private enum DocumentationSection {
        Root,
        Header,
        Constructors,
        Methods,
        Parameters,
        Errors,
        Related
    } 

    private TlDocumentationModel ParseMarkdown(string markdown) {
        var ast = Markdig.Markdown.Parse(markdown);
        using var parser = new TlDocumentationParser(ast);
        return parser.Parse();
    }
    private static string ExtractText(ContainerInline? inlineContainer) {
        if (inlineContainer == null) return string.Empty;

        var sb = new StringBuilder();
        foreach (var inline in inlineContainer.FindDescendants<LiteralInline>())
            sb.Append(inline.Content.ToString());
        
        return sb.ToString().Trim();
    }
}
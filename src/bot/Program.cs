using System.Reflection;

namespace NekoBot;

public class Program {
    public static void Main(string[] args) {
        var adapterLoader = new AdapterLoader();
        var adapters = adapterLoader.Load(SearchPath.FromEnvironment("NEKOBOT_ADAPTER_FOLDERS"));
        IChatApi? api = null;
        foreach (var adapter in adapters) {
            if (adapter.Name == "Telegram") {
                api = adapter;
                break;
            }
        }
        if (api is null) {
            throw new Exception("failed to load telegram adapter");
        }
        Console.WriteLine($"initing {api.Name} adapter");
        api.Init();
    }
}
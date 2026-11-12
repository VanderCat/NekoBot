using System.Reflection;

namespace NekoBot;

public class AdapterLoader {
    
    public List<IChatApi> Load(SearchPath path) {
        var plugins = new List<IChatApi>();
        foreach (var file in path.ListAll()) {
            try {
                var assembly = Assembly.LoadFrom(file);
                var pluginTypes = assembly.GetTypes()
                    .Where(t => typeof(IChatApi).IsAssignableFrom(t) && !t.IsInterface);

                foreach (var type in pluginTypes) {
                    var plugin = (IChatApi)Activator.CreateInstance(type);
                    plugins.Add(plugin);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load plugin {file}: {ex.Message}");
            }
        }
        
        return plugins;
    }
}
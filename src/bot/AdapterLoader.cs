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
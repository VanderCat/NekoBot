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

namespace NekoBot;

public class SearchPath(string searchPath) {
    
    public const char Separator = ';';
    
    public static string Default =>
        string.Join(Separator, Path.Join("!", "plugins", "?.dll"),
            Path.Join(".", "plugins", "?.dll"));
    
    public static SearchPath FromEnvironment(string envVar) {
        var value = Environment.GetEnvironmentVariable(envVar);
        if (string.IsNullOrWhiteSpace(value))
            return new SearchPath(Default);
        var i = value.IndexOf($"{Separator}{Separator}", StringComparison.Ordinal);
        if (i >= 0)
            value = $"{value[..i]}{Separator}{Default}{Separator}{value[(i + 2)..]}";

        return new SearchPath(value);
    }
    public IEnumerable<string> Expand(string moduleName) {
        var exeDir = Path.GetDirectoryName(AppContext.BaseDirectory);
        
        foreach (var template in searchPath.Split(Separator,
                     StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)) {
            var t = template.Replace("!", exeDir);
            yield return t.Replace("?", moduleName);
        }
    }
    
    public string? Find(string moduleName, out IReadOnlyList<string> tried) {
        var list = new List<string>();
        tried = list;

        foreach (var candidate in Expand(moduleName)) {
            try {
                var full = Path.GetFullPath(candidate);
                list.Add(full);
                if (File.Exists(full))
                    return full;
            }
            catch (ArgumentException) {}
        }
        
        return null;
    }

    public IEnumerable<string> ListAll() {
        var exeDir = Path.GetDirectoryName(AppContext.BaseDirectory);

        foreach (var template in searchPath.Split(Separator,
                     StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)) {
            var t = template.Replace("!", exeDir);
            foreach (var str in Directory.EnumerateFiles(Path.GetDirectoryName(t),
                         Path.GetFileName(t).Replace('?', '*'))) {
                yield return str;
            }
        }
    }
}
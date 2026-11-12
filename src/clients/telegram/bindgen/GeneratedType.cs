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

namespace NekoBot.Telegram.Bindgen;

public class GeneratedType {
    public string Name { get; set; }
    public int SinceLayer { get; set; }
    public int UntilLayer { get; set; }
    public string Summary { get; set; }
    public string Remarks { get; set; }

    /// <summary>
    /// A key value list of constructors.
    /// </summary>
    /// <remarks>
    /// Key is constructorName#ID
    /// </remarks>
    public Dictionary<string, GeneratedFunction> Constructors { get; set; } = new();
}
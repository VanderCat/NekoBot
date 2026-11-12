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

public class GeneratedFunction {
    /// <summary>
    /// A name of the constructor/function as seen in schema.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The id of the constructor/function. Used for denoting the type for API.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The layer this constructor/function got introduced in.
    /// </summary>
    public int SinceLayer { get; set; }

    /// <summary>
    /// The layer this constructor/function got removed (changed).
    /// </summary>
    /// <remarks>
    /// 0 if it was never removed.
    /// </remarks>
    public int UntilLayer { get; set; }

    /// <summary>
    /// Constructor/function description. Used for generating documentation.
    /// </summary>
    public string Summary { get; set; }
    
    /// <summary>
    /// Constructor/function remarks. Used for generating documentation.
    /// </summary>
    public string Remarks { get; set; }

    /// <summary>
    /// Parameters for this constructor/function.
    /// </summary>
    public List<GeneratedParameter> Parameters { get; set; } = new();
}
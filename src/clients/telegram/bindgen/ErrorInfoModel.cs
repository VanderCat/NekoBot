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

using System.Net;

namespace NekoBot.Telegram.Bindgen;

public class ErrorInfoModel {
    /// <summary>
    /// All error messages and codes for each method (object).
    /// </summary>
    /// <remarks>
    /// <p>
    /// Keys: Error codes as strings (numeric strings)
    /// </p>
    /// <p>
    /// Values: 
    /// </p>
    /// <ul>
    ///     All error messages for each method (object)
    ///     <li>
    ///     Keys: Error messages (string)
    ///     </li>
    ///     <li>
    ///     Values: An array of methods which may emit this error (array of strings, may be empty for errors that can be emitted by any method)
    ///     </li>
    /// </ul>
    /// </remarks>
    public Dictionary<HttpStatusCode, Dictionary<string, List<string>>> Errors { get; set; }
    /// <summary>
    /// Descriptions for every error mentioned in errors (and a few other errors not related to a specific method) 
    /// </summary>
    public Dictionary<string, string> Descriptions { get; set; }
    /// <summary>
    /// The full list of methods that can only be used by users, not bots.
    /// </summary>
    public List<string> UserOnly { get; set; }
    /// <summary>
    /// The full list of methods that can only be used by bots, not users.
    /// </summary>
    public List<string> BotOnly { get; set; }
    /// <summary>
    /// The full list of methods that can be used by bots over a business connection with invokeWithBusinessConnection.
    /// </summary>
    public List<string> BusinessSupported { get; set; }
    /// <summary>
    /// The full list of methods that can be used by not yet logged in connections.
    /// </summary>
    public List<string> UnauthedAllowed { get; set; }
    /// <summary>
    /// Layer number.
    /// </summary>
    public int Layer { get; set; }
}
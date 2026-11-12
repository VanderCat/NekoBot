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

public class TlDocumentationModel {
    public string Name { get; set; }
    public string Summary { get; set; }
    public string Remarks { get; set; }
    public string Tl { get; set; }
    public Dictionary<string, string>? Parameters { get; set; }
    public Dictionary<string, string>? Constructors { get; set; }
    public Dictionary<string, string>? Methods { get; set; }
    public string Result { get; set; }
    public bool UsersOnly { get; set; }
    public bool BotsOnly { get; set; }
    public bool AuthorizationRequired { get; set; }
    public List<(string code, string type, string description)> PossibleErrors { get; set; }
    public Dictionary<string, string> SeeAlson { get; set; }
}
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

using System.Text;

namespace NekoBot.Telegram.Bindgen;

public abstract class TLSchemaElementModel {
    public int Id { get; set; }
    public List<TLSchemaParamModel> Params { get; set; }
    public string Type { get; set; }
    public override string ToString() {
        var sb = new StringBuilder();
        sb.Append('#');
        sb.Append($"{Id:X}");
        sb.Append(' ');
        foreach (var param in Params) {
            sb.Append(param);
            sb.Append(' ');
        }

        sb.Append("= ");
        sb.Append(Type);
        return sb.ToString();
    }
}
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
using Markdig.Renderers.Roundtrip;
using Markdig.Syntax;

namespace NekoBot.Telegram.Bindgen;

public class TlDocumentationParser(MarkdownDocument document) : IDisposable {
    // Structure of the documentation files with relevant data in angle brackets:
    // # <name>
    // <Summary>
    // <Remarks?> //anything else than the first paragraph of the file should be considered a remark
    // <TlBlock>
    // ### Constructors // Only present on type documentation
    // | Constructor | Description |
    // | --- | --- |
    // | [<constructor>](/constructors/<constructor>.md) | constructor description | // description is the same as summary in docs for the constructor
    // ### Constructors // Only present on type documentation
    // | Method | Description |
    // | --- | --- |
    // | [<method>](/methods/<method>.md) | method description | // description is the same as summary in the docs for the method
    // ### Related Pages
    // #### [<name>](/<pageType>/<name>.md) // only relevant pageTypes are type, methods, constructors, used for see also
    // <descripton of see also>
    
    private IEnumerator<Block> _enumerator = document.GetEnumerator();
    private TlDocumentationModel _docs = new TlDocumentationModel();

    private static string GetText(MarkdownObject? obj) {
        if (obj is null) return "";
        var sb = new StringBuilder();
        var renderer = new RoundtripRenderer(new StringWriter(sb));
        renderer.Render(obj);
        return sb.ToString();
    }
    
    public TlDocumentationModel Parse() {
        _enumerator.MoveNext();
        if (_enumerator.Current is not HeadingBlock nameBlock)
            throw new InvalidDataException("The provided markdown document does not conform to telegram doc style.");
        _docs.Name = GetText(nameBlock.Inline);
        _enumerator.MoveNext();
        if (_enumerator.Current is not ParagraphBlock summaryBlock)
            throw new InvalidDataException("The provided markdown document does not conform to telegram doc style.");
        _docs.Summary = GetText(summaryBlock.Inline);
        _enumerator.MoveNext();
        var remarks = new List<string>();
        while (_enumerator.Current is ParagraphBlock remarksBlock) {
            remarks.Add($"<p>{GetText(remarksBlock.Inline)}</p>");
            _enumerator.MoveNext();
        }
        _docs.Remarks = string.Join('\n', remarks);
        if (_enumerator.Current is CodeBlock codeBlock) {
            _docs.Tl = GetText(codeBlock.Inline);
        }
        return _docs;
    }

    public void Dispose() {
        _enumerator.Dispose();
    }
}
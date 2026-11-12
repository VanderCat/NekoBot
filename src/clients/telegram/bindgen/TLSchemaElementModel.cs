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
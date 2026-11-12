namespace NekoBot.Telegram.Bindgen;

public class TLSchemaMethodModel : TLSchemaElementModel {
    public string Method { get; set; }
    public override string ToString() {
        return $"{Method}{base.ToString()}";
    }
}
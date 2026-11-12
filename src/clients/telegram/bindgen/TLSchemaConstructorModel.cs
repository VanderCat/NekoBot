namespace NekoBot.Telegram.Bindgen;

public class TLSchemaConstructorModel : TLSchemaElementModel {
    public string Predicate { get; set; }
    public override string ToString() {
        return $"{Predicate}{base.ToString()}";
    }
}
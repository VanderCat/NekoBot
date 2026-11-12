namespace NekoBot.Adapter.Telegram.Metadata;

/// <summary>
/// Which layer this api originates from
/// </summary>
/// <param name="layer"></param>
public class LayerAttribute(int layer) : Attribute {
    
}
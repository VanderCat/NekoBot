namespace NekoBot.Adapter.Telegram.Metadata;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor)]
public class TlIdAttribute(int id) : Attribute {
    
}
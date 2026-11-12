namespace NekoBot.Adapter.Telegram.Metadata;

/// <summary>
/// Denotes if this API is usable for bots or users only
/// </summary>
/// <param name="env"></param>
public class TelegramEnvironmentAttribute(string env) : Attribute {
    
}
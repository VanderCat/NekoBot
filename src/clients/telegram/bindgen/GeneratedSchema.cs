namespace NekoBot.Telegram.Bindgen;

public class GeneratedSchema {
    /// <summary>
    /// The schema latest layer. Used to generated deprecations.
    /// </summary>
    public int Layer { get; set; }

    public Dictionary<string, GeneratedType> Types { get; set; } = new();
    public Dictionary<string, GeneratedFunction> Functions { get; set; } = new();
}
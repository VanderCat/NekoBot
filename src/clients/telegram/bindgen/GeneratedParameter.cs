namespace NekoBot.Telegram.Bindgen;

public class GeneratedParameter {
    /// <summary>
    /// Name of the parameter.
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Type of the parameter.
    /// </summary>
    /// <remarks>
    /// Maybe optional, depending on a flag in format flags.bit?ActualType
    /// </remarks>
    public string Type { get; set; }
    /// <summary>
    /// Description of the parameter. Used by generating documentation.
    /// </summary>
    public string Description { get; set; }
}
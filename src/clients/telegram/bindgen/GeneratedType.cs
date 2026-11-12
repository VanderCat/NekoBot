namespace NekoBot.Telegram.Bindgen;

public class GeneratedType {
    public string Name { get; set; }
    public int SinceLayer { get; set; }
    public int UntilLayer { get; set; }
    public string Summary { get; set; }
    public string Remarks { get; set; }

    /// <summary>
    /// A key value list of constructors.
    /// </summary>
    /// <remarks>
    /// Key is constructorName#ID
    /// </remarks>
    public Dictionary<string, GeneratedFunction> Constructors { get; set; } = new();
}
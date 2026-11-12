namespace NekoBot.Telegram.Bindgen;

public class GeneratedFunction {
    /// <summary>
    /// A name of the constructor/function as seen in schema.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The id of the constructor/function. Used for denoting the type for API.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The layer this constructor/function got introduced in.
    /// </summary>
    public int SinceLayer { get; set; }

    /// <summary>
    /// The layer this constructor/function got removed (changed).
    /// </summary>
    /// <remarks>
    /// 0 if it was never removed.
    /// </remarks>
    public int UntilLayer { get; set; }

    /// <summary>
    /// Constructor/function description. Used for generating documentation.
    /// </summary>
    public string Summary { get; set; }
    
    /// <summary>
    /// Constructor/function remarks. Used for generating documentation.
    /// </summary>
    public string Remarks { get; set; }

    /// <summary>
    /// Parameters for this constructor/function.
    /// </summary>
    public List<GeneratedParameter> Parameters { get; set; } = new();
}
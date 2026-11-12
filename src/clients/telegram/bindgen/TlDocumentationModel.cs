namespace NekoBot.Telegram.Bindgen;

public class TlDocumentationModel {
    public string Name { get; set; }
    public string Summary { get; set; }
    public string Remarks { get; set; }
    public string Tl { get; set; }
    public Dictionary<string, string>? Parameters { get; set; }
    public Dictionary<string, string>? Constructors { get; set; }
    public Dictionary<string, string>? Methods { get; set; }
    public string Result { get; set; }
    public bool UsersOnly { get; set; }
    public bool BotsOnly { get; set; }
    public bool AuthorizationRequired { get; set; }
    public List<(string code, string type, string description)> PossibleErrors { get; set; }
    public Dictionary<string, string> SeeAlson { get; set; }
}
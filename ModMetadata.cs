using SPTarkov.Server.Core.Models.Spt.Mod;

namespace MilitarySurplus;

public class ModMetadata : IModMetadata
{
    public string ModGuid { get; init; } = "com.maels.militarysurplus";
    public string Name { get; init; } = "MilitarySurplus";
    public string Author { get; init; } = "Maels";
    public List<string> Contributors { get; init; } = [];
    public SemanticVersioning.Version Version { get; init; } = new("1.4.1");
    public SemanticVersioning.Range SptVersion { get; init; } = new(">=4.1.2");
    public bool HasPrepatcher { get; init; } = false;
    public List<string> Incompatibilities { get; init; } = [];
    public Dictionary<string, SemanticVersioning.Range> ModDependencies { get; init; } = [];
    public string Url { get; init; } = "";
    public string License { get; init; } = "MIT";
}
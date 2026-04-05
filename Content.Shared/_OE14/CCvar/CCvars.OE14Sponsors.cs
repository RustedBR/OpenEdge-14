using Robust.Shared.Configuration;

namespace Content.Shared.CCVar;

public sealed partial class CCVars
{
    public static readonly CVarDef<bool> SponsorsEnabled =
        CVarDef.Create("oe14.sponsor_enabled", false, CVar.SERVERONLY);

    public static readonly CVarDef<string> SponsorsApiUrl =
        CVarDef.Create("oe14.sponsor_api_url", "http://localhost:8000/sponsors", CVar.SERVERONLY | CVar.CONFIDENTIAL);

    public static readonly CVarDef<string> SponsorsApiKey =
        CVarDef.Create("oe14.sponsor_api_key", "token", CVar.SERVERONLY | CVar.CONFIDENTIAL);
}

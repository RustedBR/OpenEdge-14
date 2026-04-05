using Content.Shared._OE14.AuraDNA;
using Content.Shared._OE14.MagicVision;
using Content.Shared._OE14.MagicVision.Components;
using Content.Shared.Mobs;
using Content.Shared.StatusEffectNew;
using Robust.Shared.Random;
using Robust.Shared.Utility;

namespace Content.Server._OE14.AuraImprint;

/// <summary>
/// This system handles the basic mechanics of spell use, such as doAfter, event invocation, and energy spending.
/// </summary>
public sealed partial class OE14AuraImprintSystem : OE14SharedAuraImprintSystem
{
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly OE14SharedMagicVisionSystem _vision = default!;
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<OE14AuraImprintComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<OE14HideMagicAuraStatusEffectComponent, StatusEffectAppliedEvent>(OnShuffleStatusApplied);
        SubscribeLocalEvent<OE14AuraImprintComponent, MobStateChangedEvent>(OnMobStateChanged);
    }

    private void OnShuffleStatusApplied(Entity<OE14HideMagicAuraStatusEffectComponent> ent, ref StatusEffectAppliedEvent args)
    {
        ent.Comp.Imprint = GenerateAuraImprint(args.Target);
        Dirty(ent);
    }

    private void OnMapInit(Entity<OE14AuraImprintComponent> ent, ref MapInitEvent args)
    {
        ent.Comp.Imprint = GenerateAuraImprint((ent.Owner, ent.Comp));
        Dirty(ent);
    }

    public string GenerateAuraImprint(Entity<OE14AuraImprintComponent?> ent)
    {
        if (!Resolve(ent, ref ent.Comp))
            return string.Empty;

        var letters = new[] { "ä", "ã", "ç", "ø", "ђ", "œ", "Ї", "Ћ", "ў", "ž", "Ћ", "ö", "є", "þ"};
        var imprint = string.Empty;

        for (var i = 0; i < ent.Comp.ImprintLength; i++)
        {
            imprint += letters[_random.Next(letters.Length)];
        }

        return $"[color={ent.Comp.ImprintColor.ToHex()}]{imprint}[/color]";
    }

    private void OnMobStateChanged(Entity<OE14AuraImprintComponent> ent, ref MobStateChangedEvent args)
    {
        switch (args.NewMobState)
        {
            case MobState.Critical:
            {
                _vision.SpawnMagicTrace(
                    Transform(ent).Coordinates,
                    new SpriteSpecifier.Rsi(new ResPath("_OE14/Actions/Spells/misc.rsi"), "skull"),
                    Loc.GetString("oe14-magic-vision-crit"),
                    TimeSpan.FromMinutes(10),
                    ent);
                break;
            }
            case MobState.Dead:
            {
                _vision.SpawnMagicTrace(
                    Transform(ent).Coordinates,
                    new SpriteSpecifier.Rsi(new ResPath("_OE14/Actions/Spells/misc.rsi"), "skull_red"),
                    Loc.GetString("oe14-magic-vision-dead"),
                    TimeSpan.FromMinutes(10),
                    ent);
                break;
            }
        }
    }
}

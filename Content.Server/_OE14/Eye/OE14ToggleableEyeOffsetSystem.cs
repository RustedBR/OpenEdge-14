using Content.Server.Movement.Components;
using Content.Shared._OE14.Eye;

namespace Content.Server._OE14.Eye;

public sealed class OE14ToggleableEyeOffsetSystem : EntitySystem
{
    public override void Initialize()
    {
        SubscribeLocalEvent<EyeComponent, OE14EyeOffsetToggleActionEvent>(OnToggleEyeOffset);
    }

    private void OnToggleEyeOffset(Entity<EyeComponent> ent, ref OE14EyeOffsetToggleActionEvent args)
    {
        if (!HasComp<EyeCursorOffsetComponent>(ent))
            AddComp<EyeCursorOffsetComponent>(ent);
        else
            RemComp<EyeCursorOffsetComponent>(ent);
    }
}

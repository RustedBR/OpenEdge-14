using System.Text;
using Content.Client.Items;
using Content.Client.Stylesheets;
using Content.Shared._OE14.LockKey.Components;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Timing;

namespace Content.Client._OE14.LockKey;

public sealed class OE14ClientLockKeySystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        Subs.ItemStatus<OE14KeyComponent>(ent => new OE14KeyStatusControl(ent));
    }
}


public sealed class OE14KeyStatusControl : Control
{
    private readonly Entity<OE14KeyComponent> _parent;
    private readonly RichTextLabel _label;
    public OE14KeyStatusControl(Entity<OE14KeyComponent> parent)
    {
        _parent = parent;

        _label = new RichTextLabel { StyleClasses = { StyleNano.StyleClassItemStatus } };
        AddChild(_label);
    }

    protected override void FrameUpdate(FrameEventArgs args)
    {
        base.FrameUpdate(args);

        if (_parent.Comp.LockShape is null)
            return;

        var sb = new StringBuilder("(");
        foreach (var item in _parent.Comp.LockShape)
        {
            sb.Append($"{item} ");
        }

        sb.Append(")");
        _label.Text = sb.ToString();
    }
}

using Robust.Shared.GameStates;
using Robust.Shared.Graphics;

namespace Content.Shared._OE14.CommunicationCrystal.Components;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(true)]
public sealed partial class OE14CommunicationCrystalComponent : Component
{
    [DataField, AutoNetworkedField]
    public TimeSpan? LastGlobalAnnouncement { get; set; }

    [DataField("AnnouncementTitle"), AutoNetworkedField]
    public string AnnouncementTitle = "Crystal Broadcast";

    [DataField("AnnouncementColor"), AutoNetworkedField]
    public Color AnnouncementColor = Color.White;

    [DataField("LocalMessageColor"), AutoNetworkedField]
    public Color LocalMessageColor = Color.White;

    [DataField("SenderColor"), AutoNetworkedField]
    public Color SenderColor = Color.White;

    public const int GlobalCost = 25;
    public const int LocalCost = 5;
    public const int GlobalCooldownSeconds = 40;
}
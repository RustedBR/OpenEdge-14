using Content.Client.IconSmoothing;
using Content.Shared._OE14.Wallpaper;
using Content.Shared.IconSmoothing;
using Robust.Client.GameObjects;

namespace Content.Client._OE14.Wallpaper;

public sealed class OE14ClientWallpaperSystem : OE14SharedWallpaperSystem
{
    public override void Initialize()
    {
        SubscribeLocalEvent<OE14WallpaperHolderComponent, AfterAutoHandleStateEvent>(OnHandleState, after: new[] { typeof(IconSmoothSystem), typeof(SharedRandomIconSmoothSystem), typeof(ClientRandomIconSmoothSystem) });
        SubscribeLocalEvent<OE14WallpaperHolderComponent, ComponentStartup>(OnStartup, after: new[] { typeof(IconSmoothSystem), typeof(SharedRandomIconSmoothSystem), typeof(ClientRandomIconSmoothSystem) });
    }

    private void OnStartup(Entity<OE14WallpaperHolderComponent> holder, ref ComponentStartup args)
    {
        if (!TryComp<SpriteComponent>(holder, out var sprite))
            return;

        UpdateVisuals(holder, sprite);
    }

    private void OnHandleState(Entity<OE14WallpaperHolderComponent> holder, ref AfterAutoHandleStateEvent args)
    {
        if (!TryComp<SpriteComponent>(holder, out var sprite))
            return;

        UpdateVisuals(holder, sprite);
    }

    private static void UpdateVisuals(Entity<OE14WallpaperHolderComponent> holder, SpriteComponent sprite)
    {
        //Remove old layers
        foreach (var key in holder.Comp.RevealedLayers)
        {
            sprite.RemoveLayer(key);
        }

        holder.Comp.RevealedLayers.Clear();

        //Add new layers
        var counter = 0;
        foreach (var wallpaper in holder.Comp.Layers)
        {
            var keyCode = $"wallpaper-layer-{counter}";
            holder.Comp.RevealedLayers.Add(keyCode);

            var index = sprite.LayerMapReserveBlank(keyCode);

            sprite.LayerSetSprite(index, wallpaper);
            counter++;
        }
    }
}

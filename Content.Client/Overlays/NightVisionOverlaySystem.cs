using Content.Shared.Inventory.Events;
using Content.Shared.Overlays;
using Robust.Client.Graphics;

namespace Content.Client.Overlays;

public sealed partial class NightVisionOverlaySystem : EquipmentHudSystem<NightVisionComponent>
{
    [Dependency] private readonly IOverlayManager _overlayMan = default!;
    [Dependency] private readonly ILightManager _lightMan = default!;

    private NightVisionOverlay _overlay = default!;
    private bool _overlayAdded;
    private bool _disabledLightingByNightVision;
    private bool _originalLightingState;

    public override void Initialize()
    {
        base.Initialize();
        _overlay = new();
    }

    protected override void UpdateInternal(RefreshEquipmentHudEvent<NightVisionComponent> component)
    {
        base.UpdateInternal(component);

        if (!_overlayAdded)
        {
            _overlayMan.AddOverlay(_overlay);
            _overlayAdded = true;
        }

        if (!_disabledLightingByNightVision && _lightMan.DrawLighting)
        {
            _disabledLightingByNightVision = true;
            _originalLightingState = _lightMan.DrawLighting;

            // Disable the lighting buffer so the world appears "fullbright" in the dark, but keep
            // the eye's DrawLight enabled. DrawLighting is deliberately separate from hard FOV, so
            // wall occlusion (FOV) remains intact and the player can't see through walls.
            _lightMan.DrawLighting = false;
        }
    }

    protected override void DeactivateInternal()
    {
        base.DeactivateInternal();

        if (_overlayAdded)
        {
            _overlayMan.RemoveOverlay(_overlay);
            _overlayAdded = false;
        }

        if (_disabledLightingByNightVision && !_lightMan.DrawLighting)
        {
            _lightMan.DrawLighting = _originalLightingState;
        }

        _disabledLightingByNightVision = false;
    }
}

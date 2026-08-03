using Content.Shared.Inventory.Events;
using Content.Shared.Overlays;
using Robust.Client.Graphics;

namespace Content.Client.Overlays;

public sealed partial class BlackAndWhiteOverlaySystem : EquipmentHudSystem<BlackAndWhiteOverlayComponent>
{
    [Dependency] private readonly IOverlayManager _overlayMan = default!;

    private BlackAndWhiteOverlay _overlay = default!;
    private bool _overlayAdded;

    public override void Initialize()
    {
        base.Initialize();

        _overlay = new();
    }

    protected override void UpdateInternal(RefreshEquipmentHudEvent<BlackAndWhiteOverlayComponent> component)
    {
        base.UpdateInternal(component);

        if (!_overlayAdded)
        {
            _overlayMan.AddOverlay(_overlay);
            _overlayAdded = true;
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
    }
}

using Robust.Shared.GameStates;

namespace Content.Shared.Overlays;

/// <summary>
/// Makes the entity see in a green-tinted night vision overlay.
/// When added to a clothing item it will also grant the wearer the same overlay.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class NightVisionComponent : Component;

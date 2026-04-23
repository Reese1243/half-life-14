using Robust.Shared.GameStates;

namespace Content.Shared.Roles.Components;

/// <summary>
/// Added to mind role entities to tag they are a member of the HECU.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class HecuRoleComponent : BaseMindRoleComponent;

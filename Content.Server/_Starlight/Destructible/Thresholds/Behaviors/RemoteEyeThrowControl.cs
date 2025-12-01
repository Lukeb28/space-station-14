using Content.Server.Destructible.Thresholds.Behaviors;
using Content.Server.Destructible;
using Content.Server._Starlight.Computers.RemoteEye;

namespace Content.Server._Starlight.Destructible.Thresholds.Behaviors;

[Serializable]
[DataDefinition]
public sealed partial class RemoteEyeThrowControl : IThresholdBehavior
{
    public void Execute(EntityUid owner, DestructibleSystem system, EntityUid? cause = null) =>
        system.EntityManager.EntitySysManager.GetEntitySystem<RemoteEyeSystem>().CameraExitRelay(owner);
}
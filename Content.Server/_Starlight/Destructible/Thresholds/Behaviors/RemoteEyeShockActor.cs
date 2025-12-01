using Content.Server.Destructible.Thresholds.Behaviors;
using Content.Server.Destructible;
using Content.Server._Starlight.Computers.RemoteEye;
using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Content.Shared.Damage.Prototypes;

namespace Content.Server._Starlight.Destructible.Thresholds.Behaviors;

[Serializable]
[DataDefinition]
public sealed partial class RemoteEyeShockActor : IThresholdBehavior
{
    [DataField("damageAmount")]
    public FixedPoint2 DamageAmount = FixedPoint2.New(10);

    public void Execute(EntityUid owner, DestructibleSystem system, EntityUid? cause = null)
    {
        var _remoteEye = system.EntityManager.EntitySysManager.GetEntitySystem<RemoteEyeSystem>();
        var _damageable = system.EntityManager.EntitySysManager.GetEntitySystem<DamageableSystem>();

        if (_remoteEye.TryGetRemoteEyeActor(owner, out var actor))
            _damageable.TryChangeDamage(actor.Value, new DamageSpecifier(system.PrototypeManager.Index<DamageTypePrototype>("Shock"), DamageAmount), true);
    }
}
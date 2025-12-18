using Content.Shared.Damage;

namespace Content.Shared._Starlight.Computers.RemoteEye;

public abstract class SharedCyberspaceCombatSystem : EntitySystem
{
    [Dependency] private readonly DamageableSystem _damageableSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        //SubscribeLocalEvent<CyberAttackEvent>(OnAttack);
    }

    //private void OnAttack(CyberAttackEvent args) => _damageableSystem.TryChangeDamage(args.Target, args.Damage);
}

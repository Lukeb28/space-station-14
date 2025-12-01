using Content.Shared.Actions;
using Content.Shared.Damage;

namespace Content.Shared._Starlight.Computers.RemoteEye;

public sealed partial class CyberAttackEvent : EntityTargetActionEvent
{
    [DataField(required: true)]
    public DamageSpecifier Damage;
}
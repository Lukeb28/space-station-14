using Content.Shared.Actions;
using Content.Shared.Damage;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Content.Shared._Starlight.Computers.RemoteEye;

[Serializable, NetSerializable]
public sealed class CyberAttackEvent : AttackEvent
{
    public readonly NetEntity? Target;
    public readonly NetEntity? Weapon;
    public readonly NetEntity User;

    public CyberAttackEvent(NetEntity user, NetCoordinates coordinates, NetEntity? target = null, NetEntity? weapon = null) : base(coordinates)
    {
        Target = target;
        User = user;
        Weapon = weapon;
    }
}
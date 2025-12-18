using Content.Shared.Damage;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.Prototypes;

namespace Content.Shared._Starlight.Computers.RemoteEye;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(fieldDeltas: true), AutoGenerateComponentPause]
public sealed partial class CyberspaceCombatComponent : Component
{
    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer)), AutoNetworkedField]
    [AutoPausedField]
    public TimeSpan NextAttack;

    [DataField, AutoNetworkedField]
    public float AttackRate = 1f;

    [AutoNetworkedField]
    public bool Attacking = false;

    /// <summary>
    /// If true, attacks will be repeated automatically without requiring the mouse button to be lifted.
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool AutoAttack = false;

    /// <summary>
    /// The combat presets available to this entity, this means different attack types, like 1h slash, 2h smash, punch, kick, etc.
    /// </summary>
    [AutoNetworkedField]
    public List<ProtoId<CombatPresetPrototype>> CombatPresets = new();

    /// <summary>
    /// Alternative combat presets.
    /// </summary>
    [AutoNetworkedField]
    public List<ProtoId<CombatPresetPrototype>> AltCombatPresets = new();

    /// <summary>
    /// Last used combat preset and the time it was used.
    /// </summary>
    [DataField, AutoNetworkedField]
    public (ProtoId<CombatPresetPrototype>, TimeSpan)? LastUsedPreset;

    /// <summary>
    /// Last used alt combat preset and the time it was used.
    /// </summary>
    [DataField, AutoNetworkedField]
    public (ProtoId<CombatPresetPrototype>, TimeSpan)? LastUsedAltPreset;

    /// <summary>
    /// Time required before reusing the same preset.
    /// </summary>
    [DataField, AutoNetworkedField]
    public TimeSpan PresetTimeout = TimeSpan.FromSeconds(2);
}

[Prototype("combatPreset")]
public sealed partial class CombatPresetPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; } = default!;

    /// <summary>
    /// The damage this attack does.
    /// </summary>
    [DataField]
    public DamageSpecifier Damage = new();

    /// <summary>
    /// Modifier to the attack rate when using this preset.
    /// </summary>
    [DataField]
    public float AttackRateModifier = 1f;

    /// <summary>
    /// Whether or not this attack hits in a wide arc.
    /// </summary>
    [DataField]
    public bool WideAttack = false;

    /// <summary>
    /// The range of this attack.
    /// </summary>
    [DataField]
    public float Range = 1f;

    /// <summary>
    /// The angle of this attack. (Works only for wide attacks)
    /// </summary>
    [DataField]
    public Angle Angle = Angle.FromDegrees(60);

    /// <summary>
    /// The animation played when using this attack.
    /// </summary>
    [DataField,]
    public EntProtoId Animation = "WeaponArcPunch";

    /// <summary>
    /// The rotation of the attack animation.
    /// </summary>
    [DataField]
    public Angle AnimationRotation = Angle.Zero;

    /// <summary>
    /// Whether or not the attack swings left or right.
    /// </summary>
    [DataField]
    public bool SwingLeft;

    /// <summary>
    /// This gets played whenever a melee attack is done. This is predicted by the client.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    [DataField("soundSwing")]
    public SoundSpecifier SwingSound { get; set; } = new SoundPathSpecifier("/Audio/Weapons/punchmiss.ogg")
    {
        Params = AudioParams.Default.WithVolume(-3f).WithVariation(0.025f),
    };

    [ViewVariables(VVAccess.ReadWrite)]
    [DataField("soundHit")]
    public SoundSpecifier? HitSound;

    /// <summary>
    /// Plays if no damage is done to the target entity.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    [DataField("soundNoDamage")]
    public SoundSpecifier NoDamageSound { get; set; } = new SoundCollectionSpecifier("WeakHit");
}
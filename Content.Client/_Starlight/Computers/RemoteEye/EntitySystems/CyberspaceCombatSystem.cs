using System.Linq;
using Content.Shared._Starlight.Computers.RemoteEye;
using Robust.Client.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Timing;
using Robust.Client.State;
using Robust.Shared.Prototypes;
using Robust.Client.Player;
using Content.Client.Gameplay;
using Content.Client.CombatMode;
using Content.Shared.ActionBlocker;
using Robust.Client.Input;
using Robust.Shared.Input;
using Content.Shared.Weapons.Melee.Events;
using Robust.Client.Graphics;

namespace Content.Client._Starlight.Computers.RemoteEye;

public sealed class CyberspaceCombatSystem : SharedCyberspaceCombatSystem
{
    [Dependency] private readonly IEyeManager _eyeManager = default!;
    [Dependency] private readonly TransformSystem _transform = default!;
    [Dependency] private readonly IStateManager _stateManager = default!;
    [Dependency] private readonly IGameTiming _gameTiming = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly IMapManager _mapManager = default!;
    [Dependency] private readonly SharedMapSystem _map = default!;
    [Dependency] private readonly IPlayerManager _player = default!;
    [Dependency] private readonly CombatModeSystem _combatMode = default!;
    [Dependency] private readonly ActionBlockerSystem _actionBlocker = default!;
    [Dependency] private readonly IInputManager _inputManager = default!;
    [Dependency] private readonly InputSystem _inputSystem = default!;

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        if (!_gameTiming.IsFirstTimePredicted)
            return;

        var player = _player.LocalEntity;

        if (player is not { Valid: true } entity)
            return;

        if (!TryComp<CyberspaceCombatComponent>(entity, out var component))
            return;

        if (!_combatMode.IsInCombatMode(entity) || !_actionBlocker.CanAttack(entity))
        {
            component.Attacking = false;
            return;
        }

        var useDown = _inputSystem.CmdStates.GetState(EngineKeyFunctions.Use);
        var altDown = _inputSystem.CmdStates.GetState(EngineKeyFunctions.UseSecondary);

        if (component.AutoAttack || useDown != BoundKeyState.Down && altDown != BoundKeyState.Down)
            if (component.Attacking)
                RaisePredictiveEvent(new StopAttackEvent(GetNetEntity(entity)));

        if (component.Attacking || component.NextAttack > _gameTiming.CurTime)
            return;

        var mousePos = _eyeManager.PixelToMap(_inputManager.MouseScreenPosition);

        if (mousePos.MapId == MapId.Nullspace)
            return;

        EntityCoordinates targetPos;

        if (_mapManager.TryFindGridAt(mousePos, out var gridUid, out _))
            targetPos = _transform.ToCoordinates(gridUid, mousePos);
        else
            targetPos = _transform.ToCoordinates(_map.GetMap(mousePos.MapId), mousePos);

        if (useDown == BoundKeyState.Down)
            MainAttack(entity, mousePos, targetPos, component);
    }

    /// <summary>
    /// Handles the main attack logic for cyberspace combat.
    /// </summary>
    /// <param name="user">User which attacks</param>
    /// <param name="mousePos">Mouse position of user</param>
    /// <param name="targetPos">Target position where we attack</param>
    /// <param name="component">Combat component of user</param>
    /// <param name="weaponUid">Weapon</param>
    private void MainAttack(EntityUid user, MapCoordinates mousePos, EntityCoordinates targetPos, CyberspaceCombatComponent component, EntityUid? weaponUid = null)
    {
        var userPos = _transform.GetMapCoordinates(user);

        ProtoId<CombatPresetPrototype> preset = component.CombatPresets.FirstOrDefault();
        if (component.LastUsedPreset != null && component.LastUsedPreset.Value.Item2 + component.PresetTimeout > _gameTiming.CurTime)
            preset = component.LastUsedPreset.Value.Item1;

        if (!_prototypeManager.TryIndex<CombatPresetPrototype>(preset, out var presetProto))
            return;

        if (mousePos.MapId != userPos.MapId || (userPos.Position - mousePos.Position).Length() > presetProto.Range)
            return;

        EntityUid? target = null;
        if (_stateManager.CurrentState is GameplayStateBase screen)
            target = screen.GetClickedEntity(mousePos);

        if (target == null)
        {
            if (_mapManager.TryFindGridAt(mousePos, out var gridUid, out _))
                target = gridUid;
            else
                target = _map.GetMapOrInvalid(mousePos.MapId);
        }

        RaisePredictiveEvent(new CyberAttackEvent(GetNetEntity(user), GetNetCoordinates(targetPos), GetNetEntity(target), weaponUid == null ? null : GetNetEntity(weaponUid)));
    }
}
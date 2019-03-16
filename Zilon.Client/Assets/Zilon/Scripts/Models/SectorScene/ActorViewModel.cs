using System;
using System.Collections.Generic;

using Assets.Zilon.Scripts.Models.SectorScene;
using Assets.Zilon.Scripts.Services;

using JetBrains.Annotations;

using UnityEngine;
using UnityEngine.EventSystems;

using Zenject;

using Zilon.Core.Client;
using Zilon.Core.Common;
using Zilon.Core.Players;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

public class ActorViewModel : MonoBehaviour, IActorViewModel
{
    private const float MOVE_SPEED_Q = 1f;
    private const float END_MOVE_COUNTER = 0.3f;

    [NotNull] [Inject] private readonly ISectorUiState _playerState;

    [NotNull] [Inject] private readonly ILogService _logService;

    [NotNull] [Inject] private readonly ICommandBlockerService _commandBlockerService;

    public ActorGraphicBase GraphicRoot;
    public ActorHpBar ActorHpBar;

    private readonly List<HitSfx> _effectList;

    private Vector3 _targetPosition;
    private float? _moveCounter;
    private MoveCommandBlocker _moveCommandBlocker;

    public ActorViewModel()
    {
        _effectList = new List<HitSfx>();
    }

    public event EventHandler Selected;
    public event EventHandler MouseEnter;

    public IActor Actor { get; set; }


    [UsedImplicitly]
    public void Start()
    {
        Actor.Moved += Actor_Moved;
        Actor.Person.Survival.Dead += Survival_Dead;
        Actor.DamageTaken += Actor_DamageTaken;
        Actor.OnArmorPassed += Actor_OnArmorPassed;
        Actor.OnDefence += Actor_OnDefence;
        Actor.OpenedContainer += Actor_OpenedContainer;

        if (ActorHpBar != null)
        {
            ActorHpBar.Actor = Actor;

            if (ReferenceEquals(_playerState.ActiveActor, this))
            {
                ActorHpBar.gameObject.SetActive(false);
            }
        }
    }

    [UsedImplicitly]
    public void OnDestroy()
    {
        Actor.Moved -= Actor_Moved;
        Actor.Person.Survival.Dead -= Survival_Dead;
        Actor.DamageTaken -= Actor_DamageTaken;
        Actor.OnArmorPassed -= Actor_OnArmorPassed;
        Actor.OnDefence -= Actor_OnDefence;
        Actor.OpenedContainer -= Actor_OpenedContainer;
    }

    [UsedImplicitly]
    public void FixedUpdate()
    {
        //TODO Можно вынести в отдельный компонент, который уничтожается после выполнения движения.
        if (_moveCounter != null)
        {
            transform.position = Vector3.Lerp(transform.position, _targetPosition, _moveCounter.Value);
            _moveCounter += Time.deltaTime * MOVE_SPEED_Q;

            if (_moveCounter >= END_MOVE_COUNTER)
            {
                _moveCounter = null;
                _moveCommandBlocker.Release();
                _moveCommandBlocker = null;
            }
        }
    }

    [UsedImplicitly]
    public void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        Selected?.Invoke(this, new EventArgs());
    }

    public void OnMouseEnter()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        MouseEnter?.Invoke(this, new EventArgs());
    }

    public void AddHitEffect(HitSfx sfxObject)
    {
        sfxObject.HitSfxes = _effectList;
        sfxObject.transform.position = transform.position + Vector3.up * 0.2f * _effectList.Count;

        _effectList.Add(sfxObject);
    }

    private void Survival_Dead(object sender, EventArgs e)
    {
        var isHumanPerson = Actor.Owner is HumanPlayer;
        GraphicRoot.ProcessDeath(
            rootObject: gameObject,
            isRootRotting: !isHumanPerson
        );

        Destroy(GetComponent<Collider2D>());

        if (_playerState.ActiveActor.Equals(this))
        {
            _playerState.ActiveActor = null;
        }
    }

    private void Actor_Moved(object sender, EventArgs e)
    {
        _moveCounter = 0;
        var actorNode = (HexNode)Actor.Node;
        var worldPositionParts = HexHelper.ConvertToWorld(actorNode.OffsetX, actorNode.OffsetY);
        _targetPosition = new Vector3(worldPositionParts[0], worldPositionParts[1] / 2, -1);
        _moveCommandBlocker = new MoveCommandBlocker();
        _commandBlockerService.AddBlocker(_moveCommandBlocker);
        GraphicRoot.ProcessMove(_targetPosition);
    }

    private void Actor_OnDefence(object sender, DefenceEventArgs e)
    {
        _logService.Log($"{sender} defends {e.PrefferedDefenceItem}, roll: {e.FactToHitRoll}, success: {e.SuccessToHitRoll}.");
    }

    private void Actor_OnArmorPassed(object sender, ArmorEventArgs e)
    {
        _logService.Log($"{sender} successfully used armor rank: {e.ArmorRank}, roll: {e.FactRoll}, success: {e.SuccessRoll}.");
    }

    private void Actor_DamageTaken(object sender, DamageTakenEventArgs e)
    {
        _logService.Log($"{sender} take damage {e.Value}");
    }

    private void Actor_OpenedContainer(object sender, OpenContainerEventArgs e)
    {
        var containerNode = (HexNode)e.Container.Node;
        var worldPositionParts = HexHelper.ConvertToWorld(containerNode.OffsetX, containerNode.OffsetY);
        var targetPosition = new Vector3(worldPositionParts[0], worldPositionParts[1] / 2, -1);

        GraphicRoot.ProcessInteractive(targetPosition);
    }
}
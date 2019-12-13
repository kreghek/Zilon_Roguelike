using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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

    [NotNull] [Inject] private readonly ICommandBlockerService _commandBlockerService;

    public ActorGraphicBase GraphicRoot;

    private readonly List<HitSfx> _effectList;

    private Vector3 _targetPosition;
    private float? _moveCounter;
    private MoveCommandBlocker _moveCommandBlocker;
    private TaskScheduler _taskScheduler;

    public ActorViewModel()
    {
        _effectList = new List<HitSfx>();
    }

    public event EventHandler Selected;
    public event EventHandler MouseEnter;

    public IActor Actor { get; set; }

    public ISectorUiState PlayerState { get; set; }

    [UsedImplicitly]
    public void Start()
    {
        _taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();

        Actor.Moved += Actor_Moved;
        if (Actor.Person.Survival != null)
        {
            Actor.Person.Survival.Dead += Survival_Dead;
        }

        Actor.OpenedContainer += Actor_OpenedContainer;
    }

    [UsedImplicitly]
    public void OnDestroy()
    {
        Actor.Moved -= Actor_Moved;

        if (Actor.Person.Survival != null)
        {
            Actor.Person.Survival.Dead -= Survival_Dead;
        }

        Actor.OpenedContainer -= Actor_OpenedContainer;
    }

    [UsedImplicitly]
    public void FixedUpdate()
    {
        //TODO Можно вынести в отдельный компонент, который уничтожается после выполнения движения.
        if (_moveCounter == null)
        {
            return;
        }

        transform.position = Vector3.Lerp(transform.position, _targetPosition, _moveCounter.Value);
        _moveCounter += Time.fixedDeltaTime * MOVE_SPEED_Q;

        if (_moveCounter >= END_MOVE_COUNTER)
        {
            _moveCounter = null;
            _moveCommandBlocker.Release();
            _moveCommandBlocker = null;
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

        if (PlayerState != null && ReferenceEquals(PlayerState.ActiveActor, this))
        {
            PlayerState.ActiveActor = null;
        }
    }

    private void Actor_Moved(object sender, EventArgs e)
    {
        // Этот код обработчика должен выполниться в потоке Unity и не важно в каком потоке было выстелено событие.
        Task.Factory.StartNew(() =>
        {
            _moveCounter = 0;
            var actorHexNode = (HexNode)Actor.Node;
            var worldPositionParts = HexHelper.ConvertToWorld(actorHexNode.OffsetX, actorHexNode.OffsetY);
            _targetPosition = new Vector3(worldPositionParts[0], worldPositionParts[1] / 2, actorHexNode.OffsetY - 0.26f);
            _moveCommandBlocker = new MoveCommandBlocker();
            _commandBlockerService.AddBlocker(_moveCommandBlocker);
            GraphicRoot.ProcessMove(_targetPosition);
        }, CancellationToken.None, TaskCreationOptions.None, _taskScheduler);
    }

    private void Actor_OpenedContainer(object sender, OpenContainerEventArgs e)
    {
        var containerNode = (HexNode)e.Container.Node;
        var worldPositionParts = HexHelper.ConvertToWorld(containerNode.OffsetX, containerNode.OffsetY);
        var targetPosition = new Vector3(worldPositionParts[0], worldPositionParts[1] / 2, -1);

        GraphicRoot.ProcessInteractive(targetPosition);
    }
}
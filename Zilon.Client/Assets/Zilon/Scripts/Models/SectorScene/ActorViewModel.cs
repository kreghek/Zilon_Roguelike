﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Assets.Zilon.Scripts.Models.SectorScene;

using JetBrains.Annotations;

using UnityEngine;
using UnityEngine.EventSystems;

using Zenject;

using Zilon.Core.Client;
using Zilon.Core.Client.Sector;
using Zilon.Core.Common;
using Zilon.Core.PersonModules;
using Zilon.Core.Players;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

public class ActorViewModel : MonoBehaviour, ICanBeHitSectorObject, IActorViewModel
{
    private const float MOVE_SPEED_Q = 1f;
    private const float END_MOVE_COUNTER = 0.3f;

    private TaskScheduler _taskScheduler;

    private readonly object _lockObj;

    [NotNull] [Inject] private readonly IAnimationBlockerService _commandBlockerService;

    [NotNull] [Inject] private readonly IPlayer _player;

    public ActorGraphicBase GraphicRoot { get; private set; }

    private readonly List<HitSfx> _effectList;

    private Vector3 _targetPosition;
    private float? _moveCounter;
    private ICommandBlocker _moveCommandBlocker;

    public ActorViewModel()
    {
        _effectList = new List<HitSfx>();
        _lockObj = new object();
    }

    public event EventHandler Selected;
    public event EventHandler MouseEnter;

    public IActor Actor { get; set; }

    public ISectorUiState PlayerState { get; set; }
    public object Item { get => Actor; }
    public Vector3 Position { get => transform.position; }

    public void SetGraphicRoot(ActorGraphicBase actorGraphic)
    {
        GraphicRoot = actorGraphic;
    }

    [UsedImplicitly]
    public void Start()
    {
        _taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();

        Actor.Moved += Actor_Moved;
        var survivalModule = Actor.Person.GetModuleSafe<ISurvivalModule>();
        if (survivalModule != null)
        {
            survivalModule.Dead += Survival_Dead;
        }

        Actor.OpenedContainer += Actor_OpenedContainer;
    }

    [UsedImplicitly]
    public void OnDestroy()
    {
        Actor.Moved -= Actor_Moved;

        if (Actor.Person.GetModuleSafe<ISurvivalModule>() != null)
        {
            Actor.Person.GetModule<ISurvivalModule>().Dead -= Survival_Dead;
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

            lock (_lockObj)
            {
                if (_moveCommandBlocker != null)
                {
                    _moveCommandBlocker.Release();
                    _moveCommandBlocker = null;
                }
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

    private async void Survival_Dead(object sender, EventArgs e)
    {
        await Task.Factory.StartNew(() =>
        {
            var isHumanPerson = Actor.Person == _player.MainPerson;
            GraphicRoot.ProcessDeath(
                rootObject: gameObject,
                isRootRotting: !isHumanPerson
            );

            Destroy(GetComponent<Collider2D>());

            if (PlayerState != null && ReferenceEquals(PlayerState.ActiveActor, this))
            {
                PlayerState.ActiveActor = null;
            }
        }, CancellationToken.None, TaskCreationOptions.None, _taskScheduler);
    }

    private async void Actor_Moved(object sender, EventArgs e)
    {
        await Task.Factory.StartNew(() =>
        {
            _moveCounter = 0;
            var actorHexNode = (HexNode)Actor.Node;
            var worldPositionParts = HexHelper.ConvertToWorld(actorHexNode.OffsetCoords);
            _targetPosition = new Vector3(worldPositionParts[0], worldPositionParts[1] / 2, actorHexNode.OffsetCoords.Y - 0.26f);

            if (GraphicRoot.gameObject.activeSelf)
            {
                lock (_lockObj)
                {
                    if (_moveCommandBlocker is null)
                    {
                        _moveCommandBlocker = new AnimationCommonBlocker();
                        _commandBlockerService.AddBlocker(_moveCommandBlocker);
                    }
                }
            }
            GraphicRoot.ProcessMove(_targetPosition);
        }, CancellationToken.None, TaskCreationOptions.None, _taskScheduler);
    }

    private async void Actor_OpenedContainer(object sender, OpenContainerEventArgs e)
    {
        await Task.Factory.StartNew(() =>
        {
            var containerNode = (HexNode)e.Container.Node;
            var worldPositionParts = HexHelper.ConvertToWorld(containerNode.OffsetCoords);
            var targetPosition = new Vector3(worldPositionParts[0], worldPositionParts[1] / 2, -1);

            GraphicRoot.ProcessInteractive(targetPosition);
        }, CancellationToken.None, TaskCreationOptions.None, _taskScheduler);
    }
}
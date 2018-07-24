using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Zilon.Core.Client;
using Zilon.Core.Common;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

public class ActorViewModel : MonoBehaviour, IActorViewModel
{
    private const float MOVE_SPEED_Q = 1;
    private const float END_MOVE_COUNTER = 1;

    public ActorGraphicBase GraphicRoot;

    private Vector3 _targetPosition;
    private float? _moveCounter;

    public event EventHandler Selected;
    public IActor Actor { get; set; }


    public void Start()
    {
        Actor.OnMoved += Actor_OnMoved;
        Actor.OnDead += Actor_OnDead;
    }

    private void Actor_OnDead(object sender, EventArgs e)
    {
        GraphicRoot.ProcessDeath();
    }

    private void Actor_OnMoved(object sender, EventArgs e)
    {
        _moveCounter = 0;
        var actorNode = (HexNode) Actor.Node;
        var worldPositionParts = HexHelper.ConvertToWorld(actorNode.OffsetX, actorNode.OffsetY);
        _targetPosition = new Vector3(worldPositionParts[0], worldPositionParts[1], -1);
    }

    void Update()
    {
        if (_moveCounter != null)
        {
            transform.position = Vector3.Lerp(transform.position, _targetPosition, _moveCounter.Value);
            _moveCounter += Time.deltaTime * MOVE_SPEED_Q;

            if (_moveCounter >= END_MOVE_COUNTER)
            {
                _moveCounter = null;
            }
        }
    }

    public void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        Selected?.Invoke(this, new EventArgs());
    }
}
using System;
using System.Threading.Tasks;
using UnityEngine;
using Zilon.Core.Common;
using Zilon.Core.Tactics;

public class ActorVM : MonoBehaviour
{
    private TaskCompletionSource<bool> _moveTaskSource;
    private const float MOVE_SPEED_Q = 1;
    private const float END_MOVE_COUNTER = 1;

    private Vector3 _targetPosition;
    private float? _moveCounter;
    private Task _moveTask;

    public event EventHandler OnSelected;
    public IActor Actor { get; set; }

    public void Start()
    {
        Actor.OnMoved += ActorOnOnMoved;
    }

    private void ActorOnOnMoved(object sender, EventArgs e)
    {
        _moveCounter = 0;
        var worldPositionParts = HexHelper.ConvertToWorld(Actor.Node.OffsetX, Actor.Node.OffsetY);
        _targetPosition = new Vector3(worldPositionParts[0], worldPositionParts[1]);
    }

    // Update is called once per frame
    void Update()
    {
        if (_moveCounter != null)
        {
            transform.position = Vector3.Lerp(transform.position, _targetPosition, _moveCounter.Value);
            _moveCounter += Time.deltaTime * MOVE_SPEED_Q;

            if (_moveCounter >= END_MOVE_COUNTER)
            {
                _moveCounter = null;

                _moveTaskSource?.TrySetResult(true);
            }
        }
    }

    public Task<bool> MoveToPointAsync(Vector3 targetPosition)
    {
        _moveTaskSource = new TaskCompletionSource<bool>();
        _targetPosition = targetPosition;
        _moveCounter = 0;
        return _moveTaskSource.Task;
    }

    public void OnMouseDown()
    {
        OnSelected(this, new EventArgs());
    }
}

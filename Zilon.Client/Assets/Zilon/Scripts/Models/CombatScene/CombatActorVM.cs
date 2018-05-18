using System;
using System.Threading.Tasks;
using UnityEngine;

//TODO Переименованить в просто Актёр, потому что актёры есть только в бою
public class CombatActorVM : MonoBehaviour
{
    private TaskCompletionSource<bool> _moveTaskSource;
    private const float MOVE_SPEED_Q = 1;

    private Vector3 _targetPosition;
    private float? _moveCounter;
    private Task _moveTask;

    public event EventHandler OnSelected;

    // Update is called once per frame
    void Update()
    {
        if (_moveCounter != null)
        {
            transform.position = Vector3.Lerp(transform.position, _targetPosition, _moveCounter.Value);
            _moveCounter += Time.deltaTime * MOVE_SPEED_Q;

            if (_moveCounter >= 1)
            {
                _moveCounter = null;
                _moveTaskSource.TrySetResult(true);
            }
        }
    }

    public Task<bool> MoveToPointAsync(Vector3 targetPosition)
    {
        _moveTaskSource = new TaskCompletionSource<bool>();
        this.targetPosition = targetPosition;
        moveCounter = 0;
        return _moveTaskSource.Task;
    }

    public void OnMouseDown()
    {
        OnSelected(this, new EventArgs());
    }
}

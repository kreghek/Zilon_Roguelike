using System;
using UnityEngine;

//TODO Переименованить в просто Актёр, потому что актёры есть только в бою
public class CombatActorVM : MonoBehaviour
{

    private const float moveSpeedQ = 1;

    private Vector3 targetPosition;
    private float? moveCounter;

    public event EventHandler OnSelected;

    // Update is called once per frame
    void Update()
    {
        if (moveCounter != null)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, moveCounter.Value);
            moveCounter += Time.deltaTime * moveSpeedQ;

            if (moveCounter >= 1)
            {
                moveCounter = null;
            }
        }
    }

    public void ChangeTargetPosition(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
        moveCounter = 0;
    }

    public void OnMouseDown()
    {
        OnSelected(this, new EventArgs());
    }
}

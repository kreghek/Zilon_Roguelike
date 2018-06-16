using System;
using UnityEngine;

public class GroupVM : MonoBehaviour
{
    public SpriteRenderer Icon;

    public bool IsSelected { get; private set; }
    public bool IsMoving { get; set; }
    public MapLocation CurrentLocation { get; set; }
    public event EventHandler OnSelect;


    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (CurrentLocation != null && (CurrentLocation.transform.position - transform.position).magnitude >= 0.01f)
        {
            transform.position =
                Vector3.Lerp(transform.position, CurrentLocation.transform.position, Time.deltaTime * 3);
        }
    }

    void OnMouseDown()
    {
        if (!IsSelected)
        {
            IsSelected = true;
            OnSelect(this, new EventArgs());
        }
    }

    public void SetSelectState(bool state)
    {
        Icon.color = state ? Color.white : Color.gray;

        IsSelected = state;
    }
}
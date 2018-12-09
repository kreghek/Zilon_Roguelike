using System;

using UnityEngine;

public class Rotting : MonoBehaviour
{
    private float _rottingCounter;
    private SpriteRenderer[] _spriteRenderers;

    public event EventHandler OnSelect;

    /// <summary>
    /// Это объект, который будет уничтожен, когда время жизни трупа кончится.
    /// </summary>
    public GameObject RootObject;

    /// <summary>
    /// Указывает, нужно ли уничтожить корневой объект после гниения.
    /// </summary>
    /// <remarks>
    /// Выставляется false для персонажа игрока, чтобы труп был виден до перезапуска.
    /// </remarks>
    public bool RootRotting;


    public Rotting()
    {
        RootRotting = true;
    }

    public void Start()
    {
        _spriteRenderers = RootObject.GetComponentsInChildren<SpriteRenderer>();
    }

    public void Update()
    {
        if (RootRotting)
        {
            _rottingCounter += Time.deltaTime;

            foreach (var spriteRenderer in _spriteRenderers)
            {
                spriteRenderer.color = new Color(1, 1, 1, 1 - _rottingCounter);
            }

            if (_rottingCounter >= 1)
            {
                Destroy(RootObject);
            }
        }
        else
        {
            Destroy(this);
        }
    }
}

using UnityEngine;

public class CorpseActivity : MonoBehaviour
{
    private float _fallingCounter;
    private float _rottingCounter;
    private bool _isFalling;
    private SpriteRenderer[] _spriteRenderers;

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

    public CorpseActivity()
    {
        _isFalling = true;
        RootRotting = true;
    }

    public void Start()
    {
        _spriteRenderers = RootObject.GetComponentsInChildren<SpriteRenderer>();
    }

    public void Update()
    {
        if (_isFalling)
        {

            _fallingCounter += Time.deltaTime * 2;
            if (_fallingCounter > 1)
            {
                _fallingCounter = 1;
            }

            if (_fallingCounter >= 1)
            {
                _isFalling = false;
            }

            var targetRotation = Quaternion.AngleAxis(90, Vector3.forward);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, _fallingCounter);
        }
        else
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
}

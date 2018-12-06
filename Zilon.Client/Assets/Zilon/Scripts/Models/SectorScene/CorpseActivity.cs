using UnityEngine;

public class CorpseActivity : MonoBehaviour
{
    private float _fallingCounter;
    private float _rotingCounter;
    private bool _isFalling;
    private SpriteRenderer[] _spriteRenderers;

    /// <summary>
    /// Это объект, который будет уничтожен, когда время жизни трупа кончится.
    /// </summary>
    public GameObject RootObject;

    public CorpseActivity()
    {
        _isFalling = true;
    }

    void Start()
    {
        _spriteRenderers = RootObject.GetComponentsInChildren<SpriteRenderer>();
    }

    void Update()
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
            _rotingCounter += Time.deltaTime;

            foreach (var spriteRenderer in _spriteRenderers)
            {
                spriteRenderer.color = new Color(1, 1, 1, 1 - _rotingCounter);
            }

            if (_rotingCounter >= 1)
            {
                Destroy(RootObject);
            }
        }
    }
}

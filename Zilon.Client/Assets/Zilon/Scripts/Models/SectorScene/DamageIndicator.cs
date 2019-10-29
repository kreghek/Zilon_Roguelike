using UnityEngine;

public class DamageIndicator : MonoBehaviour
{
    private const float LIFETIME = 1;
    private const string INDICATOR_SORTING_LAYER = "SceneIndication";

    private float _lifetmeCounter;
    private Vector3 _objectPosition;
    private Vector3 _diePosition;

    public TextMesh Text;

    public void Init(ActorViewModel actorViewModel, int damageValue)
    {
        _objectPosition = actorViewModel.gameObject.transform.position + Vector3.left * Random.Range(-0.3f, 0.3f);
        _diePosition = _objectPosition + Vector3.up * 0.5f;
        gameObject.transform.position = _objectPosition;

        Text.text = GetWoundString(damageValue);
        // Это нужно делать, потому что нет возможности указать слой в инспекторе.
        // https://answers.unity.com/questions/595634/3d-textmesh-not-being-drawn-properly-in-a-2d-game.html
        Text.GetComponent<MeshRenderer>().sortingLayerName = INDICATOR_SORTING_LAYER;
    }

    private static string GetWoundString(int damageValue)
    {
        if (1 <= damageValue && damageValue <= 2)
        {
            return "Low Wound";
        }
        else if (3 <= damageValue && damageValue <= 5)
        {
            return "Certain Wound";
        }
        else
        {
            return "Heavy Wound";
        }
    }

    public void FixedUpdate()
    {
        _lifetmeCounter += Time.deltaTime;
        if (_lifetmeCounter < LIFETIME)
        {
            gameObject.transform.position = Vector3.Lerp(_objectPosition, _diePosition, _lifetmeCounter / LIFETIME);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

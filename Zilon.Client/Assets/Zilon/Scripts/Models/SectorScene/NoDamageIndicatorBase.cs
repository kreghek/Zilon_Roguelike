using Assets.Zilon.Scripts.Services;

using UnityEngine;

public class NoDamageIndicatorBase : MonoBehaviour
{
    private const float LIFETIME = 1;
    private const string INDICATOR_SORTING_LAYER = "SceneIndication";

    private float _lifetmeCounter;
    private Vector3 _objectPosition;
    private Vector3 _diePosition;

    public TextMesh Text;
    public string TextKey;

    internal Language CurrentLanguage { get; set; }

    public void Start()
    {
        Text.text = StaticPhrases.GetValue(TextKey, CurrentLanguage);
    }

    public void Init(ActorViewModel actorViewModel)
    {
        _objectPosition = actorViewModel.gameObject.transform.position + Vector3.left * Random.Range(-0.3f, 0.3f);
        _diePosition = _objectPosition + Vector3.up * 0.5f;
        gameObject.transform.position = _objectPosition;

        // Это нужно делать, потому что нет возможности указать слой в инспекторе.
        // https://answers.unity.com/questions/595634/3d-textmesh-not-being-drawn-properly-in-a-2d-game.html
        Text.GetComponent<MeshRenderer>().sortingLayerName = INDICATOR_SORTING_LAYER;
    }

    public void Update()
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


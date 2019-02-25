using JetBrains.Annotations;

using UnityEngine;

using Zenject;

using Zilon.Core.Client;

public class GameOverEffect : MonoBehaviour
{
    [NotNull] [Inject]
    private readonly ISectorModalManager _sectorModalManager;

    private float _counter;
    private const float _delayDuration = 3;

    void FixedUpdate()
    {
        _counter += Time.deltaTime;
        if (_counter >= _delayDuration)
        {
            _sectorModalManager.ShowScoreModal();
            Destroy(gameObject);
        }
    }
}

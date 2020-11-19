using JetBrains.Annotations;

using UnityEngine;

using Zenject;

using Zilon.Core.Client.Windows;

/// <summary>
/// Скрипт, отвечающий за эффект, когда игрок проигрывает.
/// 
/// Сейчас просто отображает окно результатов.
/// Потенциально, будет закрашивать экран в красный и запускать проигрышный трек.
/// </summary>
public class GameOverEffect : MonoBehaviour
{
    [NotNull]
    [Inject]
    private readonly ISectorModalManager _sectorModalManager;

    private float _counter;
    private const float _delayDuration = 3;

    void Update()
    {
        _counter += Time.deltaTime;
        if (_counter >= _delayDuration)
        {
            _sectorModalManager.ShowScoreModal();
            Destroy(gameObject);
        }
    }
}

using System.Threading.Tasks;

using JetBrains.Annotations;

using UnityEngine;

using Zenject;

using Zilon.Core.Tactics;

public class GameLoopUpdater : MonoBehaviour
{
    [NotNull] [Inject] private readonly IGameLoop _gameLoop;

    void Start()
    {
        StartGameLoopUpdate();
    }

    private async Task StartGameLoopUpdate()
    {
        while (true)
        {
            await _gameLoop.UpdateAsync();
        }
    }
}

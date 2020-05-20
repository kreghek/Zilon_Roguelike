using System.Threading.Tasks;
using Assets.Zilon.Scripts.Services;
using JetBrains.Annotations;

using UnityEngine;

using Zenject;

using Zilon.Core.Tactics;

public class GameLoopUpdater : MonoBehaviour
{
    [NotNull] [Inject] private readonly IGameLoop _gameLoop;
    [NotNull] [Inject] private readonly ICommandBlockerService _commandBlockerService;

    void Start()
    {
        StartGameLoopUpdate();
    }

    private async Task StartGameLoopUpdate()
    {
        while (true)
        {
            await _commandBlockerService.WaitBlockers();

            await _gameLoop.UpdateAsync();
        }
    }
}

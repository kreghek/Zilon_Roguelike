using System;
using System.Linq;
using JetBrains.Annotations;

using UnityEngine;

using Zenject;

using Zilon.Core.Client;

// ReSharper disable once CheckNamespace
// ReSharper disable once UnusedMember.Global
public class FollowingCamera : MonoBehaviour
{
    [NotNull] [Inject] private readonly ISectorUiState _playerState;

    public void Start()
    {
        UnityEngine.Random.InitState(1);
        var seq = new int[1000];
        for (var i = 0; i < seq.Length; i++)
        {
            seq[i] = UnityEngine.Random.Range(1, 100 + 1);
        }

        var gr = seq.GroupBy(x => x);
        var freq = gr.ToDictionary(x => x.Key, x => x.Count()).OrderBy(x => x.Key);
        Debug.Log(string.Join("\n",freq.Select(fr => fr.Key + "\t" + fr.Value)));
    }

    // ReSharper disable once UnusedMember.Local
    private void Update()
    {
        if (_playerState == null)
        {
            gameObject.SetActive(false);
            throw new InvalidOperationException($"Не удалось подтянуть зависимость {nameof(_playerState)}.");
        }

        if (_playerState.ActiveActor == null)
        {
            return;
        }

        var actorViewModel = _playerState.ActiveActor;
        var actorViewModelMonobehaviour = (ActorViewModel)actorViewModel;

        var targetPosition = actorViewModelMonobehaviour.transform.position;
        var cameraPlanePosition = new Vector3(targetPosition.x, targetPosition.y, -10);

        transform.position = Vector3.Lerp(transform.position,
            cameraPlanePosition,
            Time.deltaTime * 3);
    }
}
using System;

using JetBrains.Annotations;

using UnityEngine;

using Zenject;

using Zilon.Core.Client;

// ReSharper disable once CheckNamespace
// ReSharper disable once UnusedMember.Global
public class FollowingCamera : MonoBehaviour
{
    [NotNull] [Inject] private readonly ISectorUiState _playerState;

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
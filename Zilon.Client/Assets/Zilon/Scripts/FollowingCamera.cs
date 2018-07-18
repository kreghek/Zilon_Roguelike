using System;
using Assets.Zilon.Scripts.Models.SectorScene;
using UnityEngine;
using Zenject;

// ReSharper disable once CheckNamespace
// ReSharper disable once UnusedMember.Global
public class FollowingCamera : MonoBehaviour
{
    [Inject] private IPlayerState _playerState;

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

        var actorVm = _playerState.ActiveActor;
        
        transform.position = Vector3.Lerp(transform.position,
            actorVm.transform.position + new Vector3(0, 0, -3),
            Time.deltaTime * 3);
    }
}
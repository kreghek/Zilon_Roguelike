using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Zilon.Core.Client;

public class HpBar : MonoBehaviour
{
	public Image BarImage;
	
	[Inject] private IPlayerState _playerState;
	
	// Update is called once per frame
	void Update () {
		if (_playerState.ActiveActor == null)
		{
			return;
		}

		var actorVm = _playerState.ActiveActor;

		var hpPercentage = CalcPercentage(actorVm.Actor.Hp, actorVm.Actor.Person.Hp);

		BarImage.fillAmount = hpPercentage;
	}

	private float CalcPercentage(float actorHp, float personHp)
	{
		return actorHp/personHp;
	}
}

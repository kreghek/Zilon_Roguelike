using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Zilon.Core.Client;
using Zilon.Core.Persons;

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
        var person = actorVm.Actor.Person;


        var hpStat = person.Survival.Stats.Single(x => x.Type == SurvivalStatType.Health);


        var hpPercentage = CalcPercentage(hpStat.Value, hpStat.Range.Max);

		BarImage.fillAmount = hpPercentage;
	}

	private float CalcPercentage(float actorHp, float personHp)
	{
		return actorHp/personHp;
	}
}

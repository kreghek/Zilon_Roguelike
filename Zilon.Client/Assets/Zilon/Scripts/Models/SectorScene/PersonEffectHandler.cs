using UnityEngine;
using Zenject;
using Zilon.Core.Client;
using Zilon.Core.Persons;

public class PersonEffectHandler : MonoBehaviour
{
	[Inject] private IPlayerState _playerState;

	public Transform EffectParent;
	public EffectViewModel EffectPrefab;

	void Start()
	{
		UpdateEffects();
		
		var person = _playerState.ActiveActor.Actor.Person;
		person.Survival.StatCrossKeyValue += (sender, args) => { UpdateEffects(); };
	}

	private void UpdateEffects()
	{
		foreach (Transform childTrasform in EffectParent)
		{
			Destroy(childTrasform.gameObject);
		}
		
		var person = _playerState.ActiveActor.Actor.Person;

		var effects = person.Effects;

		foreach (var effect in effects.Items)
		{
			var survivalHazardEffect = effect as SurvivalStatHazardEffect;
			if (survivalHazardEffect != null)
			{
				var effectViewModel = Instantiate(EffectPrefab, EffectParent);
				effectViewModel.Title = $"{survivalHazardEffect.Level} {survivalHazardEffect.Type}";
			}
		}
	}
}

using UnityEngine;
using UnityEngine.UI;
using Zilon.Core.Persons;

public class PerkItemViewModel : MonoBehaviour {

	public Text LevelText;
	public Image IconImage;
	public Image SelectedBorder;

	public void Init(IPerk perk)
	{
		var iconSprite = CalcIcon(perk);

		IconImage.sprite = iconSprite;

		if (perk.CurrentLevel != null)
		{
			LevelText.gameObject.SetActive(true);
			LevelText.text = $"{perk.CurrentLevel.Primary} +{perk.CurrentLevel.Sub}";
		}
		else
		{
			LevelText.gameObject.SetActive(false);
		}
	}
	
	private Sprite CalcIcon(IPerk perk)
	{
		var iconSprite = Resources.Load<Sprite>($"Icons/perks/{perk.Scheme.Sid}");
		return iconSprite;
	}
}

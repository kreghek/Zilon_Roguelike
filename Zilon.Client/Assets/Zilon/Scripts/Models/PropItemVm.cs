using System;
using UnityEngine;
using UnityEngine.UI;
using Zilon.Core.Persons;

public class PropItemVm : MonoBehaviour
{
	public Text CountText;
	public Image IconImage;

	public Sprite FoodSprite;
	public Sprite WeaponSprite;

	public string Sid;
	
	private IProp _prop;

	public void Init(IProp prop)
	{
		_prop = prop;

		var resource = prop as Resource;
		if (resource != null)
		{
			CountText.gameObject.SetActive(true);
			CountText.text = $"x{resource.Count}";
		}
		else
		{
			CountText.gameObject.SetActive(false);
		}

		Sid = prop.Scheme.Sid;

		switch (prop.Scheme.Sid)
		{
			case "food-pack":
				IconImage.sprite = FoodSprite;
				break;
			
			case "short-sword":
				IconImage.sprite = WeaponSprite;
				break;
			
			default:
				throw new NotSupportedException();
		}
	}
}

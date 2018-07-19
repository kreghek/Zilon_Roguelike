using System;
using UnityEngine;
using UnityEngine.UI;
using Zilon.Core.Persons;

public class PropItemVm : MonoBehaviour
{
	private IProp _prop;
	
	public Text CountText;
	public Image IconImage;
	public Image SelectedBorder;

	public Sprite FoodSprite;
	public Sprite WeaponSprite;
	
	public string Sid;

	public event EventHandler Click;

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

	public void SetSelectedState(bool value)
	{
		SelectedBorder.gameObject.SetActive(value);
	}

	public void Click_Handler()
	{
		Click?.Invoke(this, new EventArgs());
	}
}

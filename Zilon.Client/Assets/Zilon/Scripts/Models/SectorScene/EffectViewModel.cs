using UnityEngine;
using UnityEngine.UI;

public class EffectViewModel : MonoBehaviour
{

	public Text TitleText;
	
	public string Title { get; set; }

	void Update()
	{
		TitleText.text = Title;
	}
}

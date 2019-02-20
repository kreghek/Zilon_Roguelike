using UnityEngine;
using UnityEngine.UI;

public class PropInfoPopup : MonoBehaviour
{
    public Text NameText;

    public PropItemVm PropViewModel { get; set; }

    public void Start()
    {
        gameObject.SetActive(false);
    }

    public void SetPropViewModel(PropItemVm propViewModel)
    {
        PropViewModel = propViewModel;

        if (propViewModel != null)
        {
            gameObject.SetActive(true);

            NameText.text = propViewModel.ToString();

            transform.position = propViewModel.transform.position + new Vector3(0.4f, -0.4f);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Zilon.Core.Client;

public class LocationInfoPanel : MonoBehaviour
{
    [Inject] private readonly IGlobeUiState _globeUiState;

    public Text LocationTitleText;

    public void Start()
    {
        _globeUiState.HoverChanged += GlobeUiState_HoverChanged;
    }

    private void GlobeUiState_HoverChanged(object sender, EventArgs e)
    {
        LocationTitleText.text = string.Empty;

        if (_globeUiState.HoverViewModel is IGlobeNodeViewModel globeViewModel)
        {
            LocationTitleText.text = globeViewModel.Node.Scheme.Name.En ?? globeViewModel.Node.Scheme.Name.Ru;
        }
    }
}

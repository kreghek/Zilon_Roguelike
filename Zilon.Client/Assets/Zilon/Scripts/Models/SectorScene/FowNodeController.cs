using Assets.Zilon.Scripts.Models.SectorScene;

using UnityEngine;

using Zilon.Core.Tactics;

public class FowNodeController : MonoBehaviour, IFowObjectController
{
    private Color[] _baseSpriteColors;

    public MapNodeVM NodeViewModel;

    public SpriteRenderer[] NodeSprites;

    private void Start()
    {
        _baseSpriteColors = new Color[NodeSprites.Length];
        for (var i = 0; i < NodeSprites.Length; i++)
        {
            _baseSpriteColors[i] = NodeSprites[i].color;
        }
    }

    public void ChangeState(SectorMapNodeFowState state)
    {
        switch (state)
        {
            case SectorMapNodeFowState.TerraIncognita:
                NodeViewModel.gameObject.SetActive(false);
                break;

            case SectorMapNodeFowState.Observing:

                for (var i = 0; i < NodeSprites.Length; i++)
                {
                    var nodeSprite = NodeSprites[i];
                    nodeSprite.color = _baseSpriteColors[i];
                }

                NodeViewModel.gameObject.SetActive(true);

                break;

            case SectorMapNodeFowState.Explored:
                for (var i = 0; i < NodeSprites.Length; i++)
                {
                    var nodeSprite = NodeSprites[i];
                    var memoryColor = Color.Lerp(_baseSpriteColors[i], new Color(1, 1, 1, 0), 0.5f);
                    nodeSprite.color = memoryColor;
                }

                NodeViewModel.gameObject.SetActive(true);
                break;
        }
    }
}

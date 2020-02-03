using Assets.Zilon.Scripts.Models.SectorScene;

using UnityEngine;

using Zilon.Core.Tactics;

public class FowNodeController : MonoBehaviour, IFowObjectController
{
    public MapNodeVM NodeViewModel;

    public SpriteRenderer[] NodeSprites;

    public void ChangeState(SectorMapNodeFowState state)
    {
        switch (state)
        {
            case SectorMapNodeFowState.TerraIncognita:
                NodeViewModel.gameObject.SetActive(false);
                break;

            case SectorMapNodeFowState.Observing:
                foreach (var nodeSprite in NodeSprites)
                {
                    nodeSprite.color = Color.white;
                }

                NodeViewModel.gameObject.SetActive(true);
                break;

            case SectorMapNodeFowState.Explored:
                foreach (var nodeSprite in NodeSprites)
                {
                    nodeSprite.color = Color.gray;
                }

                NodeViewModel.gameObject.SetActive(true);
                break;
        }
    }
}

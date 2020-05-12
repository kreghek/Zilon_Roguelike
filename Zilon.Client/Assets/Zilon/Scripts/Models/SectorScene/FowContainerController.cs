using Assets.Zilon.Scripts.Models.SectorScene;

using UnityEngine;

using Zilon.Core.Tactics;

public class FowContainerController : MonoBehaviour, IFowObjectController
{
    public StaticObjectViewModel ViewModel;
    public SpriteRenderer[] NodeSprites;

    public void ChangeState(SectorMapNodeFowState state)
    {
        switch (state)
        {
            case SectorMapNodeFowState.TerraIncognita:
                ViewModel.gameObject.SetActive(false);
                break;

            case SectorMapNodeFowState.Observing:
                foreach (var nodeSprite in NodeSprites)
                {
                    nodeSprite.color = Color.white;
                }

                ViewModel.gameObject.SetActive(true);
                break;

            case SectorMapNodeFowState.Explored:
                foreach (var nodeSprite in NodeSprites)
                {
                    nodeSprite.color = Color.gray;
                }

                ViewModel.gameObject.SetActive(true);
                break;
        }
    }
}

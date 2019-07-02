using UnityEngine;

public class DamageIndicator : MonoBehaviour
{
    private float _lifetmeCounter;

    public TextMesh Text;

    public void Init(ActorViewModel actorViewModel, int damageValue)
    {
        gameObject.transform.position = actorViewModel.gameObject.transform.position;

        Text.text = damageValue.ToString();
    }

    public void FixedUpdate()
    {
        _lifetmeCounter += Time.deltaTime;
        if (_lifetmeCounter >= 1)
        {
            Destroy(gameObject);
        }
    }
}

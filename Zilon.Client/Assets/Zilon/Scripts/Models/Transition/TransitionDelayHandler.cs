using UnityEngine;

public class TransitionDelayHandler : MonoBehaviour
{
    private float _counterSeconds = 1;

    public SceneLoader SceneLoader;

    // Update is called once per frame
    void Update()
    {
        if (_counterSeconds <= 0)
        {
            StartLoadScene();
        }
        else
        {
            _counterSeconds -= Time.deltaTime;
        }
    }

    private void StartLoadScene()
    {
        SceneLoader.gameObject.SetActive(true);
    }
}

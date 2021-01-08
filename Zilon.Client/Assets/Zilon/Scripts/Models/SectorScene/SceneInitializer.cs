using UnityEngine;

public class SceneInitializer : MonoBehaviour
{
    public SleepShadowManager SleepShadowManager;

    public void Start()
    {
        SleepShadowManager.StartSceneLoadShadowAnimation();
        Destroy(gameObject);
    }
}

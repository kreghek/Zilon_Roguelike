using UnityEngine;

public class GlobalFollowCamera : MonoBehaviour
{
    public GameObject Target;

    private void Update()
    {
        if (Target == null)
        {
            return;
            //throw new ArgumentException("Не указан объект слежения.");
        }

        transform.position = Vector3.Lerp(transform.position,
            Target.transform.position + new Vector3(0, 0, -10),
            Time.deltaTime * 3);
    }

    //TODO Сделать тоже самое для камеры сектора.
    /// <summary>
    /// Формированное перемещение камеры в указанные координаты.
    /// </summary>
    /// <param name="target"> Целевые координаты карты. </param>
    /// <remarks>
    /// Используется установки камера в момент загрузки сцены.
    /// </remarks>
    public void SetPosition(Transform target)
    {
        transform.position = target.position + new Vector3(0, 0, -10);
    }
}

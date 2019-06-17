using UnityEngine;

//TODO Сделать отдельные крипты для каждой кнопки, которые будут содежать обработчики.
//Тогда этот объект станет не нужным.
/// <summary>
/// Скрипт для обработки UI на глобальной карте.
/// </summary>
public class GlobeUiHandler : MonoBehaviour
{
    public SceneLoader SectorSceneLoader;

    public void EnterButtonHandler()
    {
        // Загрузка текущего сектра.
        // Это используется только в диких секторах, потому что при переходе в
        // остальные виды категории узлов автоматически загружаются сектора в
        // зависимости от состояния объекта игрока.

        SectorSceneLoader.LoadScene();
    }
}

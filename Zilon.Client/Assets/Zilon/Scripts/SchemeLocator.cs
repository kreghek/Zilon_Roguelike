using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Zilon.Logic.Services;

public class SchemeLocator : MonoBehaviour, ISchemeLocator
{
    public SchemeFile[] GetAll(string directory)
    {
        // Loads all TextAssets into a list.
        Object[] jsonFileArray = Resources.LoadAll("Schemes/"+directory, typeof(TextAsset));

        // Adds the paths to the paths dictionary.
        var result = new List<SchemeFile>();
        foreach (Object asset in jsonFileArray)
        {
            var sid = asset.name;

            // Загрузка контента по указанному пути из ресурсов.
            string assetPath = AssetDatabase.GetAssetPath(asset);
            //TODO Разобраться, зачем эта операция
            assetPath = assetPath.Replace("Assets/Resources/", "");
            var content = LoadJson(assetPath);

            // Итоговый файл
            var file = new SchemeFile
            {
                Sid = sid,
                Content = content
            };

            result.Add(file);
        }

        return result.ToArray();

    }

    public static string LoadJson(string path)
    {
        string jsonFilePath = path.Replace(".json", "");
        TextAsset loadedJsonFile = Resources.Load<TextAsset>(jsonFilePath);
        return loadedJsonFile.text;
    }
}

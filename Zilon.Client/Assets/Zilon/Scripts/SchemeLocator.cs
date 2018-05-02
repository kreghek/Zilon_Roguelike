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
            string assetPath = AssetDatabase.GetAssetPath(asset);
            assetPath = assetPath.Replace("Assets/Resources/", "");

            Debug.Log($"Загрузка схемы: {assetPath}.");

            var content = LoadJson(assetPath);

            Debug.Log($"Контент схемы: {content}.");

            var file = new SchemeFile
            {
                Sid = assetPath,
                Content = content
            };
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

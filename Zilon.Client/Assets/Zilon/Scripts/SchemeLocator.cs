using System.Collections.Generic;
using UnityEngine;
using Zilon.Core.Services;

public class SchemeLocator : MonoBehaviour, ISchemeLocator
{
    public SchemeFile[] GetAll(string directory)
    {
        // Loads all TextAssets into a list.
        var schemeAssets = Resources.LoadAll<TextAsset>($"Schemes/{directory}");

        // Adds the paths to the paths dictionary.
        var result = new List<SchemeFile>();
        foreach (var schemeAsset in schemeAssets)
        {
            var sid = schemeAsset.name;
            var content = schemeAsset.text;

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
}

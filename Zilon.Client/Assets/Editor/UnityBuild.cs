using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
 
public class UnityBuild
{
    private static readonly string GameName = "LastImperialVagabond";
 
    public static void BuildWindows()
    {
		Debug.Log("### BUILDING ###");
		
        string[] scenesToBuild = new[]{
          "Assets/Scenes/title",
          "Assets/Scenes/combat",
          "Assets/Scenes/globe",
          "Assets/Scenes/scores"
        };
 
        string buildsPath = "./ClientBuild";
        string buildDirectory = Path.Combine(buildsPath, $"{GameName}-Windows-{DateTime.Now.ToString("yyyy-MMM-dd HH-mm-ss")}");
 
        if (Directory.Exists(buildDirectory) == false)
            Directory.CreateDirectory(buildDirectory);
 
        string buildName = $"{GameName}.exe";
 
        BuildPlayerOptions playerOptions = new BuildPlayerOptions()
        {
            scenes = scenesToBuild,
            locationPathName = Path.Combine(buildDirectory, buildName),
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.Development
        };
 
        var report = BuildPipeline.BuildPlayer(playerOptions);
		
		Debug.Log("###   DONE   ###");
 
        Debug.Log(report.summary);
		Debug.Log(report.files);
    }
}
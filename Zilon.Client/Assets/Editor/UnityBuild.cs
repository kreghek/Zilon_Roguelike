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
          "Assets/Scenes/title.unity",
          "Assets/Scenes/combat.unity",
          "Assets/Scenes/globe.unity",
          "Assets/Scenes/scores.unity"
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
            target = BuildTarget.StandaloneLinux64,
            options = BuildOptions.Development
        };
 
        var report = BuildPipeline.BuildPlayer(playerOptions);
		
		Debug.Log("###   DONE   ###");
 
        Debug.Log("### SUMMARY: "+report.summary);
		
		var files = string.Join(", ", report.files.Select(x=>x.path));
		Debug.Log("### FILES: " + files);
    }
}
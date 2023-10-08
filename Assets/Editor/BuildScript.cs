using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

public class BuildScript
{

    public static void BuildAndroid()
    {
        Build(BuildTarget.Android);
    }

    public static void BuildOSX()
    {
        Build(BuildTarget.StandaloneOSXIntel64);
    }

    public static void BuildWindows()
    {
        Build(BuildTarget.StandaloneWindows);
    }

    public static void BuildWebGL()
    {
        Build(BuildTarget.WebGL);
    }

    public static void Build(BuildTarget target)
    {
        BuildPipeline.BuildPlayer(GetScenes(), Environment.GetCommandLineArgs().Last(), target,
            BuildOptions.None);
    }

    private static string[] GetScenes()
    {
        var result = new List<string>();
        for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
        {
            var scene = EditorBuildSettings.scenes[i];
            if (scene.enabled)
            {
                result.Add(scene.path);
            }
        }

        return result.ToArray();
    }
}
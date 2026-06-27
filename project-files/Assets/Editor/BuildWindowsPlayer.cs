using System.IO;
using UnityEditor;
using UnityEngine;

public static class BuildWindowsPlayer
{
    public static void Build()
    {
        string outputDir = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "..", "Build", "Windows"));
        Directory.CreateDirectory(outputDir);

        PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
        PlayerSettings.defaultScreenWidth = 1280;
        PlayerSettings.defaultScreenHeight = 720;
        PlayerSettings.resizableWindow = true;
        PlayerSettings.companyName = "LZX";
        PlayerSettings.productName = "SpaceShooter";

        string[] scenes =
        {
            "Assets/Scenes/Main.unity",
            "Assets/Scenes/Level_01.unity",
            "Assets/Scenes/Play.unity"
        };

        BuildPipeline.BuildPlayer(
            scenes,
            Path.Combine(outputDir, "SpaceShooter.exe"),
            BuildTarget.StandaloneWindows64,
            BuildOptions.None
        );
    }
}

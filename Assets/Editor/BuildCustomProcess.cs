using Newtonsoft.Json.Linq;
using System.IO;

//using System.IO.Compression;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class BuildCustomProcess
{
    [PostProcessBuildAttribute(1)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        string[] pathComponents = pathToBuiltProject.Split('/');
        string folderpath = pathToBuiltProject.Substring(0, pathToBuiltProject.Length - pathComponents.Last().Length);
        string executablepath = pathComponents.Last();

        string version = PlayerSettings.bundleVersion;
        string[] splits = version.Split('.');
        int newversion = int.Parse(splits[2]) + 1;
        PlayerSettings.bundleVersion = splits[0] + "." + splits[1] + "." + newversion.ToString();

        Debug.Log("Code succesfully built");
        if (File.Exists(Application.dataPath + "/Resources/manifest.dat"))
        {
            string json = File.ReadAllText(Application.dataPath + "/Resources/manifest.dat");
            dynamic manifest = JObject.Parse(json);

            manifest.version = PlayerSettings.bundleVersion;
            manifest.localLogFolder = PlayerSettings.productName;
            manifest.executableName = executablepath;
            manifest.version = Application.version;
            manifest.producer = UnityEditor.PlayerSettings.companyName;
            manifest.compiledAt = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            File.WriteAllText(folderpath + "manifest.json", manifest.ToString());
            Debug.Log("Manifest succesfully built");

            string zipPath = pathToBuiltProject.Substring(0, pathToBuiltProject.Length - (pathComponents.Last().Length + pathComponents.ElementAt(pathComponents.Length - 2).Length + 1));

            try
            {
                if (File.Exists(zipPath + pathComponents.Last().Replace(".exe", "") + ".zip"))
                {
                    File.Delete(zipPath + pathComponents.Last().Replace(".exe", "") + ".zip");
                }
                System.IO.Compression.ZipFile.CreateFromDirectory(folderpath, zipPath + pathComponents.Last().Replace(".exe", "") + ".zip");
                Debug.Log("Zip succesfully created in " + zipPath + " with name " + pathComponents.Last().Replace(".exe", "") + ".zip");
            }
            catch
            {
                Debug.LogError("Error in compressing the folder");
            }
        }
        else
        {
            Debug.LogError("Missing manifest in Resource Folder");
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Video;

public class StreamingAssetManager : MonoBehaviour
{
    public static StreamingAssetManager instance;

    [HideInInspector]
    public string PathResources = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Coroutine LoadAudioClipFromStreamingAsset(string folder, string filename, Action<AudioClip> callback)
    {
        Resources.UnloadUnusedAssets();
        FileInfo file = SearchFile(folder, filename);
        if (file != null)
        {
            return StartCoroutine(LoadAsset(file, callback));
        }
        else
        {
            return null;
        }
    }

    public Coroutine LoadImageFromStreamingAsset(string folder, string filename, Action<Texture2D> callback)
    {
        Resources.UnloadUnusedAssets();
        FileInfo file = SearchFile(folder, filename);
        if (file != null)
        {
            return StartCoroutine(LoadAsset(file, callback));
        }
        else
        {
            return null;
        }
    }

    public bool LoadVideoClipFromStreamingAsset(string folder, string filename, VideoPlayer player)
    {
        Resources.UnloadUnusedAssets();
        FileInfo file = SearchFile(folder, filename);
        if (file != null)
        {
            player.source = VideoSource.Url;
            player.name = Path.GetFileNameWithoutExtension(file.FullName);
            player.url = "file://" + file.FullName.ToString();
            return true;
        }
        return false;
    }

    private FileInfo SearchFile(string folder, string filename)
    {
        try
        {
            DirectoryInfo directory = new DirectoryInfo(Path.Combine(Application.streamingAssetsPath, folder));

            //PROBLEMA VA NEL CATCH SE LA CARTELLA NON è IN STREAMING ASSETS --> TROVARE UN MODO PER AGGIRARE IN MODO CHE CERCHI PRIMA DA UNA PARTE E POI DALL?ALTRA
            FileInfo file;

            if (!directory.Exists)
            {
                if (PathResources != null)
                {
                    directory = new DirectoryInfo(Path.Combine(PathResources, folder));
                    file = directory.GetFiles(filename + ".*").ToList().FirstOrDefault(x =>
                    {
                        return x.Extension != ".meta";
                    });
                    if (file != null)
                        return file;
                }
            }
            else
            {
                file = directory.GetFiles(filename + ".*").ToList().FirstOrDefault(x =>
                {
                    return x.Extension != ".meta";
                });
                if (file != null)
                    return file;
            }
        }
        catch (DirectoryNotFoundException)
        {
            Debug.LogError("La cartella specificata non esiste");
        }
        catch (Exception e)
        {
            Debug.LogError("Errore generico " + e);
        }
        return null;
    }

    public List<string> GetAllFilenameAvailable(string folder)
    {
        DirectoryInfo directory = new DirectoryInfo(Path.Combine(Application.streamingAssetsPath, folder));
        List<FileInfo> files = new List<FileInfo>();
        if (directory.Exists)
        {
            files.AddRange(directory.GetFiles().ToList().Where(x =>
            {
                return x.Extension != ".meta";
            }));
        }
        if (PathResources != null)
        {
            directory = new DirectoryInfo(Path.Combine(PathResources, folder));
            if (directory.Exists)
            {
                files.AddRange(directory.GetFiles().ToList().Where(x =>
                {
                    return x.Extension != ".meta";
                }));
            }
        }
        return files.Select(x =>
        {
            return x.Name;
        }).ToList();
    }

    private IEnumerator LoadAsset(FileInfo playerFile, Action<AudioClip> callback)
    {
        string uri = "file://" + playerFile.FullName.ToString();
        AudioType type = playerFile.Extension == ".mp3" ? AudioType.MPEG : AudioType.WAV;
        var request = UnityWebRequestMultimedia.GetAudioClip(uri, type);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log(request.error);
        }
        else
        {
            AudioClip clip;
            if (type == AudioType.MPEG)
            {
                clip = NAudioPlayer.FromMp3Data(request.downloadHandler.data);
            }
            else
            {
                clip = DownloadHandlerAudioClip.GetContent(request);
            }
            clip.name = Path.GetFileNameWithoutExtension(playerFile.FullName);
            callback(clip);
        }
    }

    private IEnumerator LoadAsset(FileInfo playerFile, Action<Texture2D> callback)
    {
        string uri = "file://" + playerFile.FullName.ToString();
        var request = UnityWebRequestTexture.GetTexture(uri);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log(request.error);
        }
        else
        {
            Texture2D tex = DownloadHandlerTexture.GetContent(request);
            tex.name = Path.GetFileNameWithoutExtension(playerFile.FullName);
            callback(tex);
        }
    }
}
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer), typeof(YoutubePlayer.YoutubePlayer))]
public class YoutubePlayerManager : MonoBehaviour
{
    public static YoutubePlayerManager instance;

    private YoutubePlayer.YoutubePlayer runner;
    private VideoPlayer player;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            Init();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Init()
    {
        runner = GetComponent<YoutubePlayer.YoutubePlayer>();
        player = GetComponent<VideoPlayer>();
        player.playOnAwake = false;
        player.waitForFirstFrame = true;
    }

    public async Task<VideoPlayer> PlayVideo(string id, Camera targer)
    {
        player.targetCamera = targer;
        await runner.PlayVideoAsync("https://www.youtube.com/watch?v=" + id);
        return player;
    }
}
using UnityEngine;
using UnityEngine.Video;

public class CreditsVideoController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private VideoPlayer player;

    [Header("Behavior")]
    [SerializeField] private bool loop = false;
    [SerializeField] private bool freezeOnLastFrame = true;

    [Header("Optional: start hidden")]
    [SerializeField] private GameObject videoRoot;

    private void Awake()
    {
        if (!player) player = GetComponent<VideoPlayer>();
        player.isLooping = loop;
        player.loopPointReached += OnVideoFinished;

        if (videoRoot != null)
            videoRoot.SetActive(false);
    }

    public void PlayVideo()
    {
        if (videoRoot != null && !videoRoot.activeSelf)
            videoRoot.SetActive(true);

        player.time = 0;
        player.frame = 0;
        player.Play();
    }

    public void StopVideo(bool hide = false)
    {
        player.Stop();
        if (hide && videoRoot != null)
            videoRoot.SetActive(false);
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        if (loop) return;
player.Stop();
            videoRoot.SetActive(false);

        if (freezeOnLastFrame) vp.Pause();
        else vp.Stop();
    }
}

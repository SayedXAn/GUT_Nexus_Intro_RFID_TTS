using UnityEngine;
using UnityEngine.Video;
using System.IO;

public class RFID_Manager : MonoBehaviour
{
    [Header("Database")]
    public GuestDatabase guestDatabase;

    [Header("Core Components")]
    public AudioSource welcomeAudioSource;
    public VideoPlayer videoPlayer;

    [Header("UI Elements")]
    public GameObject introImageObject;
    public GameObject videoDisplayObject;

    [Header("Video Output")]
    public RenderTexture videoRenderTexture;

    private string currentRfidInput = "";

    void Start()
    {
        introImageObject.SetActive(true);
        videoDisplayObject.SetActive(false);

        videoPlayer.playOnAwake = false;
        videoPlayer.Stop();
        videoPlayer.isLooping = false;

        videoPlayer.loopPointReached += OnVideoFinished;
    }

    void Update()
    {
        foreach (char c in Input.inputString)
        {
            if (c == '\r' || c == '\n')
            {
                if (currentRfidInput.Length > 0)
                {
                    ProcessRFID(currentRfidInput);
                }
                currentRfidInput = "";
            }
            else
            {
                currentRfidInput += c;
            }
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (currentRfidInput.Length > 0)
            {
                ProcessRFID(currentRfidInput);
            }
            currentRfidInput = "";
        }
    }

    void ProcessRFID(string id)
    {
        string trimmedID = id.Trim();
        Debug.Log("Processing ID: " + trimmedID);

        GuestEntry foundGuest = guestDatabase.GetGuestByID(trimmedID);

        if (foundGuest != null)
        {
            Debug.Log("Welcome, " + foundGuest.guestName + "!");

            CancelInvoke("StopVideoAndReturnToIntro");
            if (welcomeAudioSource.isPlaying) welcomeAudioSource.Stop();
            if (videoPlayer.isPlaying) videoPlayer.Stop();

            AudioClip clipToPlay = LoadGuestAudio(foundGuest.audioFilename);
            if (clipToPlay != null)
            {
                welcomeAudioSource.PlayOneShot(clipToPlay);
                Invoke("StopVideoAndReturnToIntro", clipToPlay.length);
                Debug.Log("Scheduling video stop in " + clipToPlay.length + " seconds.");
            }
            else
            {
                Debug.Log("No audio clip. Video will play to completion.");
            }

            introImageObject.SetActive(false);
            videoDisplayObject.SetActive(true);
            videoPlayer.Play();
        }
        else
        {
            Debug.LogWarning("RFID ID not found in database: " + trimmedID);
        }
    }

    AudioClip LoadGuestAudio(string filename)
    {
        if (string.IsNullOrEmpty(filename))
        {
            Debug.LogWarning("Guest has no audio file assigned.");
            return null;
        }

        string loadPath = Path.Combine("GuestAudio", Path.GetFileNameWithoutExtension(filename));
        AudioClip clipToPlay = Resources.Load<AudioClip>(loadPath);

        if (clipToPlay == null)
        {
            Debug.LogWarning("Audio file not found at Resources/" + loadPath);
        }

        return clipToPlay;
    }

    void StopVideoAndReturnToIntro()
    {
        Debug.Log("Media finished. Returning to intro image.");

        if (videoPlayer.isPlaying) videoPlayer.Stop();
        if (welcomeAudioSource.isPlaying) welcomeAudioSource.Stop();

        videoDisplayObject.SetActive(false);
        introImageObject.SetActive(true);

        ClearRenderTexture();
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        Debug.Log("Video file reached its end.");

        CancelInvoke("StopVideoAndReturnToIntro");

        StopVideoAndReturnToIntro();
    }

    void ClearRenderTexture()
    {
        if (videoRenderTexture == null)
        {
            Debug.LogError("Video Render Texture is NOT assigned in the Inspector!");
            return;
        }

        RenderTexture.active = videoRenderTexture;

        GL.Clear(true, true, Color.clear);

        RenderTexture.active = null;
        Debug.Log("Render Texture cleared.");
    }
}


using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public const string PrefMaster = "Vol_Master";
    public const string PrefMusic  = "Vol_Music";
    public const string PrefSFX    = "Vol_SFX";
    
    public static AudioManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private SoundLibrarySO soundLibrary;
    [SerializeField] private AudioMixer audioMixer;

    [Header("Music Source")]
    [SerializeField] private AudioSource musicSource;

    private const string MasterVolumeParam = "MasterVolume";
    private const string MusicVolumeParam  = "MusicVolume";
    private const string SFXVolumeParam    = "SFXVolume";

    private Coroutine _musicFadeCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
        soundLibrary.Init();
    }

    private void Start()
    {
        LoadAndApplyVolumes();
    }

    private void OnEnable()
    {
        EventManager.Instance.OnPathGenerated += HandlePathGenerated;
        EventManager.Instance.OnGameOver += HandleGameOver;
        EventManager.Instance.OnLevelCompleted += HandleGameOver;
    }

    private void OnDisable()
    {
        if (EventManager.Instance == null) return;
        EventManager.Instance.OnPathGenerated -= HandlePathGenerated;
        EventManager.Instance.OnGameOver -= HandleGameOver;
        EventManager.Instance.OnLevelCompleted -= HandleGameOver;
    }

    private void HandlePathGenerated() => PlayMusic(SoundID.MainTheme);
    private void HandleGameOver()      => StopMusicWithFade();

    public void PlaySFX(SoundID id, AudioSource source)
    {
        if (source == null)
        {
            Debug.LogWarning($"[AudioManager] PlaySFX called with null AudioSource for {id}");
            return;
        }

        if (!soundLibrary.TryGet(id, out SoundEntry entry))
        {
            Debug.LogWarning($"[AudioManager] SoundID {id} not found in library.");
            return;
        }

        source.outputAudioMixerGroup = entry.mixerGroup;
        source.pitch  = entry.GetPitch();
        source.PlayOneShot(entry.clip, entry.volume);
    }

    public void PlayMusic(SoundID id)
    {
        if (!soundLibrary.TryGet(id, out SoundEntry entry)) return;
        StopFadeCoroutine();

        musicSource.outputAudioMixerGroup = entry.mixerGroup;
        musicSource.clip   = entry.clip;
        musicSource.pitch  = entry.pitch;
        musicSource.volume = entry.volume;
        musicSource.loop   = true;
        musicSource.Play();
        Debug.Log($"isPlaying: {musicSource.isPlaying}, volume: {musicSource.volume}");
    }

    public void StopMusicWithFade(float duration = 1f)
    {
        StopFadeCoroutine();
        _musicFadeCoroutine = StartCoroutine(FadeMusicOut(duration));
    }

    public void ResumeMusicWithFade(float duration = 0.5f)
    {
        if (!musicSource.isPlaying)
            musicSource.Play();

        StopFadeCoroutine();
        _musicFadeCoroutine = StartCoroutine(FadeMusicIn(duration));
    }

    public void SetMasterVolume(float normalised)
    {
        normalised = Mathf.Clamp01(normalised);
        audioMixer.SetFloat(MasterVolumeParam, NormalisedToDB(normalised));
        PlayerPrefs.SetFloat(PrefMaster, normalised);
        PlayerPrefs.Save();
    }

    public void SetMusicVolume(float normalised)
    {
        normalised = Mathf.Clamp01(normalised);
        audioMixer.SetFloat(MusicVolumeParam, NormalisedToDB(normalised));
        PlayerPrefs.SetFloat(PrefMusic, normalised);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float normalised)
    {
        normalised = Mathf.Clamp01(normalised);
        audioMixer.SetFloat(SFXVolumeParam, NormalisedToDB(normalised));
        PlayerPrefs.SetFloat(PrefSFX, normalised);
        PlayerPrefs.Save();
    }

    public float GetMasterVolume() => PlayerPrefs.GetFloat(PrefMaster, 1f);
    public float GetMusicVolume()  => PlayerPrefs.GetFloat(PrefMusic,  1f);
    public float GetSFXVolume()    => PlayerPrefs.GetFloat(PrefSFX,    1f);

    private void LoadAndApplyVolumes()
    {
        SetMasterVolume(PlayerPrefs.GetFloat(PrefMaster, 1f));
        SetMusicVolume (PlayerPrefs.GetFloat(PrefMusic,  1f));
        SetSFXVolume   (PlayerPrefs.GetFloat(PrefSFX,    1f));
    }
    private static float NormalisedToDB(float normalised) =>
        normalised > 0.0001f ? Mathf.Log10(normalised) * 20f : -80f;

    private IEnumerator FadeMusicOut(float duration)
    {
        float startVolume = musicSource.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
            yield return null;
        }

        musicSource.volume = 0f;
        musicSource.Stop();
        _musicFadeCoroutine = null;
    }

    private IEnumerator FadeMusicIn(float duration)
    {
        float targetVolume = soundLibrary.TryGet(SoundID.MainTheme, out SoundEntry entry)
            ? entry.volume : 1f;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            musicSource.volume = Mathf.Lerp(0f, targetVolume, elapsed / duration);
            yield return null;
        }

        musicSource.volume = targetVolume;
        _musicFadeCoroutine = null;
    }

    private void StopFadeCoroutine()
    {
        if (_musicFadeCoroutine != null)
        {
            StopCoroutine(_musicFadeCoroutine);
            _musicFadeCoroutine = null;
        }
    }
}
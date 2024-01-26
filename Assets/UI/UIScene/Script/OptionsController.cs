using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using UnityEngine.Audio;

public class OptionsController : MonoBehaviour
{
    // TODO: is this the right way to connect two scripts?
    public MenuController menuController;
    // [Header("Camera")]
    // [Tooltip("The main camera, assign this through the editor. UNUSED")]        
    // public Camera mainCam;
    //
    // [Header("Timescale")]
    // [Tooltip("Timescale value. The default is 1 for most games. You may want to change it if you are pausing the game in a slow motion situation. UNUSED")] 
    // public float timeScale = 1f;

    [Header("Video")]
    public Dropdown vSyncDropdown;
    public Dropdown fullscreenDropdown;
    public Dropdown resolutionDropdown;

    [Header("Audio")]
    public AudioSource menuAudio;
    public Slider audioMasterSlider;
    public Slider audioMusicSlider;
    public Slider audioEffectSlider;
    
    [Tooltip("Remember to EXPOSE VOLUME of the mixer to script and RENAME THEM in the mixer menu")]
    public AudioMixer audioMixer;
    public float maxGain;
    public float minGain;
    
    [Header("Other Elements")]
    public Button SaveButton;
    public Button ExitButton;
    
    [Header("Event system")]
    [Tooltip("Event system")]
    public EventSystem uiEventSystem;

    [Header("Default selected")] [Tooltip("Default selected on the video panel")]
    public GameObject defaultSelectedMain;


    //Resolutions
    private Resolution[] _Resolutions;

    private SavedSettings _SavedSettings = new ();

    /// <summary>
    /// The start method; you will need to place all of your initial value getting/setting here. 
    /// </summary>
    public void Start()
    {
        //Set the first selected item
        uiEventSystem.firstSelectedGameObject = defaultSelectedMain;

        //Available Resolution is given by monitor
        InitializeResolutionOptions();
        
        string filePath = Application.persistentDataPath + "/" + "settings.json";
        SaveButton.onClick.AddListener(()=>{menuAudio.Play();Apply();_SavedSettings.Save(filePath);});
        ExitButton.onClick.AddListener(()=>{Cancel();menuController.MainMenu();});
        
        //stage saved settings
        if (_SavedSettings.Load(filePath))
        {
            //cancel loads staged settings to game, which works just fine here.
            Cancel();
        }
        else
        {
            // If fails to load settings, read settings from game
            SyncAudio();
            SyncVideo();
        }
        
        EnableAudioPreview();
        EnableVideoPreview();
        
    }


    // Initialize resolution options
    private void InitializeResolutionOptions()
    {
        // Ignore fresh rate
        _Resolutions = Screen.resolutions.GroupBy(resolution => new { resolution.width, resolution.height })
            .Select(group => group.First())
            .ToArray();
        
        // If possible, display in 16:9 and above 480p
        Resolution[] filteredResolutions = _Resolutions.Where((resolution) =>
            resolution.width >= 854 && (float)resolution.width / resolution.height == 16.0 / 9).ToArray();
        
        // Clear existing options and add the new resolution options
        resolutionDropdown.ClearOptions();
        
        if (filteredResolutions.Length >= 3)
        {
            _Resolutions = filteredResolutions;
        }
        
        resolutionDropdown.AddOptions(_Resolutions.Select(resolution => resolution.width + "*" + resolution.height).ToList());
        
    }

    // Find the index of a resolution in the dropdown options
    int FindResolutionIndex(int width, int height)
    {
        double rate = 0;
        for (int i = 0; i < _Resolutions.Length; i++)
        {
            if (_Resolutions[i].width == width && _Resolutions[i].height == height)
            {
                return i;
            }
        }

        return 0;
    }

    public void Apply()
    {
        ApplyAudio();
        ApplyVideo();
    }

    public void Cancel()
    {
        CancelAudio();
        CancelVideo();
    }

    private void DisableAudioPreview()
    {
        audioMasterSlider.onValueChanged.RemoveAllListeners();
        audioMusicSlider.onValueChanged.RemoveAllListeners();
        audioEffectSlider.onValueChanged.RemoveAllListeners();
    }

    private void EnableAudioPreview()
    {
        audioMasterSlider.onValueChanged.AddListener(_ => PreviewAudio());
        audioMusicSlider.onValueChanged.AddListener(_ => PreviewAudio());
        audioEffectSlider.onValueChanged.AddListener(_ => PreviewAudio());
    }

    /// <summary> 
    /// Sync audio options
    /// </summary>
    private void SyncAudio()
    {
        Debug.Log("sync");
        float master, music, effect;
        
        DisableAudioPreview();
        
        audioMixer.GetFloat("Master", out master);
        audioMasterSlider.value = Mathf.InverseLerp(minGain, maxGain, master);

        audioMixer.GetFloat("Music", out music);
        audioMusicSlider.value = Mathf.InverseLerp(minGain, maxGain, music);

        audioMixer.GetFloat("Effect", out effect);
        audioEffectSlider.value = Mathf.InverseLerp(minGain, maxGain, effect);
        
        EnableAudioPreview();
    }
    
    //TODO: Optimize slider value https://blog.csdn.net/lvcoc/article/details/105483834
    /// <summary> 
    /// Stage audio options
    /// </summary>
    private void StageAudio()
    {
        Debug.Log("stage");
         _SavedSettings.masterVolume = audioMasterSlider.value;
         _SavedSettings.musicVolume = audioMusicSlider.value;
         _SavedSettings.effectVolume = audioEffectSlider.value;
    }

    /// <summary> 
    /// The method for changing the applying new audio settings
    /// </summary>
    private void CancelAudio()
    {
        Debug.Log("cancel");
        audioMixer.SetFloat("Master", Mathf.Lerp(minGain, maxGain,  _SavedSettings.masterVolume));
        audioMixer.SetFloat("Effect", Mathf.Lerp(minGain, maxGain,  _SavedSettings.effectVolume));
        audioMixer.SetFloat("Music", Mathf.Lerp(minGain, maxGain,  _SavedSettings.musicVolume));
        SyncAudio();
    }

    /// <summary> 
    /// Cancel the audio setting changes
    /// </summary>
    private void ApplyAudio()
    {
        Debug.Log("apply");
        StageAudio();
        CancelAudio();
    }

    private void PreviewAudio()
    {
        Debug.Log("preview");
        audioMixer.SetFloat("Master", Mathf.Lerp(minGain, maxGain, audioMasterSlider.value));
        audioMixer.SetFloat("Music", Mathf.Lerp(minGain, maxGain, audioMusicSlider.value));
        audioMixer.SetFloat("Effect", Mathf.Lerp(minGain, maxGain, audioEffectSlider.value));
    }
    
    private void EnableVideoPreview()
    {
        resolutionDropdown.onValueChanged.AddListener(_ => PreviewVideo());
        vSyncDropdown.onValueChanged.AddListener(_ => PreviewVideo());
        fullscreenDropdown.onValueChanged.AddListener(_ => PreviewVideo());
    }

    private void DisableVideoPreview()
    {
        resolutionDropdown.onValueChanged.RemoveAllListeners();
        vSyncDropdown.onValueChanged.RemoveAllListeners();
        fullscreenDropdown.onValueChanged.RemoveAllListeners();
    }

    /// <summary> 
    /// Sync video options with in-game settings (in-game->UI)
    /// </summary>
    private void SyncVideo()
    {
        DisableVideoPreview();
        //Screen.currentResolution returns the size of the screen in windowed mode, so use Screen.width and Screen.height instead
        resolutionDropdown.value = FindResolutionIndex(Screen.width, Screen.height);
        resolutionDropdown.onValueChanged.AddListener(_ => PreviewVideo());
        fullscreenDropdown.value = Screen.fullScreen ? 0 : 1;
        vSyncDropdown.value = QualitySettings.vSyncCount == 0 ? 1 : 0;
        EnableVideoPreview();
    }

    /// <summary>
    /// Stage video options (for restoring settings if user cancel changes) (UI->stage)
    /// </summary>
    private void StageVideo()
    {
        _SavedSettings.width = _Resolutions[resolutionDropdown.value].width;
        _SavedSettings.height = _Resolutions[resolutionDropdown.value].height;
        
        _SavedSettings.vsync = vSyncDropdown.value == 0;
        _SavedSettings.fullscreen = fullscreenDropdown.value == 0;
    }

    /// <summary>
    /// Cancel the video setting changes (stage->in-game)
    /// </summary>
    private void CancelVideo()
    {
        // mainCam.fieldOfView =  _SavedSettings.fov;
        Screen.SetResolution(_SavedSettings.width, _SavedSettings.height, _SavedSettings.fullscreen);
        QualitySettings.vSyncCount = _SavedSettings.vsync? 1: 0;
        
        //It takes some time for the resolution to change, so wait for a short period
        IEnumerator Wait()
        {
            yield return new WaitForSeconds(0.1f);
            SyncVideo();
        }

        StartCoroutine(Wait());
    }

    /// <summary>
    /// Apply the video settings (UI->stage->in-game)
    /// </summary>
    private void ApplyVideo()
    {
        StageVideo();
        CancelVideo();
    }

    /// <summary>
    /// Preview the video settings (UI->in-game)
    /// </summary>
    private void PreviewVideo()
    {
        Screen.SetResolution(_Resolutions[resolutionDropdown.value].width,
            _Resolutions[resolutionDropdown.value].height, fullscreenDropdown.value==0);
        
        QualitySettings.vSyncCount = vSyncDropdown.value==0? 1: 0;
    }
}
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This class manages the information needed between scene.
/// It will not be destroyed on load and will debug log all carried information.
/// -IN- MenuManager
/// </summary>
public class PlayerSetup : MonoBehaviour
{
    public List<PlayerSetting> playerSettings { get; private set; }
    public OptionsSettings optionsSettings { get; private set; }

    private void Awake()
    {
        ServiceManager.Register(this);
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        playerSettings = new();
        optionsSettings = null;
    }

    private void OnDestroy()
    {
        ServiceManager.Unregister<GameManager>();
    }

    /// <summary>
    /// Create a PlayerSetting to be kept between scenes.
    /// -IN- MenuManager from StartGameAction()
    /// </summary>
    /// <returns></returns>
    public void AddPlayerSetting(int playerID, string playerName, Color playerColor, int playerTextureID)
    {
        PlayerSetting playerSetting = new PlayerSetting(playerID, playerName, playerColor, playerTextureID);
        playerSettings.Add(playerSetting);
    }

    /// <summary>
    /// Create a OptionsSetting to be kept between scenes.
    /// -IN- MenuManager from StartGameAction()
    /// </summary>
    /// <returns></returns>
    public void AddOptionsSetting(float soundVolume, float musicVolume, int languageIndex)
    {
        optionsSettings = new OptionsSettings(soundVolume, musicVolume, languageIndex);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
    }

    private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.isLoaded)
        {
            Debug.Log(string.Format("Scene {0} loaded with mode {1}", scene, mode));
            if (playerSettings != null)
            {
                Debug.Log(string.Format("{0} Player Settings found.", playerSettings.Count));
                for (int i = 0; i < playerSettings.Count; i++)
                {
                    Debug.Log(string.Format("Player ID: {0} | name: {1} | color: {2} | textureID: {3}",
                        playerSettings[i].playerID,
                        playerSettings[i].playerName,
                        playerSettings[i].playerColor,
                        playerSettings[i].playerTextureID));
                }
            }
            if (optionsSettings != null)
            {
                Debug.Log(string.Format("Options : sound volume: {0} | music volume: {1} | languageID: {2}",
                    optionsSettings.soundVolume,
                    optionsSettings.musicVolume,
                    optionsSettings.languageIndex));
            }
        }
    }
}

/// <summary>
/// Store the player settings to be put in the PlayerSetup list, which don't get destroyed between scenes.
/// </summary>
public class PlayerSetting
{
    public int playerID { get; private set; }
    public string playerName { get; private set; }
    public Color playerColor { get; private set; }
    public int playerTextureID { get; private set; }

    public PlayerSetting(int playerID, string playerName, Color playerColor, int playerTextureID)
    {
        this.playerID = playerID;
        this.playerName = playerName;
        this.playerColor = playerColor;
        this.playerTextureID = playerTextureID;
    }
}

/// <summary>
/// Store the options settings to be put in the PlayerSetup var, which don't get destroyed between scenes.
/// </summary>
public class OptionsSettings
{
    public float soundVolume { get; private set; }
    public float musicVolume { get; private set; }
    public int languageIndex { get; private set; }

    public OptionsSettings(float soundVolume, float musicVolume, int languageIndex)
    {
        this.soundVolume = soundVolume;
        this.musicVolume = musicVolume;
        this.languageIndex = languageIndex;
    }
}

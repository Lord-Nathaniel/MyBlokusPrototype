using System.Collections.Generic;
using UnityEngine;

public class PlayerSetup : MonoBehaviour
{
    public List<PlayerSetting> playerSettings { get; private set; }
    public OptionsSettings optionsSettings { get; private set; }

    void Awake()
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

    public void AddPlayerSetting(int playerID, string playerName, Color playerColor, int playerTextureId)
    {
        PlayerSetting playerSetting = new PlayerSetting(playerID, playerName, playerColor, playerTextureId);
        playerSettings.Add(playerSetting);
    }

    public void AddOptionsSetting(int soundVolume, int musicVolume, int languageIndex)
    {
        optionsSettings = new OptionsSettings(soundVolume, musicVolume, languageIndex);
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
    public int playerTextureId { get; private set; }

    public PlayerSetting(int playerID, string playerName, Color playerColor, int playerTextureId)
    {
        this.playerID = playerID;
        this.playerName = playerName;
        this.playerColor = playerColor;
        this.playerTextureId = playerTextureId;
    }
}

/// <summary>
/// Store the options settings to be put in the PlayerSetup var, which don't get destroyed between scenes.
/// </summary>
public class OptionsSettings
{
    public int soundVolume { get; private set; }
    public int musicVolume { get; private set; }
    public int languageIndex { get; private set; }

    public OptionsSettings(int soundVolume, int musicVolume, int languageIndex)
    {
        this.soundVolume = soundVolume;
        this.musicVolume = musicVolume;
        this.languageIndex = languageIndex;
    }
}

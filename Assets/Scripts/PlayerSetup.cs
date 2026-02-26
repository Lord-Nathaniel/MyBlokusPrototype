using System.Collections.Generic;
using UnityEngine;

public class PlayerSetup : MonoBehaviour
{
    public List<PlayerSetting> playerSettings { get; private set; }

    void Awake()
    {
        ServiceManager.Register(this);
        DontDestroyOnLoad(gameObject);
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

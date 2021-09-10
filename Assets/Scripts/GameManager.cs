using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] PlayerStats[] playerStats;
    public bool gameMenuOpened, dialogBoxOpened, shopMenuOpened;
    public int currentGoldCoins;

    // Start is called before the first frame update
    void Start()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(gameObject);
        playerStats = FindObjectsOfType<PlayerStats>();
/*        var temp = playerStats[0];
        playerStats[0] = playerStats[playerStats.Length - 1];
        playerStats[playerStats.Length-1] = temp;*/
    }

    // Update is called once per frame
    void Update()
    {
        if(dialogBoxOpened || gameMenuOpened || shopMenuOpened)
        {
            Player.instance.enableMovement = false;
        }
        else
        {
            Player.instance.enableMovement = true;
        }
    }

    public PlayerStats[] GetPlayerStats()
    {
        return playerStats;
    }
}

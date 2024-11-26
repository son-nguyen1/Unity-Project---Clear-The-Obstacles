using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    private static int playerGems = 0;
    private static int playerCoins = 0;
    private const string Gem_Tracker = "GemTracker";
    private const string Coin_Tracker = "CoinTracker";

    static GameData()
    {
        playerGems = PlayerPrefs.GetInt(Gem_Tracker, 0);
        playerCoins = PlayerPrefs.GetInt(Coin_Tracker, 0);
    }

    public static int GemTracker
    {
        get { return playerGems; }
        set
        {
            playerGems = value;
            PlayerPrefs.SetInt(Gem_Tracker, playerGems);
        }
    }

    public static int CoinTracker
    {
        get { return playerCoins; }
        set
        {
            playerCoins = value;
            PlayerPrefs.SetInt(Coin_Tracker, playerCoins);
        }
    }
}
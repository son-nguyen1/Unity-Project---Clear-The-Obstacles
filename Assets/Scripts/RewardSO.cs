using DailyRewardsSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Daily Rewards System / Rewards Database")]
public class RewardSO : ScriptableObject
{
    public Rewards[] rewardsArray;

    public int rewardsCount => rewardsArray.Length;

    public Rewards GetDailyRewards(int rewardIndex)
    {
        return rewardsArray[rewardIndex];
    }
}

public enum RewardType
{
    Gems,
    Coins
}

[System.Serializable]
public struct Rewards
{
    public RewardType[] rewardType;
    public int rewardAmount;
}
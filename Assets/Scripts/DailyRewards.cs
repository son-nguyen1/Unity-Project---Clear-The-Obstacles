using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using Unity.VisualScripting;

namespace DailyRewardsSystem
{
    public class DailyRewards : MonoBehaviour
    {
        [Header("Tracker UI")]
        [SerializeField] private TextMeshProUGUI gemTrackerTextUI;
        [SerializeField] private TextMeshProUGUI coinTrackerTextUI;

        [Header("Rewards UI")]
        [SerializeField] private GameObject[] rewardClaimedPanels;
        [SerializeField] private GameObject claimOnButton;
        [SerializeField] private GameObject claimOffButton;

        [Header("Timing")]
        [SerializeField] private double nextRewardReset = 48f;
        [SerializeField] private double nextRewardDelay = 23f;
        [SerializeField] private float rewardCheckInterval = 5f;

        [Header("Rewards Scriptable Objects")]
        [SerializeField] private RewardSO rewardSO;

        public int rewardsIndex;

        private const string Reward_Index = "RewardIndex";
        private const string Last_Reward_Claim_DateTime = "LastRewardClaimDateTime";

        private bool canClaimReward = false;

        private void Start()
        {
            Initialize();
            InvokeRepeating("CheckRewardAvailabilityTimer", 0f, rewardCheckInterval);
        }

        /// <summary>
        /// On launch, retrieve the reward key that is saved from previous sessions. Apply the key value onto the reward index.
        /// Use the index to correctly display the Panels for Claimed Reward, so players know that day has been claimed.
        /// Update the text for Gems and Coins that players have earned from previous sessions.
        /// </summary>
        private void Initialize()
        {
            if (string.IsNullOrEmpty(PlayerPrefs.GetString(Last_Reward_Claim_DateTime)))
            {
                PlayerPrefs.SetString(Last_Reward_Claim_DateTime, DateTime.UtcNow.Ticks.ToString());

                rewardsIndex = 0;
                ActivateRewards();
            }
            else
            {
                rewardsIndex = PlayerPrefs.GetInt(Reward_Index, 0);
            }

            UpdateGemTrackerTextUI();
            UpdateCoinTrackerTextUI();

            DisableRewardClaimedPanels();
        }

        private void DisableRewardClaimedPanels()
        {
            for (int i = 0; i < rewardsIndex; i++)
            {
                rewardClaimedPanels[i].SetActive(true);
            }
        }

        /// <summary>
        /// Checks how much time has passed since the last reward claim, so the next reward will become available or reset to Day 1.
        /// Retrieve the Last Reward Claim key and converts it into a long data type, which is the time in "ticks".
        /// Convert the long value from lastClaimUTCTime back into a DateTime object - the exact moment the last reward was claimed.
        /// Gets the current UTC time. Calculate the difference between the Last Claim Time and Current Time.
        /// 
        /// If >= 48 hours have passed, the rewards reset to Day 1. If between 24 and 48 hours, the next reward is available for claiming.
        /// If < 24 hours have passed, the player has to wait for the next 24-hour window.
        /// </summary>
        private void CheckRewardAvailabilityTimer()
        {
            long lastClaimUTCTime = long.Parse(PlayerPrefs.GetString(Last_Reward_Claim_DateTime, DateTime.UtcNow.Ticks.ToString()));
            DateTime lastRewardClaimDateTime = new DateTime(lastClaimUTCTime);
            DateTime currentDateTime = DateTime.UtcNow;

            double elapsedHours = (currentDateTime - lastRewardClaimDateTime).TotalHours;

            if (elapsedHours >= nextRewardReset)
            {
                rewardsIndex = 0;
                PlayerPrefs.SetInt(Reward_Index, rewardsIndex);

                ResetRewardClaimedPanels();
                ActivateRewards();
            }
            else if (elapsedHours >= nextRewardDelay && !canClaimReward)
            {
                ActivateRewards();
            }
            else
            {
                DeactivateRewards();
            }
        }

        private void OnEnable()
        {
            GameManager.OnClaimButtonClickEvent += HandleOnClaimButtonClickEvent;
        }

        private void OnDisable()
        {
            GameManager.OnClaimButtonClickEvent -= HandleOnClaimButtonClickEvent;

            CancelInvoke("CheckRewardAvailabilityTimer");
        }

        /// <summary>
        /// As the 'On Claim Button Click' event is triggered, use the reward index to identify what type and amount for that day.
        /// Add that amount of Gems or Coins into the total. The Panel for Claimed Reward of that day appears.
        /// Increment the reward index by 1 and save it in the key. Claim Off Button appears to let players know the reward is claimed.
        /// </summary>
        private void HandleOnClaimButtonClickEvent()
        {
            if (!canClaimReward) return;

            Rewards rewards = rewardSO.GetDailyRewards(rewardsIndex);

            for (int i = 0; i < rewards.rewardType.Length; i++)
            {
                if (rewards.rewardType[i] == RewardType.Gems)
                {
                    Debug.Log("<color=green>" + rewards.rewardType.ToString() + " Claimed : </color>+" + rewards.rewardAmount);
                    GameData.GemTracker += rewards.rewardAmount;
                    UpdateGemTrackerTextUI();
                }
                else if (rewards.rewardType[i] == RewardType.Coins)
                {
                    Debug.Log("<color=yellow>" + rewards.rewardType.ToString() + " Claimed : </color>+" + rewards.rewardAmount);
                    GameData.CoinTracker += rewards.rewardAmount;
                    UpdateCoinTrackerTextUI();
                }
            }

            rewardClaimedPanels[rewardsIndex].gameObject.SetActive(true);

            rewardsIndex++;
            PlayerPrefs.SetInt(Reward_Index, rewardsIndex);

            PlayerPrefs.SetString(Last_Reward_Claim_DateTime, DateTime.UtcNow.Ticks.ToString());

            DeactivateRewards(); // Reset the flag to begin the timer, after the reward is claimed
        }

        private void ActivateRewards()
        {
            canClaimReward = true; // This flag stops the timer, so it doesn't stack when reward hasn't been claimed.

            claimOnButton.gameObject.SetActive(true);
            claimOffButton.gameObject.SetActive(false);

            ResetDailyRewardsCycle();
        }

        private void DeactivateRewards()
        {
            canClaimReward = false;

            claimOnButton.gameObject.SetActive(false);
            claimOffButton.gameObject.SetActive(true);
        }

        /// <summary>
        /// When the reward index has gone through all the rewards, it will return to 0 and begin the cycle all over again.
        /// All Panels for Claimed Reward also disappear, as players can claim all the rewards, starting from Day 1 now.
        /// </summary>
        private void ResetDailyRewardsCycle()
        {
            if (rewardsIndex >= rewardSO.rewardsCount)
            {
                rewardsIndex = 0;
                PlayerPrefs.SetInt(Reward_Index, rewardsIndex);

                ResetRewardClaimedPanels();
            }
        }

        private void ResetRewardClaimedPanels()
        {
            foreach (GameObject claimedPanel in rewardClaimedPanels)
            {
                claimedPanel.gameObject.SetActive(false);
            }
        }

        private void UpdateGemTrackerTextUI()
        {
            gemTrackerTextUI.text = GameData.GemTracker.ToString();
        }

        private void UpdateCoinTrackerTextUI()
        {
            coinTrackerTextUI.text = GameData.CoinTracker.ToString();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                ResetGameState();
            }
        }

        private void ResetGameState()
        {
            PlayerPrefs.DeleteKey(Reward_Index);
            rewardsIndex = 0;

            ResetRewardClaimedPanels();
        }
    }
}
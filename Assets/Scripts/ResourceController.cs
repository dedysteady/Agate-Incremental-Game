using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static AchievementController;
using static GameManager;


public class ResourceController : MonoBehaviour
{
    public AudioSource AudioUpgrade;

    public Button ResourceButton;
    public Image ResourceImage;
    public Text ResourceDescription;
    public Text ResourceUpgradeCost;
    public Text ResourceUnlockCost;

    private ResourceConfig _config;

    private int _level = 1;

    public bool IsUnlocked { get; private set; }

    private void Start ()
    {
        ResourceButton.onClick.AddListener (() =>
        {
            if (IsUnlocked)
            {
                UpgradeLevel ();
            }
            else
            {
                UnlockResource ();
            }
        });

        ResourceButton.onClick.AddListener (UpgradeLevel);
    }


    public void SetConfig (ResourceConfig config)
    {
        _config = config;

        // ToString("0") berfungsi untuk membuang angka di belakang koma
        ResourceDescription.text = $"{ _config.Name } Lv. { _level }\n+{ GetOutput ().ToString ("0") }";
        ResourceUnlockCost.text = $"Unlock Cost\n{ _config.UnlockCost }";
        ResourceUpgradeCost.text = $"Upgrade Cost\n{ GetUpgradeCost () }";

        SetUnlocked (_config.UnlockCost == 0);
    }

    public double GetOutput ()
    {
        return _config.Output * _level;
    }

    public double GetUpgradeCost ()
    {
        return _config.UpgradeCost * _level;
    }

    public double GetUnlockCost ()
    {
        return _config.UnlockCost;
    }

    public void UpgradeLevel ()
    {
        double upgradeCost = GetUpgradeCost ();
        if (GameManager.Instance.TotalGold < upgradeCost)
        {
            return;
        }

        GameManager.Instance.AddGold (-upgradeCost);
        _level++;
        AudioUpgrade.Play();


        ResourceUpgradeCost.text = $"Upgrade Cost\n{ GetUpgradeCost () }";
        ResourceDescription.text = $"{ _config.Name } Lv. { _level }\n+{ GetOutput ().ToString ("0") }";
   
        if (_level == 25)
        {
            ResourceUpgradeCost.text = "LEVEL MAX";
            ResourceDescription.text = $"{ _config.Name } Lv. { _level }";
            ResourceButton.gameObject.SetActive(false);
        }

    }




    public void UnlockResource ()
    {
        double unlockCost = GetUnlockCost ();
        if (GameManager.Instance.TotalGold < unlockCost)
        {
            return;
        }

        SetUnlocked (true);
        GameManager.Instance.ShowNextResource();
        GameManager.Instance.changekirito++;
        GameManager.Instance.ChangeKirito();
        AchievementController.Instance.UnlockAchievement (AchievementType.UnlockResource, _config.Name);
    }

    public void SetUnlocked (bool unlocked)
    {
        IsUnlocked = unlocked;
        ResourceImage.color = IsUnlocked ? Color.white : Color.grey;
        ResourceUnlockCost.gameObject.SetActive (!unlocked);
        ResourceUpgradeCost.gameObject.SetActive (unlocked);
    }
}
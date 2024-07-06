using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class GameMenu : MonoBehaviour
{

    public GameObject theMenu;

    private CharStats[] playerStats;
    public TextMeshProUGUI[] nameText, hpText, mpText, levelText, expText;

    public Slider[] expSlider;
    public Image[] charImage;
    public GameObject[] charStatsHolder;
    public GameObject[] windows;
    public GameObject[] statusButtons;

    public TextMeshProUGUI statusName, statusHP, statusMP, statusLV, statusStr, statusDef, statusWpnEqpd, statusArmrEqpd, statusArmrPwr, statusEXP;
    public Image statusImage;

    public static GameMenu instance;
    // Start is called before the first frame update
    void Start()
    {

    }
    private void Awake()
    {

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            if (theMenu.activeInHierarchy)
            {
                CloseMenu();
            }
            else
            {
                theMenu.SetActive(true);
                UpdateMainStats();
                GameManager.instance.gameMenuOpen = true;
            }
        }
    }

    public void UpdateMainStats()
    {
        playerStats = GameManager.instance.playerStats;

        for (int i = 0; i < playerStats.Length; i++)
        {
            if (playerStats[i].gameObject.activeInHierarchy)
            {
                charStatsHolder[i].SetActive(true);
                nameText[i].text = playerStats[i].charName;
                hpText[i].text = "HP :" + playerStats[i].currentHP + "/" + playerStats[i].maxHP;
                mpText[i].text = "MP :" + playerStats[i].currentMP + "/" + playerStats[i].maxMP;
                levelText[i].text = "lv :" + playerStats[i].playerLever;
                expText[i].text = "" + playerStats[i].currentEXP + "/" + playerStats[i].expToNextLever[playerStats[i].playerLever];
                expSlider[i].maxValue = playerStats[i].expToNextLever[playerStats[i].playerLever];
                expSlider[i].value = playerStats[i].currentEXP;
                charImage[i].sprite = playerStats[i].charImage;
            }
            else
            {
                charStatsHolder[i].SetActive(false);
            }
        }
    }

    public void ToggleWindow(int windowNumber)
    {
        UpdateMainStats();
        for (int i = 0; i < windows.Length; i++)
        {
            if (i == windowNumber)
            {
                windows[i].SetActive(!windows[i].activeInHierarchy);
            }
            else
            {

                windows[i].SetActive(false);
            }
        }
    }
    public void CloseMenu()
    {
        for (int i = 0; i < windows.Length; i++)
        {
            windows[i].SetActive(false);
        }

        theMenu.SetActive(false);
        GameManager.instance.gameMenuOpen = false;
    }

    public void OpenStatus()
    {
        UpdateMainStats();


        statusChar(0);

        for (int i = 0;i < statusButtons.Length; i++)
        {
            statusButtons[i].SetActive(playerStats[i].gameObject.activeInHierarchy);
            statusButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = playerStats[i].charName;
        }
    }

    public void statusChar(int selected)
    {
        statusName.text = "Name: "+ playerStats[selected].charName;
        statusHP.text = "HP : " + playerStats[selected].currentEXP + "/" + playerStats[selected].maxHP;
        statusMP.text ="MP : "+ playerStats[selected].currentMP + "/" + playerStats[selected].maxMP;
        statusLV.text ="LV : " + playerStats[selected].playerLever.ToString();
        statusStr.text = "Strength : " + playerStats[selected].strength.ToString();
        statusDef.text = "Defence : " + playerStats[selected].defence.ToString();
        if (playerStats[selected].equippedWpn != "") 
        {
            statusWpnEqpd.text = "equippedWpn : "+  playerStats[selected].equippedWpn;
        }
        statusArmrPwr.text = "Armor : " + playerStats[selected].armrPwr.ToString();

        statusEXP.text = "EXP : " + (playerStats[selected].expToNextLever[playerStats[selected].playerLever] - playerStats[selected].currentEXP).ToString();

        statusImage.sprite = playerStats[selected].charImage;
    }
}

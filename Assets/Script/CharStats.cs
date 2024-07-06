using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharStats : MonoBehaviour
{

    public string charName;
    public int playerLever = 1;
    public int currentEXP;
    public int[] expToNextLever;
    public int maxLevel = 100;
    public int baseEXP = 1000;



    public int currentHP;
    public int maxHP = 100;
    public int currentMP;
    public int maxMP =30;
    public int[] mpLVBonus;
    public int strength;
    public int defence;
    public int wsnPwr;
    public int armrPwr;
    public string equippedWpn;
    public string equippedArmr;
    public Sprite charImage;
    // Start is called before the first frame update
    void Start()
    {
        expToNextLever = new int[maxLevel];
        expToNextLever[1] = baseEXP;

        for(int i = 2; i< expToNextLever.Length; i++)
        {
            expToNextLever [i] = Mathf.FloorToInt(expToNextLever [i-1] * 1.05f);
        }
    }

    // Update is called once per frame
    void Update()
    {
     if(Input.GetKeyDown(KeyCode.K))
        {
            addEXP(500);
        }   
    }

    public void addEXP(int expToAdd)
    {
        currentEXP += expToAdd;
        if (playerLever < maxLevel)
        {
            if (currentEXP > expToNextLever[playerLever])
            {
                currentEXP -= expToNextLever[playerLever];
                playerLever++;

                if (playerLever % 2 == 0)
                {
                    strength++;
                }
                else
                {
                    defence++;
                }
                maxHP = Mathf.FloorToInt(maxHP * 1.05f);
                currentEXP = maxHP;

                maxHP += mpLVBonus[playerLever];
                currentMP = maxHP;
            }
        }
        else
        {
            currentEXP =0;
        }
    }
}

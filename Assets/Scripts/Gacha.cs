using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class Gacha : MonoBehaviour
{
    public GameObject GachaUI;
    public TMP_Text WeaponObtainedUI;

    [SerializeField]
    public List<GameObject> weapons;                // list of all weapons

    [SerializeField]
    public int[] table = {500, 300, 160, 40};       // total weight of each rarity
    public string[] tableName = {"Sword", "Axe", "Bow", "Gun"};     // weapon name of the according rarity above
    public List<Item> Items;

    public int totalWeight;
    public int randomNumber;

    private void Start()
    {
        GachaUI.SetActive(false);
        // calculate total weight of loot table
        foreach(var item in table)
        {
            totalWeight += item;
        }
    }

    public void StartGacha()
    {
        print("interact key was pressed");
        // generate random number
        randomNumber = Random.Range(0, totalWeight);
        // compare random number to loot table weight
        for(int i = 0; i < table.Length; i++)
        {
            // compare random number to the [i] weight in loot table, if smaller give [i] item
            if(randomNumber <= table[i])
            {
                //weapons[i].SetActive(true);
                GachaUI.SetActive(true);
                if (table[i] == 500)
                {
                    WeaponObtainedUI.text = "Common " + tableName[i].ToUpper();
                    WeaponObtainedUI.color = Color.white;
                }
                if (table[i] == 300)
                {
                    WeaponObtainedUI.text = "Rare " + tableName[i].ToUpper();
                    WeaponObtainedUI.color = Color.blue;
                }
                if (table[i] == 160)
                {
                    WeaponObtainedUI.text = "Epic " + tableName[i].ToUpper();
                    WeaponObtainedUI.color = Color.magenta;
                }
                if (table[i] == 40)
                {
                    WeaponObtainedUI.text = "Legendary " + tableName[i].ToUpper();
                    WeaponObtainedUI.color = Color.red;
                }
                Inventory.instance.Add(Items[i]);
                Debug.Log("Award: " + table[i] + tableName[i]);
                return;
            }else
            {
                // if random number is bigger than previous weight, subtract it with weight and compare the product to next weight in table
                randomNumber -= table[i];
            }
        }
    }

}

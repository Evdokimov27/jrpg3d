using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BLINK.RPGBuilder.Managers;
using BLINK.RPGBuilder.Character;
using BLINK.RPGBuilder.Logic;
using BLINK.RPGBuilder.LogicMono;
using BLINK.RPGBuilder.UIElements;
using BLINK.RPGBuilder.World;

namespace BLINK.RPGBuilder.Managers
{
    public class UpgradeHub : MonoBehaviour
    {
        [SerializeField]
        public Station[] UpgradeStation;


        void Update()
        {
            for (int i = 0; i < UpgradeStation.Length; i++)
            {
                if (UpgradeStation[i].buyed == true)
                {
                    UpgradeStation[i].buyingStation.active = true;
                }
                else
                {
                    UpgradeStation[i].buyingStation.active = false;
                }
            }
        }
        public void Buying(int station)
        {
            UpgradeStation[station].buyed = true;
            
        }

        public void BuyThisItem(int ThisStation)
        {
            InventoryManager.Instance.BuyStation(UpgradeStation[ThisStation].ThisCurrency, UpgradeStation[ThisStation].ThisCost, ThisStation);
            
        }     

    }
}

[System.Serializable]
public class Station 
{
    public GameObject buyingStation;
    public bool buyed;
    public RPGCurrency ThisCurrency;
    public int ThisCost;
}

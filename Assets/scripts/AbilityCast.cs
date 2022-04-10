using BLINK.RPGBuilder.LogicMono;
using BLINK.RPGBuilder.Managers;
using BLINK.RPGBuilder.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BLINK.RPGBuilder.Managers
{

    public class AbilityCast : MonoBehaviour
    {
        [SerializeField] public int[] abilityID;
        CombatNode caster;
        [SerializeField] float[] cooldown;
        [SerializeField] public GameObject target;
        
        
        int ID;

        // Start is called before the first frame update
        public void UseAbility()
        {
            CombatManager.Instance.InitAbility(CombatManager.playerCombatNode, RPGBuilderUtilities.GetAbilityFromID(abilityID[ID]), false);
        }
        private void Start()
        {
            target = GameObject.Find("Target_Nameplate");
        }

        // Update is called once per frame
        void Update()
        {
            if (target.GetComponent<CanvasGroup>().alpha == 1)
            {
                if (GetComponent<ExampleGestureHandler>().ID_Draw == "ILIYA")
                {
                    ID = 0;
                    if (RPGBuilderUtilities.isAbilityKnown(abilityID[ID]))
                    {
                        Debug.Log("fireball");
                        if (cooldown[ID] <= 0)
                        {
                            UseAbility();
                            cooldown[ID] = 3;
                        }
                        GetComponent<ExampleGestureHandler>().ID_Draw = null;
                    }
                    else
                        GetComponent<ExampleGestureHandler>().ID_Draw = null;
                }
                if (GetComponent<ExampleGestureHandler>().ID_Draw == "horizontal")
                {
                    ID = 1;
                    if (RPGBuilderUtilities.isAbilityKnown(abilityID[ID]))
                    {
                        Debug.Log("coldbolt");
                        if (cooldown[ID] <= 0)
                        {
                            UseAbility();
                            cooldown[ID] = 5;
                        }
                        GetComponent<ExampleGestureHandler>().ID_Draw = null;
                    }
                    else
                        GetComponent<ExampleGestureHandler>().ID_Draw = null;
                }
                if (GetComponent<ExampleGestureHandler>().ID_Draw == "heart")
                {
                    ID = 2;
                    if (RPGBuilderUtilities.isAbilityKnown(abilityID[ID]))
                    {
                        Debug.Log("barier");
                        if (cooldown[ID] <= 0)
                        {
                            UseAbility();
                            cooldown[ID] = 5;
                        }
                        GetComponent<ExampleGestureHandler>().ID_Draw = null;
                    }
                    else
                        GetComponent<ExampleGestureHandler>().ID_Draw = null;
                }
                if (GetComponent<ExampleGestureHandler>().ID_Draw == "bolt")
                {
                    ID = 3;
                    if (RPGBuilderUtilities.isAbilityKnown(abilityID[ID]))
                    {
                        Debug.Log("coldbolt");
                        if (cooldown[ID] <= 0)
                        {
                            UseAbility();
                            cooldown[ID] = 5;
                        }
                        GetComponent<ExampleGestureHandler>().ID_Draw = null;
                    }
                    else
                        GetComponent<ExampleGestureHandler>().ID_Draw = null;
                }
            }
            else ErrorEventsDisplayManager.Instance.ShowErrorEvent("Для заклинания нужна цель", 3);
            for (int cd_id = 0; cd_id < abilityID.Length + 1; cd_id++)
            {
                if (cooldown[cd_id] > 0)
                {
                    cooldown[cd_id] -= Time.deltaTime;
                }
            }
        }
       
    }
}
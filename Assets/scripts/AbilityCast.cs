using BLINK.RPGBuilder.LogicMono;
using BLINK.RPGBuilder.Managers;
using BLINK.RPGBuilder.UI;
using System.Collections;
using System.Windows;
using System.Collections.Generic;
using UnityEngine;
using System;
using GestureRecognizer;
using System.Linq;

namespace BLINK.RPGBuilder.Managers
{
    public class AbilityCast : MonoBehaviour
    {
        CombatNode caster;
        [SerializeField] public int amountRuns;
        [SerializeField] public GameObject target;
        [SerializeField] public GameObject PAINTER;
        [SerializeField] public Ability[] AbilityCasts;
        public KeyCode UseAbilityKey;
        public List <GesturePattern> rune;
        public GameObject[] IconCombo;
        public GesturePattern[] Runs;

        private void Start()
        {
            rune.Clear();
            target = GameObject.Find("Target_Nameplate");
        }
        void Update()
        {
            for (int h = 0; h < amountRuns; h++)
            {
                if (IconCombo[h].GetComponent<GesturePatternDraw>().pattern == null)
                {
                    IconCombo[h].active = false;
                }
                else IconCombo[h].active = true;
            }

            UseAbilityKey = RPGBuilderUtilities.GetCurrentKeyByActionKeyName("USE_ABILITY_KEY");
            PAINTER = GameObject.FindWithTag("PAINTER");
            if (PAINTER.GetComponent<Canvas>().enabled == true)
            {
                if (Input.GetKeyDown(KeyCode.J))
                {
                    rune.Clear();
                    for (int h = 0; h < amountRuns; h++)
                    {
                        IconCombo[h].GetComponent<GesturePatternDraw>().pattern = null;
                    }
                }

                if (Input.GetKeyDown(KeyCode.K))
                {
                    for (int i = 0; i < AbilityCasts.Length; i++)
                    {
                        for (int j = 0; j < AbilityCasts[i].need_rune.Length; j++)
                            {
                            if (GetComponent<ExampleGestureHandler>().ID_Draw == AbilityCasts[i].need_rune[j].id)
                            {
                                rune.Add(AbilityCasts[i].need_rune[j]);
                                Runs[0] = AbilityCasts[i].need_rune[j];
                            }
                            
                        }
                    }
                    for (int h = 0; h < amountRuns; h++)
                    {
                        IconCombo[rune.Count-1].GetComponent<GesturePatternDraw>().pattern = Runs[0];
                    }
                }
            }
            if (Input.GetKeyDown(UseAbilityKey))
            {
                
                for (int i = 0; i < AbilityCasts.Length; i++)
                {
                    bool isEqual = rune.SequenceEqual(AbilityCasts[i].need_rune);
                    if ((isEqual == true) && (AbilityCasts[i].cooldown_now <= 0))
                        {
                            CombatManager.Instance.InitAbility(CombatManager.playerCombatNode, RPGBuilderUtilities.GetAbilityFromID(AbilityCasts[i].CastAbility.ID), false);
                            rune.Clear();
                            AbilityCasts[i].cooldown_now = AbilityCasts[i].cooldown_skill;
                        }
                        else Debug.Log("Такого заклинания нет");
                        Debug.Log(isEqual);
                }
            }
            
          
            for (int cd_id = 0; cd_id < AbilityCasts.Length + 1; cd_id++)
            {
                if (AbilityCasts[cd_id].cooldown_now > 0)
                {
                    AbilityCasts[cd_id].cooldown_now -= Time.deltaTime;
                }
            }
        } 
    }
}
[System.Serializable]
public class Ability
{
    public GesturePattern[] need_rune;
    public RPGAbility CastAbility;
    public float cooldown_now;
    public float cooldown_skill;
}


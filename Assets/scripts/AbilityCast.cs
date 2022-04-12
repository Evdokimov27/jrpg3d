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
        [SerializeField] public GameObject target;
        [SerializeField] public GameObject PAINTER;
        [SerializeField] public Ability[] AbilityCasts;
        public KeyCode UseAbilityKey;
        public List<GesturePattern> rune;

        private void Start()
        {
            target = GameObject.Find("Target_Nameplate");
        }
        void Update()
        {
            UseAbilityKey = RPGBuilderUtilities.GetCurrentKeyByActionKeyName("USE_ABILITY_KEY");
            PAINTER = GameObject.FindWithTag("PAINTER");
            if (PAINTER.GetComponent<Canvas>().enabled == true)
            {
                if (Input.GetKeyDown(KeyCode.J))
                {
                    rune.Clear();
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
                            }
                        }
                    }
                }
            }
            if (Input.GetKeyDown(UseAbilityKey))
            {
                for (int i = 0; i < AbilityCasts.Length; i++)
                {
                    if (rune.SequenceEqual(AbilityCasts[i].need_rune) && (AbilityCasts[i].cooldown_now <= 0))
                    {
                        CombatManager.Instance.InitAbility(CombatManager.playerCombatNode, RPGBuilderUtilities.GetAbilityFromID(AbilityCasts[i].CastAbility.ID), false);
                        rune.Clear();
                        AbilityCasts[i].cooldown_now = AbilityCasts[i].cooldown_skill;
                    }
                    else Debug.Log("Такого заклинания нет");
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
    public GesturePattern pattern;
    public RPGAbility CastAbility;
    public float cooldown_now;
    public float cooldown_skill;
}


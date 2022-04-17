using BLINK.RPGBuilder.LogicMono;
using BLINK.RPGBuilder.Managers;
using BLINK.RPGBuilder.UI;
using System.Collections;
using System.Windows;
using System.Collections.Generic;
using UnityEngine;
using BayatGames.SaveGameFree;
using System;
using GestureRecognizer;
using System.Linq;
using UnityEngine.UI;

namespace AbilityCast
{
    public class AbilityCast : MonoBehaviour
    {
        CombatNode caster;
        [Header("-----Изучение рун-----")]
        [SerializeField] public List<GesturePattern> isSearch;
        [SerializeField] public GesturePattern selectRune;
        [Header("-----Заклинания-----")]
        
        [SerializeField] public int amountRuns;
        [SerializeField] public float TimeAdd;
        [SerializeField] public Ability[] AbilityCasts;
        [Header("-----Разное-----")]
        [SerializeField] public GameObject target;
        [SerializeField] public GameObject PAINTER;
        [SerializeField] public List<GameObject> PAINTER_Icon;

        public Text ResultText;
        public List <GesturePattern> rune;
        public GesturePattern element;
        public GameObject[] IconCombo;
        public GameObject IconElement;
        public GesturePattern[] Runs;
        public CharacterData characterData;
        [Header("-----Управление-----")]
        public KeyCode UseAbilityKey;


        private void Start()
        {           
            IconElement.GetComponent<GesturePatternDraw>().pattern = null;
            rune.Clear();
            target = GameObject.Find("Target_Nameplate");
            ResultText.text = null;
            GetComponent<ExampleGestureHandler>().ID_Draw = null;
        }

        public void SearchRune()
        {
            for (int i = 0; i < isSearch.Count; i++)
            {
                for (int j = 0; j < PAINTER_Icon.Count; j++)
                {
                    Debug.Log(PAINTER_Icon[j].GetComponent<GesturePatternDraw>().pattern.id + " " + isSearch[i].id );
                    if (isSearch[i].id == PAINTER_Icon[j].GetComponent<GesturePatternDraw>().pattern.id)
                    {
                        PAINTER_Icon[j].SetActive(true);
                    }
                    else PAINTER_Icon[j].SetActive(false);
                }
            }
        }

        public void Save()
        {
            if (selectRune != null)
            { 
            isSearch.Add(selectRune);
                selectRune = null;
            }

            SaveGame.Save<List<GesturePattern>>(characterData.CharacterName + "isSearch", isSearch);
        }
        
        void Update()
        {
           SearchRune();
            if (selectRune != null)
            {
                Save();
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                isSearch.Clear();
                SaveGame.Save<List<GesturePattern>>(characterData.CharacterName + "isSearch", isSearch);
            }


            if (characterData.CharacterName == null)
            {
                isSearch.Clear();
            }
            isSearch = SaveGame.Load<List<GesturePattern>>(characterData.CharacterName + "isSearch");

            


          

            for (int h = 0; h < amountRuns; h++)
            {
                if (IconCombo[h].GetComponent<GesturePatternDraw>().pattern == null)
                {
                    IconCombo[h].active = false;
                   
                }
                else
                {
                    IconCombo[h].active = true;
                }


                if (IconElement.GetComponent<GesturePatternDraw>().pattern == null)
                {
                    IconElement.active = false;
                }
                else
                {
                    IconElement.active = true;
                }
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

                if (Input.GetKey(KeyCode.Mouse0))
                {
                    TimeAdd = 5;
                }

                if (TimeAdd > 0)
                {
                    TimeAdd -= Time.deltaTime;
                    if (TimeAdd <= 0)
                    {

                        for (int i = 0; i < AbilityCasts.Length; i++)
                        {
                            for (int j = 0; j < AbilityCasts[i].need_rune.Length; j++)
                            {
                                if (GetComponent<ExampleGestureHandler>().ID_Draw == AbilityCasts[i].need_element.id)
                                {
                                    element = AbilityCasts[i].need_element;
                                    IconElement.GetComponent<GesturePatternDraw>().pattern = element;
                                }
                                if (GetComponent<ExampleGestureHandler>().ID_Draw == AbilityCasts[i].need_rune[j].id)
                                {
                                    rune.Add(AbilityCasts[i].need_rune[j]);
                                    Runs[0] = AbilityCasts[i].need_rune[j];
                                    IconCombo[rune.Count - 1].GetComponent<GesturePatternDraw>().pattern = Runs[0];
                                    ResultText.text = null;
                                    GetComponent<ExampleGestureHandler>().ID_Draw = null;
                                }




                            }
                        }
                    }
                }
            }
            if (Input.GetKeyDown(UseAbilityKey))
            {

                for (int i = 0; i < AbilityCasts.Length; i++)
                {
                    bool isEqual = rune.SequenceEqual(AbilityCasts[i].need_rune);
                    if ((AbilityCasts[i].need_element == element) && (isEqual == true) && (AbilityCasts[i].cooldown_now <= 0))
                    {
                        CombatManager.Instance.InitAbility(CombatManager.playerCombatNode, RPGBuilderUtilities.GetAbilityFromID(AbilityCasts[i].CastAbility.ID), false);
                        rune.Clear();
                        for (int h = 0; h < amountRuns; h++)
                        {
                            IconCombo[h].GetComponent<GesturePatternDraw>().pattern = null;
                        }
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
    public GesturePattern need_element;
    public GesturePattern[] need_rune;
    public RPGAbility CastAbility;
    public float cooldown_now;
    public float cooldown_skill;
}



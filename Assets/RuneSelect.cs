using System;
using System.Collections.Generic;
using BLINK.RPGBuilder.LogicMono;
using BLINK.RPGBuilder.UIElements;
using UnityEngine;
using GestureRecognizer;
using AbilityCast;

namespace BLINK.RPGBuilder.Managers
{
    public class RuneSelect : ScriptableObject
    {
        public static RuneSelect Instance;

            public void Rune(CombatNode cbtNode)
             {
            Debug.Log(cbtNode.npcDATA.pattern);
            AbilityCast.AbilityCast abilityCast = new AbilityCast.AbilityCast();
            abilityCast.isSearch[abilityCast.isSearch.Count-1] = cbtNode.npcDATA.pattern;
            Debug.Log("Give" + cbtNode.npcDATA.pattern);
            }


    }
}
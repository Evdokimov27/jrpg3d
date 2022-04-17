using System;
using System.Collections.Generic;
using BLINK.RPGBuilder.LogicMono;
using BLINK.RPGBuilder.UIElements;
using UnityEngine;
using GestureRecognizer;
using BayatGames.SaveGameFree;


public class RuneResearch : MonoBehaviour
{
    [Header("-----Изучаемая руна-----")]
    public GesturePattern Rune;
    [Header("-----Разное-----")]
    public GameObject[] Lighting;
    public GameObject ACast;

    public static RuneResearch Instance;

    public void OnTriggerEnter(Collider other)
    {
        ACast.GetComponent<AbilityCast>().selectRune = Rune;
        Rune = null;
    }

    private void Update()
    {
        ACast = GameObject.FindWithTag("AbilityCast");
        
        if (Rune == null)
        {
            for (int i = 0; i < Lighting.Length; i++)
            {
                Lighting[i].GetComponent<Light>().intensity -= Time.deltaTime / 2;
            }
        }
    }


}
using System.Collections;
using System.Collections.Generic;
using BLINK.RPGBuilder.LogicMono;
using BLINK.RPGBuilder.Managers;
using BLINK.Controller;
using UnityEngine;
using MouseSpeedSwitcher;
using System.Runtime.InteropServices;
using System;



    public class AbilityBook : MonoBehaviour
    {
        public RPGBThirdPersonController controller;
        public float speed;
        public string mouseSpeed = "10";
        [SerializeField] public GameObject player;
        [SerializeField] public GameObject PAINTER;
        KeyCode SpellKey = RPGBuilderUtilities.GetCurrentKeyByActionKeyName("CAST_SPELL_BOOK");
    // Start is called before the first frame update

    void Start()
        {
        
        speed = 1;
            if (PAINTER = GameObject.FindWithTag("PAINTER"))
            {
                PAINTER.GetComponent<Canvas>().enabled = false;
            }
            player = GameObject.FindWithTag("Player");
        }

        
    
        public static AbilityBook abilityBook;
    // Update is called once per frame
     
    void Update()
        {
        
        player = GameObject.FindWithTag("Player");
            PAINTER = GameObject.FindWithTag("PAINTER");
        if (GameOptionsDisplayManager.Instance.showing == false)
        {
            if (Input.GetKeyDown(SpellKey))
            {
                PAINTER.GetComponent<Canvas>().enabled = !PAINTER.GetComponent<Canvas>().enabled;
                if (PAINTER.GetComponent<Canvas>().enabled == true)
                {
                    
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                    speed = 0.2f;
                    mouseSpeed = "2";
                    GetComponent<RPGBThirdPersonCharacterControllerEssentials>().SetCameraAiming(true);  
                }
                else
                {
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                    speed = 1;
                    mouseSpeed = "10";
                    GetComponent<RPGBThirdPersonCharacterControllerEssentials>().SetCameraAiming(false);
                }
            }
        }
        if (GameOptionsDisplayManager.Instance.showing == true)
        {
            PAINTER.GetComponent<Canvas>().enabled = false;
            GetComponent<RPGBThirdPersonCharacterControllerEssentials>().SetCameraAiming(false);
            speed = 1;
            mouseSpeed = "10";
        }
    }
    }

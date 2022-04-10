using System.Collections;
using System.Collections.Generic;
using BLINK.RPGBuilder.LogicMono;
using BLINK.RPGBuilder.Managers;
using BLINK.Controller;
using UnityEngine;



public class AbilityBook : MonoBehaviour
{
        public float speed;
        [SerializeField] public GameObject PAINTER;
        KeyCode SpellKey = RPGBuilderUtilities.GetCurrentKeyByActionKeyName("CAST_SPELL_BOOK");
        // Start is called before the first frame update
        void Start()
        {
        speed = 1;
        PAINTER.GetComponent<Canvas>().enabled = false;
        }

        // Update is called once per frame
        void Update()
        {
            PAINTER = GameObject.FindWithTag("PAINTER");
            if (Input.GetKeyDown(SpellKey))
            {
                PAINTER.GetComponent<Canvas>().enabled = !PAINTER.GetComponent<Canvas>().enabled;
                if (PAINTER.GetComponent<Canvas>().enabled == true)
                {
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.Confined;
                    speed = 0.2f;
                }
                else
                {
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                    speed = 1;
            }
            }
        }
    }


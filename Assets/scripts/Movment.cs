using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using BLINK.RPGBuilder.Character;

namespace BLINK.Controller
{
    public class Movment : MonoBehaviour
    {
        public Transform goal;
        public RPGBThirdPersonController ThirdPersonController;
        public CharacterController controller_player;
        public NavMeshAgent agent_player;
        public Transform player;
        public GameObject move_point;
        public GameObject PAINTER;
        public float speed = 1f;
        [SerializeField] public int rot = 0;
        [SerializeField] public GameObject cube;
        KeyCode SpellKey = RPGBuilderUtilities.GetCurrentKeyByActionKeyName("CAST_SPELL_BOOK");
        private void Start()
        {
            controller_player = GetComponent<CharacterController>();
            agent_player = GetComponent<NavMeshAgent>();
            ThirdPersonController = GetComponent<RPGBThirdPersonController>();
            player = GameObject.FindWithTag("Player").transform;
            PAINTER = GameObject.FindWithTag("PAINTER");
        }


        void Update()
        {
                if (Input.GetKeyDown(SpellKey))
                {
                PAINTER.GetComponent<Canvas>().enabled = !PAINTER.GetComponent<Canvas>().enabled;
                }

            goal = GameObject.FindWithTag("end_move").transform;
            UnityEngine.AI.NavMeshAgent agent
                = GetComponent<UnityEngine.AI.NavMeshAgent>();
            agent.destination = goal.position;
            if (rot == 1)
            { 
                GetComponent<RPGBThirdPersonController>().RotationSettings.UseControlRotation = true;
                player.rotation = Quaternion.Euler(0, 180, 0);
                //Debug.Log(player.eulerAngles.y);
                if (player.eulerAngles.y == 180)
                {
                    rot = 0;
                }   
            }
        }


        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "move")
            {
                GetComponent<RPGBThirdPersonController>().RotationSettings._orientRotationToMovement = false;
                controller_player.enabled = false;
                agent_player.enabled = true;
                move_point = goal.gameObject;
            }
            else if (other.gameObject.tag == "end_move")
            {
                GetComponent<RPGBThirdPersonController>().RotationSettings._orientRotationToMovement = true;
                GetComponent<RPGBThirdPersonController>().RotationSettings.UseControlRotation = true;
                cube.SetActive(true);
                rot = 1;
                agent_player.enabled = false;     
                controller_player.enabled = true;

            }
        }

    }
}

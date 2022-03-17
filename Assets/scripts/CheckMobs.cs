using BLINK.Controller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
namespace BLINK.RPGBuilder.AI
{
    public class CheckMobs : MonoBehaviour
    {
        
        [SerializeField] public RPGBThirdPersonController ThirdPersonController;
        [SerializeField] public GameObject cube;
        [SerializeField] public GameObject obj;
        public CharacterController controller_player;
        [SerializeField]public GameObject move;
        public NavMeshAgent agent_player;

        private void Start()
        {
            
        }
        void Update()
        {
            if (obj.GetComponent<CapsuleCollider>() == null)
            {
                ThirdPersonController.RotationSettings.OrientRotationToMovement = true;
                controller_player.enabled = true;
                cube.SetActive(false);
                Debug.Log("Мобов нет");
                Destroy(move);
            }

        }
       
        private void OnTriggerStay(Collider other)
        {
                if (other.GetComponent<CapsuleCollider>() != null)
                {
                    obj = other.gameObject;
                    ThirdPersonController.RotationSettings.OrientRotationToMovement = false;
                    agent_player.enabled = false;
                    controller_player.enabled = false;
            }
            Debug.Log("Мобы есть");
        }
    }
}

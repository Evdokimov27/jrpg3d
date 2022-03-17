using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NextMove : MonoBehaviour
{
    public NavMeshAgent agent_player;
    private void Start()
    {

    }
    // Start is called before the first frame update
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("работает");
            Destroy(this.gameObject);
        }
    }
}

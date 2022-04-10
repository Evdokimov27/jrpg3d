using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewBehaviourScript : MonoBehaviour
{
    private GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            SceneManager.LoadScene(3);
            player.transform.position = new Vector3 (0, 0, 0);
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            SceneManager.LoadScene(2);
            player.transform.position = new Vector3(0, 0, 0);
        }
    }
}

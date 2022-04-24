using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BLINK.RPGBuilder.Managers;
using UnityEngine.SceneManagement;

public class NewBehaviourScript : MonoBehaviour
{
    private GameObject player;
    public int SlotIndex;
    [SerializeField]RPGItem RequirementItem;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < CharacterData.Instance.inventoryData.baseSlots.Count; i++)
            if (CharacterData.Instance.inventoryData.baseSlots[i].itemID == RequirementItem.ID)
            {
                SlotIndex = i;
            }

        

        if (Input.GetKeyDown(KeyCode.M) && (InventoryManager.Instance.isItemOwned(RequirementItem.ID, 2)))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            ErrorEventsDisplayManager.Instance.ShowErrorEvent("Сцена изменена на " + SceneManager.GetActiveScene().name, 3);
            CharacterData.Instance.position = new Vector3(0, 0, 0);
            InventoryManager.Instance.RemoveItem(RequirementItem.ID, 2, 0, SlotIndex, true);
           
        }
        else if (Input.GetKeyDown(KeyCode.M) && !InventoryManager.Instance.isItemOwned(RequirementItem.ID, 2))
            ErrorEventsDisplayManager.Instance.ShowErrorEvent("Нет предмета для использования", 3);


        if (Input.GetKeyDown(KeyCode.N))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
            player.transform.position = new Vector3(0, 0, 0);

        }
    }
}

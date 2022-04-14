using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TestCursor: MonoBehaviour
{
    [SerializeField] public int speed;
    public void Update()
    {
        Debug.Log(this.transform.position);
        if (Input.GetKey(KeyCode.Mouse0))
        {
            //Get the mouse
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos = new Vector3(mousePos.x, mousePos.y, 0);

            //Rotate the sprite to the mouse point
            Vector3 diff = mousePos - transform.position;
            diff.Normalize();
            float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);
            transform.position += transform.up * speed * Time.deltaTime;
        }
    }
}

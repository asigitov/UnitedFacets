using UnityEngine;
using System.Collections;

public class LeftRightTopDown : MonoBehaviour
{

    public float speed = 1.0f;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            gameObject.transform.position += new Vector3(Time.deltaTime * -speed, 0, 0);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            gameObject.transform.position += new Vector3(Time.deltaTime * speed, 0, 0);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            gameObject.transform.position += new Vector3(0, Time.deltaTime * -speed, 0);
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            gameObject.transform.position += new Vector3(0, Time.deltaTime * speed, 0);
        }
    }
}

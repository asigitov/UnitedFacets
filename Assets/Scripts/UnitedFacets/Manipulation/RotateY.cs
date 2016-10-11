using UnityEngine;
using System.Collections;

public class RotateY : MonoBehaviour {

    public float speed = 1.0f;
	
	// Update is called once per frame
	void Update () {
        gameObject.transform.Rotate(0, Time.deltaTime * speed, 0);
	}
}

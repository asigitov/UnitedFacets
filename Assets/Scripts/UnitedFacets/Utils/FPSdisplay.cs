using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FPSdisplay : MonoBehaviour
{

    public Text label;
    float deltaTime;
    bool visible;
    float fps;

    void Start()
    {
        deltaTime = 0.0f;
        visible = label.gameObject.activeSelf;
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.F))
        {
            visible = !visible;
            label.gameObject.SetActive(visible);
        }

        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        if (visible & Time.frameCount % 1 == 0)
        {
            fps = (1.0f / deltaTime);
            label.text = string.Format("FPS: {0}", fps.ToString());
        }
    }
}

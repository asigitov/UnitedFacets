using UnityEngine;
using System.Collections;

public class PS3CameraController : MonoBehaviour {

    // Keyboard axes buttons in the same order as Unity
    public enum JoystickAxis { Horizontal = 0, Vertical = 1, Pitch = 2, Yaw = 3, None = 5 }

    
    [System.Serializable]
    // Handles common parameters for translations and rotations
    public class JoystickControlConfiguration
    {

        public bool activate;
        public JoystickAxis joystickAxis;
        public float sensitivity;

        public bool isActivated()
        {
            return activate && joystickAxis != JoystickAxis.None;
        }
    }

    // Yaw default configuration
    public JoystickControlConfiguration yaw = new JoystickControlConfiguration { joystickAxis = JoystickAxis.Yaw, sensitivity = 1F };

    // Pitch default configuration
    public JoystickControlConfiguration pitch = new JoystickControlConfiguration { joystickAxis = JoystickAxis.Pitch, sensitivity = 1F };

    // Roll default configuration
    public JoystickControlConfiguration roll = new JoystickControlConfiguration { joystickAxis = JoystickAxis.Horizontal, sensitivity = 1F };

    // Vertical translation default configuration
    public JoystickControlConfiguration verticalTranslation = new JoystickControlConfiguration { joystickAxis = JoystickAxis.Vertical, sensitivity = 0.5F };

    // Horizontal translation default configuration
    public JoystickControlConfiguration horizontalTranslation = new JoystickControlConfiguration { joystickAxis = JoystickAxis.Horizontal, sensitivity = 0.5F };

    // Depth (forward/backward) translation default configuration
    public JoystickControlConfiguration depthTranslation = new JoystickControlConfiguration { joystickAxis = JoystickAxis.Vertical, sensitivity = 0.5F };

    // Default unity names for keyboard axes
    public string joystickHorizontalAxisName = "Horizontal";
    public string joystickVerticalAxisName = "Vertical";
    public string joystickPitchAxisName = "Pitch";
    public string joystickYawAxisName = "Yaw";


    private string[] joystickAxesNames;

    void Start()
    {
        joystickAxesNames = new string[] { joystickHorizontalAxisName, joystickVerticalAxisName, joystickPitchAxisName, joystickYawAxisName };
    }


    // LateUpdate  is called once per frame after all Update are done
    void Update()
    {
        
        if (yaw.isActivated())
        {
            float rotationX = Input.GetAxis("Yaw") * yaw.sensitivity;
            transform.Rotate(0, rotationX, 0);
        }
        if (pitch.isActivated())
        {
            float rotationY = Input.GetAxis("Pitch") * pitch.sensitivity;
            transform.Rotate(-rotationY, 0, 0);
        }
        if (roll.isActivated())
        {
            float rotationZ = Input.GetAxis(joystickAxesNames[(int)roll.joystickAxis]) * roll.sensitivity;
            transform.Rotate(0, 0, rotationZ);
        }
        if (verticalTranslation.isActivated())
        {
            float translateY = Input.GetAxis(joystickAxesNames[(int)verticalTranslation.joystickAxis]) * verticalTranslation.sensitivity;
            transform.Translate(0, translateY, 0);
        }
        if (horizontalTranslation.isActivated())
        {
            float translateX = Input.GetAxis(joystickAxesNames[(int)horizontalTranslation.joystickAxis]) * horizontalTranslation.sensitivity;
            transform.Translate(translateX, 0, 0);
        }
        if (depthTranslation.isActivated())
        {
            float translateZ = Input.GetAxis(joystickAxesNames[(int)depthTranslation.joystickAxis]) * depthTranslation.sensitivity;
            transform.Translate(0, 0, translateZ);
        }


    }
}

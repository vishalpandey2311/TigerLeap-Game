using UnityEngine;

namespace StretchSense.OSCBridge
{
    [System.Serializable]
    public class ControllerInput
    {
        public int idle;
        public int grab_pressed;
        public float grab_value;
        public int button1;
        public int button2;
        public int trigger_pressed;
        public float trigger_value;
        public int menu_pressed;
        public float joystick_x;
        public float joystick_y;
    }
}
using UnityEngine;
using System.Collections.Generic;
using extOSC;


namespace StretchSense.OSCBridge
{
    public class StretchSenseOSCReceiver : MonoBehaviour
    {
        [Header("OSC Settings")]
        public string inputAddress = "127.0.0.1";
        public int inputPort = 9002;
        private OSCReceiver receiver;

        private readonly string[] expectedJointNames =
        {
            "palm", "hand",
            "thumb_cmc", "thumb_mcp", "thumb_dip", "thumb_tip",
            "index_cmc", "index_mcp", "index_pip", "index_dip", "index_tip",
            "middle_cmc", "middle_mcp", "middle_pip", "middle_dip", "middle_tip",
            "ring_cmc", "ring_mcp", "ring_pip", "ring_dip", "ring_tip",
            "pinky_cmc", "pinky_mcp", "pinky_pip", "pinky_dip", "pinky_tip"
        };

        private void Start()
        {
            receiver = gameObject.AddComponent<OSCReceiver>();
            receiver.LocalPort = inputPort;

            receiver.Bind("/v1/controller_input/all", OnControllerInput);
            receiver.Bind("/v1/orientation/all", OnOrientation);
            receiver.Bind("/v1/animation/kinematic/all", OnKinematic);
            receiver.Bind("/v1/config/tracker_offset_calibration/all", OnTrackerOffsetCalibration);
            receiver.Bind("/v1/config/set_tracker_offset/all", OnSetTrackerOffset);
            receiver.Bind("/v1/config/set_tracker_source/all", OnSetTrackerSource);
            receiver.Bind("/v1/config/set_tracker_location/all", OnSetTrackerLocation);
            receiver.Bind("/v1/config/enable_button_passthrough/all", OnEnableButtonPassthrough);

        }

        private float DenoiseFloat(float value, float threshold = 0.001f)
        {
            return Mathf.Abs(value) < threshold ? 0f : value;
        }

        private void OnKinematic(OSCMessage message)
        {
            int handedness = message.Values[2].IntValue;
            const int jointStartIndex = 5;
            const int jointSize = 7;
            int jointCount = (message.Values.Count - jointStartIndex) / jointSize;

            var jointList = new List<JointData>(jointCount);
            for (int i = 0; i < jointCount; i++)
            {
                int baseIndex = jointStartIndex + i * jointSize;
                jointList.Add(new JointData
                {
                    name = expectedJointNames[i],
                    position = new Vector3(
                        DenoiseFloat(-message.Values[baseIndex + 0].FloatValue),
                        DenoiseFloat(message.Values[baseIndex + 1].FloatValue),
                        DenoiseFloat(message.Values[baseIndex + 2].FloatValue)
                    ),
                    rotation = new Quaternion(
                        DenoiseFloat(-message.Values[baseIndex + 3].FloatValue),
                        DenoiseFloat(message.Values[baseIndex + 4].FloatValue),
                        DenoiseFloat(message.Values[baseIndex + 5].FloatValue),
                        DenoiseFloat(-message.Values[baseIndex + 6].FloatValue)
                    )
                });
            }

            HandStateManager.Instance.UpdateKinematic(handedness, jointList);
        }

        private void OnOrientation(OSCMessage message)
        {
            int handedness = message.Values[2].IntValue;
            Vector3 accelerometer = new Vector3(
                DenoiseFloat(message.Values[5].FloatValue),
                DenoiseFloat(message.Values[6].FloatValue),
                DenoiseFloat(message.Values[7].FloatValue)
            );
            Quaternion orientation = new Quaternion(
                DenoiseFloat(message.Values[8].FloatValue),
                DenoiseFloat(message.Values[9].FloatValue),
                DenoiseFloat(message.Values[10].FloatValue),
                DenoiseFloat(message.Values[11].FloatValue)
            );

            HandStateManager.Instance.UpdateOrientation(handedness, accelerometer, orientation);
        }

        private void OnControllerInput(OSCMessage message)
        {
            int handedness = message.Values[2].IntValue;

            var input = new ControllerInput
            {
                idle = message.Values[5].IntValue,
                grab_pressed = message.Values[6].IntValue,
                grab_value = DenoiseFloat(message.Values[7].FloatValue),
                button1 = message.Values[8].IntValue,
                button2 = message.Values[9].IntValue,
                trigger_pressed = message.Values[10].IntValue,
                trigger_value = DenoiseFloat(message.Values[11].FloatValue),
                menu_pressed = message.Values[12].IntValue,
                joystick_x = DenoiseFloat(message.Values[13].FloatValue),
                joystick_y = DenoiseFloat(message.Values[14].FloatValue)
            };

            HandStateManager.Instance.UpdateControllerInput(handedness, input);
        }

        private void OnTrackerOffsetCalibration(OSCMessage message)
        {
            Debug.Log("[OSC] Tracker Offset Calibration triggered.");
        }

        private void OnSetTrackerOffset(OSCMessage message)
        {
            int handedness = message.Values[2].IntValue;
            Vector3 offset = new Vector3(
                DenoiseFloat(message.Values[5].FloatValue),
                DenoiseFloat(message.Values[6].FloatValue),
                DenoiseFloat(message.Values[7].FloatValue)
            );
            HandStateManager.Instance.SetTrackerOffset(handedness, offset);
        }

        private void OnSetTrackerSource(OSCMessage message)
        {
            int handedness = message.Values[2].IntValue;
            int source = message.Values[5].IntValue;
            HandStateManager.Instance.SetTrackerSource(handedness, source);
        }

        private void OnSetTrackerLocation(OSCMessage message)
        {
            int handedness = message.Values[2].IntValue;
            int location = message.Values[5].IntValue;
            HandStateManager.Instance.SetTrackerLocation(handedness, location);
        }

        private void OnEnableButtonPassthrough(OSCMessage message)
        {
            int handedness = message.Values[2].IntValue;
            bool enabled = message.Values[5].IntValue == 1;
            HandStateManager.Instance.SetButtonPassthrough(handedness, enabled);
        }
    }
}

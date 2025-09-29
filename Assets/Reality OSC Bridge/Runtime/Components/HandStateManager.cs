using UnityEngine;
using System.Collections.Generic;

namespace StretchSense.OSCBridge
{
    public class HandStateManager : MonoBehaviour
    {
        public static HandStateManager Instance { get; private set; }

        public HandData leftHand = new();
        public HandData rightHand = new();

        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(this);
            else
                Instance = this;
        }

        public HandData GetHand(int handedness)
        {
            return handedness == 1 ? leftHand : rightHand;
        }

        public void UpdateKinematic(int handedness, List<JointData> jointData)
        {
            GetHand(handedness).joints = jointData;
        }

        public void UpdateOrientation(int handedness, Vector3 accelerometer, Quaternion orientation)
        {
            var hand = GetHand(handedness);
            hand.accelerometer = accelerometer;
            hand.orientation = orientation;
        }

        public void UpdateControllerInput(int handedness, ControllerInput input)
        {
            GetHand(handedness).controller = input;
        }


        public void SetTrackerOffset(int handedness, Vector3 offset)
        {
            GetHand(handedness).trackerOffset = offset;
        }

        public void SetTrackerSource(int handedness, int source)
        {
            GetHand(handedness).trackerSource = source;
        }

        public void SetTrackerLocation(int handedness, int location)
        {
            GetHand(handedness).trackerLocation = location;
        }

        public void SetButtonPassthrough(int handedness, bool enabled)
        {
            GetHand(handedness).buttonPassthroughEnabled = enabled;
        }

    }
}

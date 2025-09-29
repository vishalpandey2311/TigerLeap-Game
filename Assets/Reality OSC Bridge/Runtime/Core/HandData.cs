using UnityEngine;
using System.Collections.Generic;

namespace StretchSense.OSCBridge
{
    [System.Serializable]
    public class HandData
    {
        public List<JointData> joints = new();
        public Vector3 accelerometer = Vector3.zero;
        public Quaternion orientation = Quaternion.identity;
        public ControllerInput controller = new();
        public Vector3 trackerOffset = Vector3.zero;
        public int trackerSource = 0;
        public int trackerLocation = 0;
        public bool buttonPassthroughEnabled = false;
    }
}
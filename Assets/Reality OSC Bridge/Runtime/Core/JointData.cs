using UnityEngine;

namespace StretchSense.OSCBridge
{
    [System.Serializable]
    public class JointData
    {
        public string name;
        public Vector3 position;
        public Quaternion rotation;
    }
}
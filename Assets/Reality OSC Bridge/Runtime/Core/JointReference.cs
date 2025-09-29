using UnityEngine;

namespace StretchSense.OSCBridge
{
    [System.Serializable]
    public class JointReference
    {
        public string jointName;
        public Transform assignedTransform;
    }
}
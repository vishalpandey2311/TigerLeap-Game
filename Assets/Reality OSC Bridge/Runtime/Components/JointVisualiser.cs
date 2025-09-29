using UnityEngine;
using System.Collections.Generic;

namespace StretchSense.OSCBridge
{
    public class JointVisualiser : MonoBehaviour
    {
        [Header("Joint Mapping")]
        public List<JointReference> jointsLeft = new();
        public List<JointReference> jointsRight = new();

        private int[] leftIndexMap;
        private int[] rightIndexMap;

        private string[] lastLeftJointNames;
        private string[] lastRightJointNames;

        private void Update()
        {
            // Ensure HandStateManager exists
            if (HandStateManager.Instance == null)
                return;

            var leftHand = HandStateManager.Instance.leftHand;
            var rightHand = HandStateManager.Instance.rightHand;

            // Ensure hand data is valid before proceeding
            if (leftHand?.joints == null || rightHand?.joints == null)
                return;

            // Only rebuild map if joint names have changed
            if (HasJointNamesChanged(leftHand, ref lastLeftJointNames))
                leftIndexMap = BuildIndexMap(jointsLeft, leftHand);

            if (HasJointNamesChanged(rightHand, ref lastRightJointNames))
                rightIndexMap = BuildIndexMap(jointsRight, rightHand);

            UpdateJointVisuals(jointsLeft, leftHand, leftIndexMap);
            UpdateJointVisuals(jointsRight, rightHand, rightIndexMap);
        }

        private bool HasJointNamesChanged(HandData data, ref string[] lastNames)
        {
            if (data == null || data.joints == null)
                return false;

            int count = data.joints.Count;

            // First-time setup or size mismatch
            if (lastNames == null || lastNames.Length != count)
            {
                lastNames = GetJointNamesArray(data);
                return true;
            }

            // Check for any changes in joint name order
            for (int i = 0; i < count; i++)
            {
                if (data.joints[i].name != lastNames[i])
                {
                    lastNames = GetJointNamesArray(data);
                    return true;
                }
            }
            return false;
        }

        private string[] GetJointNamesArray(HandData data)
        {
            if (data == null || data.joints == null)
                return new string[0];

            var arr = new string[data.joints.Count];
            for (int i = 0; i < data.joints.Count; i++)
                arr[i] = data.joints[i].name;
            return arr;
        }

        private int[] BuildIndexMap(List<JointReference> refs, HandData data)
        {
            if (refs == null || data == null || data.joints == null)
                return null;

            int[] map = new int[refs.Count];
            for (int i = 0; i < refs.Count; i++)
            {
                if (refs[i] != null)
                    map[i] = data.joints.FindIndex(j => j.name == refs[i].jointName);
                else
                    map[i] = -1; // No matching joint
            }
            return map;
        }

        private void UpdateJointVisuals(List<JointReference> refs, HandData data, int[] indexMap)
        {
            if (refs == null || refs.Count == 0 || data == null || data.joints == null || data.joints.Count == 0 || indexMap == null)
                return;

            for (int i = 0; i < refs.Count; i++)
            {
                if (refs[i] == null)
                    continue;

                int idx = indexMap[i];
                if (idx >= 0 && idx < data.joints.Count)
                {
                    var match = data.joints[idx];
                    var tr = refs[i].assignedTransform;
                    if (tr != null)
                    {
                        tr.localPosition = match.position;
                        tr.localRotation = match.rotation;
                    }
                }
            }
        }
    }
}

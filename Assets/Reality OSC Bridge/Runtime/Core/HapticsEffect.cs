using extOSC;
using UnityEngine;

namespace StretchSense.OSCBridge
{
    [System.Serializable]
    public class HapticsEffect
    {
        private Handedness handedness;
        public int amplitude = 255;
        private float frequency;
        public float durationMs = 1000f;
        public bool trigger;

        public HapticsEffect(Handedness handedness)
        {
            this.handedness = handedness;
            this.frequency = 0f;
        }
        public OSCMessage ToOSCMessage()
        {
            var msg = new OSCMessage(OscAddresses.OutputHaptic);
            msg.AddValue(OSCValue.Int((int)handedness)); // e.g. "left" or "right"
            msg.AddValue(OSCValue.Int(amplitude));
            msg.AddValue(OSCValue.Float(frequency));
            msg.AddValue(OSCValue.Float(durationMs));
            return msg;
        }
    }
}

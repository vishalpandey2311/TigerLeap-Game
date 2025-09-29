using UnityEngine;
using System.Collections.Generic;
using extOSC;

namespace StretchSense.OSCBridge
{
    public class StretchSenseOSCSender : MonoBehaviour
    {
        [Header("OSC Sender Settings")]
        public string outputAddress = "127.0.0.1";
        public int outputPort = 9003;

        [Header("Haptics")]
        public HapticsEffect hapticsEffectLeft = new HapticsEffect(Handedness.LEFT);
        public HapticsEffect hapticsEffectRight = new HapticsEffect(Handedness.RIGHT);

        private OSCTransmitter transmitter;

        private void Awake()
        {
            transmitter = GetComponent<OSCTransmitter>();
            if (transmitter == null)
                transmitter = gameObject.AddComponent<OSCTransmitter>();

            transmitter.RemoteHost = outputAddress;
            transmitter.RemotePort = outputPort;

        }

        private void Update()
        {
            if (hapticsEffectLeft.trigger)
            {
                var msg = hapticsEffectLeft.ToOSCMessage();
                transmitter.Send(msg);
                hapticsEffectLeft.trigger = false;
            }

            if (hapticsEffectRight.trigger)
            {
                var msg = hapticsEffectRight.ToOSCMessage();
                transmitter.Send(msg);
                hapticsEffectRight.trigger = false;
            }
        }
    }
}

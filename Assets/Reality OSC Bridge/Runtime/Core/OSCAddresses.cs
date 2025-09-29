using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StretchSense.OSCBridge
{
    public static class OscAddresses
    {
        // --- Streaming Hand Data ---
        public const string AnimationKinematicAll = "/v1/animation/kinematic/all";
        public const string OrientationAll = "/v1/orientation/all";
        public const string ControllerInputAll = "/v1/controller_input/all";

        // --- Configuration Data for OpenXR ---
        public const string TrackerOffsetCalibrationAll = "/v1/config/tracker_offset_calibration/all";
        public const string SetTrackerOffsetAll = "/v1/config/set_tracker_offset/all";
        public const string SetTrackerSourceAll = "/v1/config/set_tracker_source/all";
        public const string SetTrackerLocationAll = "/v1/config/set_tracker_location/all";
        public const string EnableButtonPassthroughAll = "/v1/config/enable_button_passthrough/all";

        // --- OSC Inputs ---
        public const string OutputHaptic = "/v1/output/haptic";

        // Application / Tracker Config
        public const string ConfigApplicationName = "/v1/config/application_name";
        public const string CalibrateTrackerOffset = "/v1/calibrate_tracker_offset";
        public const string ConfigCalibratedTrackerOffset = "/v1/config/calibrated_tracker_offset";
    }
}


VRUB.Plugins.Haptics = {
    Trigger: function() {
        return VRUB.Interop.Call("VRUB_Core_Haptics", "Trigger");
    },
    
    TriggerOnPointingDevice: function(axisId, strength) {
        return VRUB.Interop.Call("VRUB_Core_Haptics", "TriggerOnPointingDevice", axisId, strength);
    },
    
    TriggerOnDevice: function(deviceId, axisId, strength) {
        return VRUB.Interop.Call("VRUB_Core_Haptics", "TriggerOnDevice", deviceId, axisId, strength);
    },
}
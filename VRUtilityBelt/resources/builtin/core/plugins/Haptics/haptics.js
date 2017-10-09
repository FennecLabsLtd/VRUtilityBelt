var VRUB_Core_Haptics_Init = function() {
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

    delete VRUB_Core_Haptics_Init;
}

if(VRUB) {
    VRUB_Core_Haptics_Init();
} else {
    document.addEventListener('vrub:ready', function() {
        VRUB_Core_Haptics_Init();
    });
}
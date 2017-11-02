var VRUB_Core_Haptics_Init = function() {
    VRUB.Plugins.Haptics = {
        _bindings: [],

        Defaults: {
            Strength: 200,
            Duration: 15,
            Interval: 5,
        },

        Trigger: function() {
            return VRUB.Interop.Call("VRUB_Core_Haptics", "Trigger");
        },
        
        TriggerOnPointingDevice: function(axisId, strength, interval, duration) {
            return VRUB.Interop.Call("VRUB_Core_Haptics", "TriggerOnPointingDevice", axisId, strength, interval, duration);
        },
        
        TriggerOnDevice: function(deviceId, axisId, strength) {
            return VRUB.Interop.Call("VRUB_Core_Haptics", "TriggerOnDevice", deviceId, axisId, strength, interval, duration);
        },

        AddBinding: function(selector) {
            VRUB.Plugins.Haptics._bindings.push(selector);
        },

        EnableBindings: function() {
            //var el = document.querySelector("[data-haptics]");
            document.addEventListener('mouseover', function(e) {
                if(!e.target.hasAttribute('data-haptics')) {
                    var matches = false;

                    for(var i = 0; i < VRUB.Plugins.Haptics._bindings.length; i++) {
                        if(e.target.matches(VRUB.Plugins.Haptics._bindings[i])) {
                            matches = true;
                            break;
                        }
                    }

                    if(!matches)
                        return;
                }

                var hapticValue = e.target.getAttribute('data-haptics');

                var strength = VRUB.Plugins.Haptics.Defaults.Strength;
                var duration = VRUB.Plugins.Haptics.Defaults.Duration;
                var interval = VRUB.Plugins.Haptics.Defaults.Interval;

                if(hapticValue && hapticValue != '') {
                    if(hapticValue.indexOf(",") > -1) {
                        var split = hapticValue.split(',');

                        duration = split[0];

                        if(split.length > 1)
                            strength = parseInt(split[1]);

                        if(split.length > 2)
                            interval = parseInt(split[2]);

                    } else {
                        duration = parseInt(hapticValue);
                    }
                }

                VRUB.Plugins.Haptics.TriggerOnPointingDevice(0, strength, interval, duration);
            });
        }
    }

    VRUB._fireDocumentEvent('vrub:plugin_ready:VRUB_Core_Haptics');
    VRUB.Plugins.Haptics.EnableBindings();

    delete VRUB_Core_Haptics_Init;
}

if(VRUB) {
    VRUB_Core_Haptics_Init();
} else {
    document.addEventListener('vrub:ready', function() {
        VRUB_Core_Haptics_Init();
    });
}
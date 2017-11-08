var VRUB_Core_Config_Init = function() {
    VRUB.Plugins.Config = {
        Set: function(key, value, temp) {
            return VRUB.Interop.Call("VRUB_Core_Config", "Set", key, value, temp);
        },
        
        FetchConfig: function(key, temp) {
            return VRUB.Interop.Call("VRUB_Core_Config", "FetchConfig", key, temp);
        },
        
        FetchValue: function(key, temp) {
            return VRUB.Interop.Call("VRUB_Core_Config", "FetchValue", key, temp);
        }
    }

    VRUB._fireDocumentEvent('vrub:plugin_ready:VRUB_Core_Config');
    
    delete VRUB_Core_Config_Init;
}

if(window.hasOwnProperty("VRUB")) {
    VRUB_Core_Config_Init();
} else {
    document.addEventListener('vrub:ready', function() {
        VRUB_Core_Config_Init();
    });
}
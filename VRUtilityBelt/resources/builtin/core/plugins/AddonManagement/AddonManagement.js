var VRUB_Core_AddonManagement_Init = function() {
    VRUB.Plugins.AddonManagement = {
        GetAddons: function() {
            return VRUB.Interop.Call("VRUB_Core_AddonManagement", "GetAddons");
        },
        
        ToggleAddon: function(addonKey, toggle) {
            return VRUB.Interop.Call("VRUB_Core_AddonManagement", "ToggleAddon", addonKey, toggle);
        },
    }
    
    delete VRUB_Core_AddonManagement_Init;
}

if(VRUB) {
    VRUB_Core_AddonManagement_Init();
} else {
    document.addEventListener('vrub:ready', function() {
        VRUB_Core_AddonManagement_Init();
    });
}
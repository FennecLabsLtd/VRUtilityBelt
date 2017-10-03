VRUB.Plugins.AddonManagement = {
    GetAddons: function() {
        return VRUB.Interop.Call("VRUB_Core_AddonManagement", "GetAddons");
    },
    
    ToggleAddon: function(addonKey, toggle) {
        return VRUB.Interop.Call("VRUB_Core_AddonManagement", "ToggleAddon", addonKey, toggle);
    },
}
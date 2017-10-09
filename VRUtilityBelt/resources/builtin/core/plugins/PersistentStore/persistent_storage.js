var VRUB_Core_PersistentStore_Init = function() {
    VRUB.Plugins.PersistentStore = {
        Set: function(key, value, temp) {
            return VRUB.Interop.Call("VRUB_Core_PersistentStore", "Set", key, value, temp);
        },
        
        Fetch: function(key, temp) {
            return VRUB.Interop.Call("VRUB_Core_PersistentStore", "Fetch", key, temp);
        },
        
        Clear: function(key, temp) {
            return VRUB.Interop.Call("VRUB_Core_PersistentStore", "Clear", key, temp);
        },
        
        ClearAll: function(temp) {
            return VRUB.Interop.Call("VRUB_Core_PersistentStore", "ClearAll", temp);
        },
        
        FetchAll: function(temp) {
            return VRUB.Interop.Call("VRUB_Core_PersistentStore", "FetchAll", temp);
        }
    }

    VRUB._fireDocumentEvent('vrub:plugin_ready:VRUB_Core_PersistentStore');
    
    delete VRUB_Core_PersistentStore_Init;
}

if(VRUB) {
    VRUB_Core_PersistentStore_Init();
} else {
    document.addEventListener('vrub:ready', function() {
        VRUB_Core_PersistentStore_Init();
    });
}
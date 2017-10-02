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
var PersistentStorage = {
    Set: function(key, value, temp) {
        VRUB_Plugins_PersistentStore.Set(key, JSON.stringify(value), temp);
    },
    
    Get: function(key, temp) {
        return JSON.parse(VRUB_Plugins_PersistentStore.Get(key, temp));
    },
    
    Clear: function(key, temp) {
        VRUB_Plugins_PersistentStore.Clear(key, temp);
    },
    
    ClearAll: function(temp) {
        VRUB_Plugins_PersistentStore.ClearAll(temp);
    },
    
    GetAll: function(temp) {
        return JSON.parse(VRUB_Plugins_PersistentStore.GetAll(temp));
    }
}
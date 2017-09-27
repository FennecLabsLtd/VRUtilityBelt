function VRUBPromise(uuid) {
    this._thens = [];
    
    var promise = this;
    
    VRUB.Events.Subscribe("promise-" + uuid, function(response) {
        for(i = 0; i < promise._thens.length; i++) {
            promise._thens[i](response);
        }
        
        VRUB.Events.Clear("promise-" + uuid);
    });
    
    this.then = function(callback) {
        this._thens.push(callback);
    }
};

function VRUBInstantiator() {
    var root = this;
    
    this.Events = {
        Listeners: {},
        Subscribe: function(event, callback) {
            if(!this.Listeners[event])
                this.Listeners[event] = [];
            
            this.Listeners[event].push(callback);
        },
            
        Fire: function(name, payload) {
            if(this.Listeners[name]) {
                var arr = this.Listeners[name];
                for(i = 0; i < arr.length; i++) {
                   arr[i](payload, name);
                }
            }
        },
        
        Clear: function(event) {
            if(this.Listeners[event])
                delete this.Listeners[event];
        },

        Interop: {},
    };
    
    this.Interop = {
        Call: function(objectName, methodName) {
            var promiseUUID = root.GenerateUUID();
            var promise = new VRUBPromise(promiseUUID);
            
            VRUB_Interop_Bridge.Call(objectName, methodName, promiseUUID, JSON.stringify(Array.from(arguments).slice(2)));
            
            return promise;
        },
        
        CallSync: function(objectName, methodName) {
            var promiseUUID = root.GenerateUUID();
            var promise = new VRUBPromise(promiseUUID);
            VRUB_Interop_Bridge.CallSync(objectName, methodName, promiseUUID, JSON.stringify(Array.from(arguments).slice(2)));
                
            return promise;
        },
    };
    
    this.Events.Subscribe("BridgeError", function(error) {
        console.error("BRIDGE ERROR: " + error);
    });
    
    // Borrowed from https://stackoverflow.com/questions/105034/create-guid-uuid-in-javascript
    this.GenerateUUID = function() {
        return ([1e7]+-1e3+-4e3+-8e3+-1e11).replace(/[018]/g, c =>
            (c ^ crypto.getRandomValues(new Uint8Array(1))[0] & 15 >> c / 4).toString(16)
        );
    };
    
    return this;
};

var VRUB = new VRUBInstantiator();
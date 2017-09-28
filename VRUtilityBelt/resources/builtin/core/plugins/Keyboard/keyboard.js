var VRUB_Keyboard = {
    Show: function(value) {
        return VRUB.Interop.Call("VRUB_Core_Keyboard", "Show", value ? value : "");
    },
    
    Hide: function() {
        return VRUB.Interop.Call("VRUB_Core_Keyboard", "Hide");
    },
   
}
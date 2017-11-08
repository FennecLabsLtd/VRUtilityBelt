var VRUB_Core_Keyboard_Init = function() {
    VRUB.Plugins.Keyboard = {
        Show: function(value) {
            return VRUB.Interop.Call("VRUB_Core_Keyboard", "Show", value ? value : "");
        },
        
        Hide: function() {
            return VRUB.Interop.Call("VRUB_Core_Keyboard", "Hide");
        },
    }
    
    delete VRUB_Core_Keyboard_Init;
}

if(window.hasOwnProperty("VRUB")) {
    VRUB_Core_Keyboard_Init();
} else {
    document.addEventListener('vrub:ready', function() {
        VRUB_Core_Keyboard_Init();
    });
}
$(document).ready(function() {
    $("#btn-pstore-set").click(function() {
        var btn = this;
        VRUB.Plugins.PersistentStore.Set("foo", "bar").then(function() {
            $(btn).text('Successflly set "Foo" to "Bar"');
        }).reject(function() {
            $(btn).text('Failed to set "Foo" to "Bar"');
        });
    });
    
    $("#btn-pstore-get").click(function() {
        var btn = this;
        VRUB.Plugins.PersistentStore.Fetch("foo").then(function(response) {
            $(btn).text("Foo: " + response);
        }).reject(function() {
            $(btn).text('Failed to fetch "Foo"');
        });
    });
    
    $("#btn-pstore-clear").click(function() {
        var btn = this;
        VRUB.Plugins.PersistentStore.Clear("foo").then(function() {
            $(btn).text('Successflly cleared "Foo"');
        }).reject(function() {
            $(btn).text('Failed to clear "Foo"');
        });
    });
    
    $("#btn-pstore-clearall").click(function() {
        var btn = this;
        VRUB.Plugins.PersistentStore.ClearAll().then(function() {
            $(btn).text('Successflly cleared all values');
        }).reject(function() {
            $(btn).text('Failed to clear all values');
        });
    });
    
    $("#btn-keyboard-show").click(function() {
        VRUB.Plugins.Keyboard.Show();
    });
    
    $("#btn-keyboard-hide").click(function() {
        VRUB.Plugins.Keyboard.Hide();
    });
    
    $("#btn-haptics-1").click(function() {
        VRUB.Plugins.Haptics.Trigger();
    });
    
    $("#btn-haptics-2").click(function() {
        VRUB.Plugins.Haptics.TriggerOnPointingDevice(1, 700);
    });
    
    $("#btn-haptics-3").click(function() {
        VRUB.Plugins.Haptics.TriggerOnPointingDevice(2, 700);
    });
    
    $("#btn-haptics-4").click(function() {
        VRUB.Plugins.Haptics.TriggerOnPointingDevice(3, 700);
    });

    var pos = $("#info-tracking-latestPosition");

    var stop = false;

    var devices = {};

    $(document).on('vrub:event:new_pose', function(ev) {
        devices['' + ev.detail.device_index] = ev.detail.device_index + " (" + ev.detail.device_class + "): " + ev.detail.position.X + ", " + ev.detail.position.Y + ", " + ev.detail.position.Z;

        var str = "";

        for(var dev in devices) {
            str += devices[dev] + "<br>";
        }

        pos.html(str);
    });
});

if(!window.hasOwnProperty("VRUB") || VRUB.Plugins.Haptics) {
    $(document).on('vrub:plugin_ready:VRUB_Core_Haptics', function() {
        VRUB.Plugins.Haptics.AddBinding('#btn-haptics-manual');
    });
} else {
    VRUB.Plugins.Haptics.AddBinding("#btn-haptics-manual");
}
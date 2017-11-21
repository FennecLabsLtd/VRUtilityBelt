Events Bus
==========

Every page, on load, will have the VRUB.js script injected into it - which provides a simple event bus.

.. method:: VRUB.Events.Subscribe(event, callback)

    Subscribe to an event and fire a callback when the event occurs

    :param string event: The event name
    :param void callback: The function (anonymous or otherwise) to call, with the arguments ``payload`` (event data) and ``name`` (the event name)

.. method:: VRUB.Events.Clear(event)

    Clear all listeners for an event
    
    :param string event: The event name

.. method:: VRUB.Events.Fire(name, payload)

    Fires the specified event with the specified payload
    
    :param string event: The event name
    :param object payload: The payload data to send to the callback

Native Document Events
~~~~~~~~~~~~~~~~~~~~~~

All events from the event bus are also fired as a document event under the namespace ``vrub:event``. You can listen to events using this method by adding a listener to the document for ``vrub:event::event_name`` using a callback with one argument for the ``payload``. Please note that using this method, event data will be under a ``detail`` key on the payload object (``payload.detail.fieldName`` rather than ``payload.fieldName``).

A few built in events are also fired at various times:

.. method:: vrub:ready

    Fired when VRUB's core (VRUB.js) is injected and ready.

.. method:: vrub:plugin_ready:plugin_name

    Fired when a plugin is ready. All plugins with manually created JS Bridges should fire a ``plugin_ready`` event when ``vrub:ready`` has fired or once the plugin is ready (whichever comes last)


Examples
~~~~~~~~

These examples use JQuery (available at ``vrub://libs/jquery-3.2.1.min.js`` or you can use your own version)

Binding to a ``plugin_ready`` event

.. code-block:: javascript

    $(document).on('vrub:plugin_ready:VRUB_Core_Haptics', function() {
        VRUB.Plugins.Haptics.AddBinding("#btn-example");
    });

Waiting for VRUB to be ready and firing an event onto the event bus. This will print "Hello World" twice (once for the VRUB listener, once for the document event)

.. code-block:: javascript

    $(document).on('vrub:ready', function() {
        VRUB.Events.Listen('test_event', function(data) {
            console.log(data.foo); // Will print "Hello World"
        });

        $(document).on('vrub:event:test_event', function(data) {
            console.log(data.detail.foo); // Prints "Hello World" also
        });

        VRUB.Events.Fire("test_event", { foo: "Hello World" });
    });
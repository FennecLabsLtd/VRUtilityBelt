Haptics API
===========

The Haptics API provides access to the OpenVR Haptics functions via a set of convenient methods.

Haptics API Plugin Key: ``VRUB_Core_Haptics``

Haptics API Ready Event: ``vrub:plugin_ready:VRUB_Core_Haptics``

Pulse Triggering
~~~~~~~~~~~~~~~~

.. method:: VRUB.Plugins.Haptics.Trigger()

    Triggers a default haptic pulse with a strength of 200, interval of 5ms and duration of 100ms on the current pointing device.


.. method:: VRUB.Plugins.Haptics.TriggerOnPointingDevice(axis, strength, interval, duration)

    Triggers a haptic pulse on the specified axis (usually 0) with the specified strength, interval (ms) and duration (ms). A duration of 50 with 5 milliseconds between pulses will trigger 10 pulses (though this would be limited by the framerate of the application)

    :param int axis: Axis ID to trigger pulse on (recommended: 0)
    :param int strength: Value between 0 and 3999 to use for strength (200 is recommended)
    :param int interval: Milliseconds between pulses (minimum: 5ms)
    :param int duration: Duration of haptic pulse (how long to perform pulses for)


.. method:: VRUB.Plugins.Haptics.TriggerOnDevice(deviceId, axisId, strength, interval, duration)

    Triggers a haptic pulse on device ``deviceId`` on the specified axis (usually 0) with the specified strength, interval (ms) and duration (ms). A duration of 50 with 5 milliseconds between pulses will trigger 10 pulses (though this would be limited by the framerate of the application)

    :param int deviceId: The Device Index to trigger a pulse on
    :param int axis: Axis ID to trigger pulse on (recommended: 0)
    :param int strength: Value between 0 and 3999 to use for strength (200 is recommended)
    :param int interval: Milliseconds between pulses (minimum: 5ms)
    :param int duration: Duration of haptic pulse (how long to perform pulses for)

Event Binding
~~~~~~~~~~~~~

.. method:: VRUB.Plugins.Haptics.AddBinding(selector)

    Adds a binding that will cause a haptic pulse to binding when the pointer enters an element that matches the CSS selector specified by ``selector``.

    :param string selector: The CSS selector to add a binding for


.. attribute:: VRUB.Plugins.Haptics.Defaults

    .. code-block:: javascript

        {
            Strength: 200,
            Duration: 15,
            Interval: 5
        }


    You can alter the values for ``Strength``, ``Interval`` and ``Duration`` used by the element binder by changing the values on this JS object.
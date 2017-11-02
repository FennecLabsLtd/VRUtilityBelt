Haptics API
===========

The Haptics API provides access to the OpenVR Haptics functions via a set of convenient methods.

Haptics API Plugin Key: ``VRUB_Core_Haptics``
Haptics API Ready Event: ``vrub:plugin_ready:VRUB_Core_Haptics``

Pulse Triggering
~~~~~~~~~~~~~~~~

``VRUB.Plugins.Haptics.Trigger()``

Triggers a default haptic pulse with a strength of 200, interval of 5ms and duration of 100ms on the current pointing device.

``VRUB.Plugins.Haptics.TriggerOnPointingDevice(axis, strength, interval, duration)``

Triggers a haptic pulse on the specified axis (usually 0) with the specified strength, interval (ms) and duration (ms). A strenth of 200 is recommended, and the interval must be at least 5ms.

``VRUB.Plugins.Haptics.TriggerOnDevice(deviceId, axisId, strength, interval, duration)``

Triggers a haptic pulse on the device with the index ``deviceId``, on the specified axis axis (usually 0) with the specified strength, interval (ms) and duration (ms). A strenth of 200 is recommended, and the interval must be at least 5ms.

Event Binding
~~~~~~~~~~~~~

``VRUB.Plugins.Haptics.AddBinding(selector)``

Adds a binding that will cause a haptic pulse to binding when the pointer enters an element that matches the CSS selector specified by ``selector``.

``VRUB.Plugins.Haptics.Defaults``

You can alter the values for ``Strength``, ``Interval`` and ``Duration`` used by the element binder by changing the values on this JS object.
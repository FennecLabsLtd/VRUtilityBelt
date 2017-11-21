Overlays
========

URL Schemes
-----------

In aid of security, ``file://`` is not permitted as a protocol for VRUB Overlays - nothing should have that kind of access out of the box to the user's entire filesystem.

Instead, there are three schemes in place to access various parts of the filesystem

.. method:: addon://

Refers to the root of the overlay's parent addon, useful to grabbing assets for the current and other overlays as part of the addon

**Example**

``addon://overlays/sample_overlay/example.css`` would grab the `example.css` file from the `sample_overlay` in the addon that owns the current overlay.

.. method:: vrub://

Refers to the ``static/`` folder in the VRUB root. This is where VRUB-provided assets live such as the ``VRUB.js`` library.

**Example**

``vrub://VRUB.js``

.. method:: plugin://

Used to access assets provided by plugins, these are not addon restricted.

**Examples**

``plugin://vrub_core_persistentstore/persistent_store``

``plugin_key`` is the plugin's addon key (e.g. ``vrub_core``) and the plugin's name (e.g. ``PersitentStore``) concatanated (``vrub_core_PersistentStore``)

``plugin://`` URLs are not case sensitive, so don't worry about having to match the casing of the plugin name.

Terms
-----

.. attribute:: "Dashboard Overlay"

    These are overlays that are fixed inside the SteamVR dashboard, which has the tabs, volume controls, etc.

.. attribute:: "Floating Overlay"

    These are overlays that can be positioned anywhere and attached to another overlay, a device or positioned absolutely in the roomscale space.

Overlay manifest.json
---------------------

Overlay manifest.json files are similar to Addon manifest.json files, but are much more complex. They contain the following data:

.. attribute:: string: name

    The name of the overlay, this is used in the tab for dashboard overlays so keep it short.

.. attribute:: string: key

    The key of the overlay, keep this the same as the folder name for the overlay

.. attribute:: string: description

    Description of the overlay, will eventually appear in the VRUB Options UI

.. attribute:: string: entrypoint

    The URL or file to load first. This can be a fully qualified URL (e.g. ``https://google.com`` or ``addon://overlays/overlay_key/index.html``) or a filename (e.g. ``index.html`` or ``../other_overlay/index.html``). When using a relative filename, only files within the addon's folder and subfolders can be loaded.

.. attribute:: integer: width

    The pixel-width of the browser and resulting texture. Defaults to 1200

.. attribute:: integer: height

    The pixel-height of the browser and resulting texture. Defaults to 800

.. attribute:: float: meters

    The meter-width of the dashboard overlay in SteamVR. The meter-height is calculated from the aspect ratio of the browser and the meter-width. Defaults to 2.5m

.. attribute:: float: floating_meters

    As with ``meters``, this is the meter-width of a floating (non-dashboard) overlay. Defaults to 2.5m

.. attribute:: bool: debug

    When enabled, and VRUB is launched with the ``-debug`` flag, this will display the Chromium Web Development Tools when the overlay is loaded. Do not leave this on when releasing on the workshop. Defaults to false.

.. attribute:: bool: keyboard

    When enabled, text inputs and textareas will automatically bring up the SteamVR keyboard when focused. Defaults to false.

.. attribute:: bool: mouse

    When enabled, floating overlays can take mouse input. Dashboard overlays can always take mouse input and will not pay attention to this value. Defaults to false.

.. attribute:: object: inject

    An object containing two keys: ``js`` and ``css``. These two keys are arrays of JS and CSS files to inject into every page on load. ``VRUB.js`` is always injected and should not be included here.

    Example:

    .. code-block:: javascript

        "inject": {
            "js": [
                "addon://overlay/overlay_key/injected_script.js",
            ],
            "css": [
                "addon://overlay/overlay_key/injected_styles.css",
            ]
        }

.. attribute:: bool: persist_session_cookies

    Generally not advisable, this will persist session cookies (which are usually wiped after each session) between sessions. This could pose a security risk for users and should be used carefully. Defaults to false.

.. attribute:: int: mouse_delta_tolerance

    The tolerance when holding the trigger before a click should be considered a drag. When a user clicks the trigger on their controller while pointing at an overlay, the browser cursor will be locked in place until they move the laser pointer outside this radius. This was implemented due to slight movement in the controller resulting in buttons and links not being clickable as the movement would turn the click into a drag event. Defaults to 20 pixels.

.. attribute:: float: opacity

    The opacity of the overlay, in the range of 0 (transparent) to 1 (complete opaque). Defaults to 0.9

.. attribute:: string: thumbnail

    The image file path to use as the icon on the dashboard tab list. This must be inside the addon folder or a subdirectory (like the overlay folder...)

.. attribute:: object: attachment

    The attachment for a floating overlay. Comprises of an attachment type, position and rotation. Defaults to absolute positioning with a position of 0 (middle of the playspace on the ground) and a rotation of 0. All positions are in meters, with + on the Z-axis moving towards the front of the play space.

    Example:

    .. code-block:: json

        {
            "type": "absolute", // Must be one of type: "hmd", "leftcontroller", "rightcontroller", "absolute", "overlay" or "deviceindex"
            "position": { // Measured in meters
                "x": 0,
                "y": 1.2,
                "z": 1.5
            },
            "rotation": { // Measured in euler angles (360 degrees)
                "x": 0,
                "y": 45,
                "z": 0
            },
            "key": "", // When attached to an overlay, this is the overlay key to attach to. When attached to a deviceindex, this is the device index.
        }

.. attribute:: bool: show_as_dashboard

    Defines whether the overlay should display as a dashboard overlay. Defaults to true

.. attribute:: bool: show_as_floating

    Defines whether the overlay should display as a floating overlay. Defaults to false

**Note**: One browser instance is launched per overlay, if you set both of the above to ``true`` then they will share the same browser instance and view, they will essentially mirror each other.

.. attribute:: array: render_models

    A list of render models to load into the overlay (refer to the :doc:`./render_models` documentation for more information)

.. attribute:: array: plugins
    
    A list of plugin keys to load in when the overlay is first loaded. Please refer to an individual plugin's documentation for the relevant key

.. attribute:: bool: disable_scrolling

    Disable the scroll binding on the controller. It is recommended that you try to avoid using scrolling pages and instead find ways to fit everything within the overlay boundaries where possible. Defaults to false

.. attribute:: string: alpha_mask

    An alpha mask image to apply to the output texture of the overlay. An alpha mask must be a PNG, and only the alpha channel will be read from it. Use this for things like rounding the corners of an overlay, or basically anything you want really.

.. attribute:: string fragment_shader

    Use a custom fragment (pixel) GLSL shader path for this overlay instead of the default. You can copy the default shader from the ``static`` directory and modify it as you see fit. Please note this is still quite experimental and may be prone to breaking.
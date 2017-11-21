Getting Started
===============

Folder Structure
----------------

VRUB expects addon components to follow a specific folder structure, which you can see below::

    addon_name
    |   manifest.json
    |   preview_image.jpg
    |   workshop.json
    |   
    \---overlays
        +---overlay_name
        |       index.html
        |       manifest.json
    \---plugins
        +---plugin_name
        |       PluginName.dll
        |       manifest.json
        
Addon manifest.json
-------------------

Addon manifests provide VRUB with information about a specific addon. Every addon must have a manifest.json.

The following keys are read from a manifest file:

.. attribute:: string: name
    
    The name of the addon, used for display purposes

.. attribute:: string: key

    The addon key, must be unique and descriptive

.. attribute:: string: description

    A short description of the addon

.. attribute:: string: author

    The name of the addon author

.. attribute:: array: overlays

    A JSON array of overlay keys (as strings) to load, which must match the folder names within the ``overlays`` folder

.. attribute:: themes

    Currently unused.

.. attribute:: plugins

    A JSON of plugin keys (as strings) to load, which must match the folder names within the ``plugins`` folder. Please note that only addons in the ``builtin`` folder will currently load plugins for security reasons.

.. attribute:: permissions

    A JSON object of key/values for any permissions the addon will request, the value of each key must describe the reason for requesting the permission. For example:

    .. code-block:: javascript

        "permissions": {
            "vrub.core.persistent_store": "To store user data",
            "vrub.core.overlay_manipulation": "To change the position of the Steam Dashboard"
        }

    Please refer to an individual plugin's documentation for any permission keys and their purpose.

.. attribute:: bool: sudo

    Gives an addon "sudo" privileges, which means it will bypass all permission checks. Only available to addons in the ``builtin`` folder as this is exclusively used for configuring and managing VRUB.

.. attribute:: bool: default_to_disabled

    Defaults the addon to be disabled by default, usually addons are enabled when first installed. Defaults to false.

Example Manifest
~~~~~~~~~~~~~~~~

This is the manifest.json for the sample addon

.. code-block:: javascript

    {
        "name": "Sample Addon",
        "key": "sample_addon",
        "description": "A sample addon",
        "overlays": [
            "sample"
        ],
        "plugins": [
        ],
        "default_to_disabled": true,
    }
Overlays
========

URL Schemes
-----------

In aid of security, ``file://`` is not permitted as a protocol for VRUB Overlays - nothing should have that kind of access out of the box to the user's entire filesystem.

Instead, there are three schemes in place to access various parts of the filesystem

addon://
~~~~~~~~

Refers to the root of the overlay's parent addon, useful to grabbing assets for the current and other overlays as part of the addon

**Example**

``addon://overlays/sample_overlay/example.css`` would grab the `example.css` file from the `sample_overlay` in the addon that owns the current overlay.

vrub://
~~~~~~~

Refers to the ``static/`` folder in the VRUB root. This is where VRUB-provided assets live such as the ``VRUB.js`` library.

**Example**

``vrub://VRUB.js``

plugin://
~~~~~~~~~

Used to access assets provided by plugins, these are not addon restricted.

**Example**

``plugin://vrub_core_persistentstore/persistent_store``

``plugin_key`` is the plugin's addon key (e.g. ``vrub_core``) and the plugin's name (e.g. ``PersitentStore``) concatanated (``vrub_core_PersistentStore``)

``plugin://`` URLs are not case sensitive, so don't worry about having to match the casing of the plugin name.
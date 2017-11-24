Overlay Render Models
=====================

You can now render 3D models into the compositor, parented to an overlay! Each model will have its own transparent overlay attached to it, so you can have multiple models per overlay and they can be manipulated using the pending overlay manipulation API!

Caveats
~~~~~~~

SteamVR's overlay render model system is clunky. It expects textures, it expects baked vertex normals and it expects them to be exported in a certain way.

It also caches the overlay in memory, even when you close VRUB. So if you modify your OBJ or MTL file once you've loaded it in, you'll need to rename it or restart SteamVR to refresh the data.

Preparing your Model
~~~~~~~~~~~~~~~~~~~~

As stated above, SteamVR is very picky about the format of the OBJ files. For the purposes of this guide, we will be using Blender to ensure our OBJ and MTLs fit an expected format.

**Preface**

You **must** have a texture attached to this model. Vertex colours don't appear to work (or perhaps I've set it up wrong).

**1. Importing**

You'll need to start a new Blender project and delete the create cube. You can also delete everything else like the camera, lights, etc if you'd like.

Go to File > Import > Wavefront OBJ and locate your file. If it's not an OBJ, choose the relevant format and import it into Blender.

**2. Scale and Transform**

Ensure your model is to the correct scale. If you need a guide, import the ``vr_controller_vive_1_5.obj`` file into Blender (File > Import > Wavefront OBJ). This file can be located in your ``steamapps\common\OpenVR\resources\rendermodels\vr_controller_vive_1_5`` folder. You can then use the controller as a reference for the scale of your model.

You must also ensure your model is central, so select it and select "Geometry to Origin" on the left toolbar to set it to 0,0,0.

**Note**: You must ensure you only have one "shape" in Blender, so select all the pieces and Ctrl+J to merge them if you have more than one.

**3. Exporting**

1.) Select just the model you want to export, if you didn't delete everything else in the scene 
previously.

2.) Go to ``File > Export > Wavefront OBJ``

3.) Ensure the following are selected:
    - Triangulate Faces
    - Write normals
    - Export Selection (if you didn't delete everything else in the scene)

Everything else can be left at the defaults.

**4. Textures**

You'll need to open up your texture PNG in Photoshop or a similar application and ensure it uses 8-bits or less per channel. In Photoshop, this is found under ``Image > Mode``. Also ensure it is RGB and not Indexed as I haven't tested Indexed so let's play it safe.

SteamVR also only supports one diffuse texture material per model, so you will need to ensure your models have all textures baked into one material.

**5. Done**

Hopefully, your OBJ will contain vertex normals (``vn``) and a reference to a material file. And hopefully your material file will reference a PNG diffuse map (your texture). I'm not sure if you can use normal maps or anything like that unfortunately, so it's just the diffuse map available until further testing has been done.

Preparing the manifest.json
~~~~~~~~~~~~~~~~~~~~~~~~~~~

Each render model must be referenced in an overlay's manifest.json. If you just want render models, have the entry point for the overlay be an empty page and have it be a floating overlay **only** positioned at 0,0,0.

The ``render_models`` key in the manifest.json should look like this:

.. code-block:: javascript

        "render_models": [
        {
            "key": "example",

            // If true, the model will be positioned absolutely instead of relative to the parent
            "absolute": true,

            // If not provided, the overlay that provides this render model will be the parent (if the model is not absolute). Example here would attach it the Steam Big Picture overlay if absolute was false.
            "parent": "valve.steam.bigpicture",
            "model": "models/example.obj",

            // Don't mess with the following 3 unless you want your model to break aspect ratio. Models only scale on two axis, so expect strange results.
            "width": 1,
            "height": 1,
            "meters": 1,

            // These values are in meters, if the overlay is not absolute they are relative to the parent overlay
            "position": {
                "X": 1.6,
                "Y": 0.78,
                "Z": 0
            },

            // As with overlays, these values are euler angles (360 degrees)
            "rotation": {
                "X": 65,
                "Y": 180,
                "Z": -10
            },
            "opacity": 1 // The opacity of the model
        }
    ]

Hopefully, when you load in your overlay next you'll have your model! If not, check the ``vrclient_vrcompositor.txt`` and ``vrcompositor.txt`` files in your Steam logs folder for any errors. Also check the VRUB log (accessible via the system tray icon) for any errors too.
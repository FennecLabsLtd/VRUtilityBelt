Workshop Submissions
====================

To submit an overlay to the workshop, you'll need to write a workshop.json file and use the workshop tool to upload/update it.

Please note that the workshop will not be available until the store page has been published.

Workshop.json
~~~~~~~~~~~~~

The workshop.json file looks like this:

.. code-block:: javascript

    {
        "title": "Sample Addon", // The name of the workshop entry
        "description": "This is a sample overlay", // A description of the entry.
        "type": [ "overlay" ], // Only "overlay" is available currently
        "tags": [ "utility" ], // Please refer to the list below for available tags
        "ignore": [ ".git", ".scss", "gulpfile.json" ], // Files to not include in the uploaded archive
        "preview_image": "preview_image.png", // The image used on the workshop for your item

        // This will be set on first submission, do not modify or delete this as you will end up creating a new workshop item instead of updating your existing entry.
        "file_id": null
    }

**Tags**

The following tags are currently available:
    - Music
    - Social
    - Utility
    - Game

If you would like more tags added, please submit a Github issue with your ideas.
# Romancing SaGa 3 Resource Manager

## How To Install

1. Download the latest [RS3ResourceManager.zip](https://github.com/rcfox/RS3ResourceManager/releases/latest/download/RS3ResourceManager.zip)
    * Older versions might also be available on the [Releases](https://github.com/rcfox/RS3ResourceManager/releases) page.
2. Get [UnityModManager](https://www.nexusmods.com/site/mods/21/)(UMM).
    * If you already have it, make sure to upgrade to at least version **0.23.4b**
3. Run UMM, set the game to "Romancing SaGa 3", choose the directory it's installed to, then click the "Install" button.
    * If you're using Steam, the directory will probably be `C:\Program Files (x86)\Steam\steamapps\common\RomancingSaGa3`
4. In the Mods tab of UMM, either drop the zip file into the bottom of the window, or click the "Install Mod" button to find the zip file.
5. If everything worked correctly, you should be greeted by a "Mod Manager" window when the game starts up.
    * You can disable this popup from happening at startup from the Mod Manager's settings tab.
    * You can open/close the Mod Manager with Ctrl+F10

## What Does It Do?

The mod is for easily extracting and loading resources from the game. ie: images, translation files, maps, etc. (I haven't figured out sound or music yet though.)

By default, the game will first look in the "resources" directory under this mod's directory for any of the files it's trying to load.
If the file isn't found, it will load from the game's packaged assets like normal. This lets us create targeted resource mods by just modifying the files we care about.

Those who wish to create resource mods can enable the "Extract files as they load" option in the Mod Manager window,
or set `<SaveMissingFiles>true</SaveMissingFiles>` in the Settings.xml file.
This will cause all of the resources that the game loads to be saved in the "resources" directory under the mod's directory.

Unfortunately due to a limitation in how the game is written, only resources that are actually loaded will be saved.
You would have to play through the entire game to save every monster image, for example.

The `LoadFilesOnStartup` setting is a comma-separated string that can be used to force the game to load files that we know about.
For instance, all of the character sprites are of the form `obj/objXX.gif`, where `XX` is a hex value.
All of the monster images are of the form `monster/mXXX.png`, where `XXX` is a 3-digit decimal value.

To force the game to load some less-common images, we could set:
`<LoadFilesOnStartup>obj/obj1c.gif,obj/obj11.gif,monster/m261.png</LoadFilesOnStartup>`

You can look through the game's source code in `Assembly-CSharp.dll` with [dnSpy](https://github.com/dnSpy/dnSpy) to find other filename patterns, or perhaps some files will refer to filenames. You'll have to explore!

## Where Is The Mod Directory?

It will probably be something like `C:\Program Files (x86)\Steam\steamapps\common\RomancingSaGa3\Mods\RS3ResourceManager`, but it depends on how you installed the game.

## Note About Image Editing

The character sprites (`obj/objXX.gif`) are GIF images that use colour tables. MS Paint and Paint.net don't seem to support saving images with colour tables,
and the images will be messed up when loaded back into the game. I'm told Photoshop and Aseprite are able to save the images properly, but I haven't verified this yet.

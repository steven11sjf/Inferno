# Inferno

Inferno is a top-down shooter that was in development from May 2020 - February 2020. This prototype was in development from June to December. It implements some of the features that would have been in the final game, including the combat and dialogue systems. Due to issues with other aspects of production such as a struggle finding artists, the project has been shelved for now. 

## Play the game

There is a compiled copy of the game in the latest release that you can download, ignore the Windows Defender notification, you can check the code to see that it's not malicious. Select the version that matches your operating system. I am unable to test the Mac OS X and Linux versions but they are what Unity spat out, but Windows x64 should work. 

If you want to compile from source, download the repo and Unity editor version 2019.4.1f1. Open it and the project should work correctly. You can play it within the Unity editor or create your own copy using File -> Build and Run (or the shortcut ctrl-B).

## Game controls

To move, use the WASD keys to move up, left, down, right. You can use the cursor to aim your crosshair. 

Pressing left-click will fire the gun you have equipped. You can change the gun using the number keys (the ones above QWERTY, not the numpad). 1 equips the pistol, 2 equips the shotgun, 3 equips the SMG. 

Pressing right-click will swing a sword for short-range melee damage. 

The keys Z, X and C are bound to toggle your abilities. In the final game you would be able to swap these out through a menu, but this was not implemented yet. You can swap them in the Unity editor if you really want to play around with them. 

The E key interacts with objects in the direction your cursor is pointing. This currently allows you to talk to other characters in the game. 

In dialogue, you can press E to immediately complete the sentence that is being typed, or proceed to the next dialogue if the typewriter effect is done. Sometimes you are given options for a response which you can select with your cursor. 

Pressing escape will pause the game until you press it again. Pressing M on any scene other than the title screen will return you to the title screen. 

# Contributions

Most code is written and (c) Steven Franklin 2020. All rights reserved. 

Many of the graphics and textures are also by me. The graphic of the main character Daniel (which is seen in dialogue boxes) is completed by Ryan Olds. All the rest are placeholder art I designed. 

All characters and plot elements are by Tristan Hatchell. 

I use the xNode package for dialogue trees, this can be found [on GitHub here](https://github.com/Siccity/xNode). This is distributed under the MIT License. 

Additional thanks goes to the great people behind years-old StackOverflow and Reddit posts, as well as Spotify and various energy drink companies :]
# Lunacid---Ultrawidescreen-fix
IMPORTANT - thanks to the game's developer work, this patch is now obsolete and is now archived. Thank you Kira for your work.

Old description:
A basic fix that prevents from HUD being stretched outside the screen countries to be used as temporary solution to a problem as we wait for the game developer to hopefully fix it.

![preview.jpg](/Screens/preview.jpg)

What it does
------
* What this ultrawidescreen fix does is, it hooks a Menus class to apply hotpatching when the menu is being loaded. Then the MonoBehaviour changes the CanvasScaler to to use Unity's Extend method instead of % match between width and height. This should normally be fixed by repositioning elements properly anchoring elements in Unity prefab.

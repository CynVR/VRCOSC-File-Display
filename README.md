# VRCOSC-File-Display
File display module for VRCOSC v3

Module should be self explanatory, set a file path in the options for the file to be displayed. 
If you want the text to be shown continuously while the file isn't empty, enable 'DisplayUntilEmptied'. 
If you want the text to update even when the file is empty, enable 'UpdateOnEmpty'.

If you aren't seeing any text, check the console and make sure the file path is valid. If you still aren't getting display, make sure the 'Content Changed' event is enabled, and the duration is set to a reasonable value.
If the module appears to be blocking lower priority modules from displaying, make sure you don't have both 'DisplayUntilEmptied' and 'UpdateOnEmpty' enabled as it will continuously update the module with blank text.

# Installation
Simply put the module's .nupkg file into /%localappdata%/VRCOSC/packages and restart OSCVRC.

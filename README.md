# Itemgen for dominions 5
This program generates items for dominions 5. It automaticly generates the items in place of the vanilla items except the vanilla items that cant be removed
due to events. Currently the UI only works in windows and is work in progress.

## Short user guide
### Generating a standard itemgen
1. Get the release from github releases.
2. Unpack the zip
3. Run view.exe
4. generate the mod with the generate button

### Customizing itemgen
The ui provides basic functionality for changing the different variables that control itemgen.

In the homescreen:
  - Debugging -> write additional informantion in the DM
  - MagicGen -> enable magicgen integration THIS NEEDS the mergetool otherwise the weapons are wrong (on the todo)
  - Scaling increment -> amount of points a attribute needs to spend before more expensive scaling
  - Amount of items -> amount of items to generate more than 887 generates too many items leaving a incorrect DM
  - default points -> the points a item gets just for existing before factors like construction level and gemcost
  - point multiplier -> global point multiplier the balance slider for itemgen
  
In the attribute screen:
  - Here you can see al the attributes loaded from the AtributeList.txt in the input folder.
  - You can also add / delete attributes. Saving is WIP and not recommended.

Without the UI you have the input folder.
This is where all the data for itemgen is stored. Removing files from the input folder would most likely result in itemgen crashing.
Adding more sprites simpley: add more .tga files in the correct folder and they will appear in the mod.

## For help, comments and feedback
Add a issue here on github or contact me in my channel in the dominionsmods discord.

# Many thanks
Illwinter for making dominions 5.
The absolute lads in the immersion server (you know who you are and you know why i want to thank you).
The first testers for the test game ran in immersion.
The guy who wanted my mod for a game with every random generator (and his welcome input).
Everyone who might contribute to itemgen. 
And everyone who has fun with itemgen.

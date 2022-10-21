This is a simple mod for Terra Invicta, which allows modders to add new 
icons and illustrations without building asset bundles in Unity.

Usage:

1. In your mod folder create a sub-folder "sprites"
2. Copy the images you want to use into the foler
3. Create a JSON file with the sprites list:
  
```json
[
 {
 "Name": "Laurals",
 "Path": "skull_laurals.png"
 },
 {
 "Name": "Warhammer1",
 "Path": "warhammer1.png"
 }
]
   ```

4. Save it as sprites.txt in "sprites" folder (.txt, because Terra Invicta 
mod loader tries to process all .jsons)
5. Use it in mod templates, for example, TIProjectTemplate:
```json
"iconResource": "Laurals",
"completedIllustrationPath": "Warhammer1"
```
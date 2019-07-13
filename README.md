# Advanced Scene Search 
A tool for unity allowing you to search for specific GameObjects in the scene, using a variety of filters


Ever have an object in your scene with one awkward setting on and you have to go through every object to find it?

Or, search for all MeshRenderers that have a specific material, and are set to cast shadows!

Or even, all the objects with exactly 5 audiosources, a mesh renderer, and the word HACK in the name?

Well, this tool should solve your problems!

![Advanced Scene Search](http://jacobkeane.co.uk/wp-content/uploads/2017/08/170815_Unity_Kv6wv31.png)

![Advanced Scene Search](https://i.imgur.com/YdhpBbP.png)


---

**Filters**
* Name
  * Exact match
  * Contains the text
  * Case sensitity
  * Regex
* Components
  * Specific combination of components
  * Specific amount of components
  * Specific serialized values
* Tags
* Layers
* Shaders

You can also drag any GameObject in from the scene, and have it auto-fill the settings for that GameObject, in order to find similar GameObjects!

Should work from Unity 5 to Unity 2019, but let me know if you have any problems!


---
 
## How do I use it?

Checkout / grab a zip of this repo, and place the files from it somewhere in your project. 
I'd recommend throwing it under something like Assets/Third Party/AdvancedSceneSearch/, just to not have all of the editor tools and plugins clutter up your repo over time.

Then, go to Tools -> Advanced Scene Search to open the scene search window, OR select Tools -> Advanced scene search launch window, which will give you a tiny dockable window (I normally dock it at the bottom of the hierarchy), which you can drag a GameObject onto to launch the search window and autofill settings from that object

![Advanced Scene Search Launch Window](https://i.imgur.com/jGLvv4v.png)


The "?" button lets you select individual serialized values to srearch for - enable it to pick which values to list, and when it's disabled, you can set those values.

![Advanced Scene Search Launch Window](https://i.imgur.com/wdmyFGA.gif)


# Advanced Scene Search 
A tool for unity allowing you to search for specific GameObjects in the scene, using a variety of filters


Ever have an object in your scene with one awkward setting on and you have to go through every object to find it?
Or say you want to find every UI text using a specific font?
Or even, all the objects with exactly 5 audiosources, a mesh renderer, and the word HACK in the name?

Well, this tool should solve your problems!

![Advanced Scene Search](http://jacobkeane.co.uk/wp-content/uploads/2017/08/170815_Unity_Kv6wv31.png)

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
  * Specific serialized values changed from their default
* Tags
* Layers
* Shaders

You can also drag any GameObject in from the scene, and have it auto-fill the settings for that GameObject, in order to find similar GameObjects!

Should work from Unity 5 to Unity 2019, but let me know if you have any problems!

TerrainGenerator
================

*Simulates basic landscapes based on a generated height map.*

About
-----
The TerrainGenerator allows you to create and view height maps that simulate landscapes.

Usage
-----
TerrainGenerator consists of a Settings and Viewer window.

The Settings window has three tabs: Generate, Modify, and Info. Use the Generate tab to set the terrain detail level (n) and the genetic data (n floats). These float values specify the amount of variance on different levels of detail (i.e. frequency domain). Press the Generate button to create a new terrain height map. Use the Modify tab to change the terrain rotation, level of detail, lighting, and texturing. Images can be mapped by storing then in the executable's folder naming it textureX.jpg, X in (0, 15). The Info tab provides rendering stats.

The Viewer window can be used to navigate. Press a mouse key to activate camera rotation. Use arrow keys to move around.

Build
-----
TerrainGenerator can be compiled using the 'TerrainGenerator.csproj' Visual Studio project file. Terrain requires DirectX 9 and the Microsoft .NET 2.0 Framework.

License
-------
AstarViz is licensed under the terms of the GNU General Public License, see the included LICENSE file.

Author
------
[Leo Vandriel](http://www.leovandriel.com)
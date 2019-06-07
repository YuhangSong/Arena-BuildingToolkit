-------------
PROJECT SETUP
-------------

	For best look and performance, set Color Space to Linear (Edit->Project Settings->Player->Other Settings->Color Space).

	It is also recommended to set Rendering Path of your main camera to "Deferred"


-----------
DEMO SCENES
-----------

	Open Sun_Temple/Scenes/DemoScene.unity and hit play to run around in scene and familiarize yourself with environment and assets.

	Use mouse for looking around and W-A-S-D for moving. Mouse Click to open/close doors. ESC to toggle cursor lock/centering.

	Sun_Temple/Scenes/BuildingPrefabOverview.unity contains overview of prefabbed buildings laid on plane. 

	There are 3 versions of each building - ones filled with interior structures and props, empty ones with just walls and "hollow" buildings with backfaces removed. Hollow ones are least performance-heavy.



--------------------
PREFABS AND MATERIALS
--------------------

	Sun_Temple/Prefabs folder contains most of the objects you might be interested in. These are:

	> Backgrounds -- for background mountains, buildings, clouds and other objects that are only supposed to be seen from afar
	> Buildings_Prefabbed -- prefabbed buildings that are ready to be dragged & dropped in your scene.
	> Modules_Common -- individual modules from which prefabbed buildings are assembled.
	> Modules_Structs -- modules for structures that are not necessarily buildings - walls, scaffoldings, support structures, curbs etc
	> Modules_Temple -- including modules for main temple building as well it's surrounding buildings and towers
	> Props -- exterior and interior props
	> Natural -- bushes, shrubs, grasses, trees rocks and various natural formations
	> Decals -- decals, wall paintings, murals, puddles etc.

	You can find all materials in Sun_Temple/Content/Meshes/Main/Materials folder. Most of them use Standard shader (Roughness Setup). Larger buildings and walls use VertexBlend shader that uses vertex color to blend between two sets of textures. You can tweak blend by painting vertex color and tweaking "BLEND_Choke" and "BLEND_Crispyness" values of respective material.



-------
SCRIPTS
-------

	All scripts that come with this asset are specifically made for making demo environment more interactive and optimizing it's performance (mostly culling objects). You still might find use for them in your project, but please note that they are not intended to be general purpose solutions to problem X or Y or main subject of this asset.

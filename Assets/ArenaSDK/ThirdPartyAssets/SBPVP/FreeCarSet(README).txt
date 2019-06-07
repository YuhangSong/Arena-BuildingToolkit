2016 SpyBlood Productions
Email : melvinjames885@gmail.com

Thank you for downloading!

this is a car control setup I've provided for you. It has everything you need, with everything
thoroughly explained with comments in the code. The best part, it's free! It also features 2 low poly race cars I made that are 
an example for this set, but feel free to use them in other projects. I mainly intended them for
non-commerical use, but if you do want to use them or this package commerically, some credit would be 
greatly appreciated.

Setting up your vehicle:
Even though the prefabs are present, some of you might want to make the cars from the ground up from scratch.
Here's how:

1. Start with a fresh scene by doing file->new scene,then add a terrain. (GameObject->3D Object-> Terrain).

2. Place any vehicle in the scene from the models folder on the terrain you created.

3. select the Car_Body object, and add a box collider to it(Component->Physics->BoxCollider).

4. select all the wheels except for one, (I recommend the WheelFL_Mesh you leave), and delete them. we dont need them.

5.double click the remaining 1 wheel you have, and click GameObject->Create Empty. you now have your container for that wheel.

6.make the wheel mesh object a child of the new created gameObject.

7. name the new GameObject "WheelFL".

8. duplicate that object, name it "WheelFR", and drag it using the axis widget to the other side of the car, alligning it with the other wheel well

9. duplicate both those wheels and drag them to the back wheel wells.

10.name the one under "WheelFl", "WheelRL", and the one under "WheelFR","WheelRR".

11.Double click the entire car object and go GameObject->Create Empty.

12.parent and center that object if it's not on the car, and name it "WheelTransforms", and parent all the wheel objects to "WheelTransforms".

13. duplicate it, and name the duplicate "WheelColliders";

14.delete all the child objects from the wheels in WheelColliders. we dont need 2 sets of wheel meshes.

15. Add a Rigidbody component to the main car object. make it's mass to 1000.(Component->Physics->Rigidbody)

16.Add a WheelCollider component to the empty wheel objects. (Component->Physics->WheelCollider).

17.edit the wheel colliders to your liking.

NOTE: (in my opinion) this is the best WheelCollider setup
mass: 100 - 150 ish
FORWARD FRICTION extremum slip: 0.3
stiffness: 3

SIDEWAYS FRICTION extremum slip: 0.2
stiffness: 3.5;



18. go to the car object, and drag the CarControl Script onto it.

19. assign the wheelcolliders in the wheels array slot, and the transforms into the tires array slot. PUT IN ORDER!

20. if you want mobile controls, check the Mobileinput box, and drag the MobileCarButtonSetup object into the scene.

21. Next, find your main camera object, and put the CarCameraScript on it. Assign the car you just created to the "Car" Transform variable.

click play, And there you have it! a car you can drive around and control with your keyboard or your phone!

Again, thanks for downloading this package, and keep on making games!
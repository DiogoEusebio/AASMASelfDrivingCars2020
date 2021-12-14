# AAMAS Project 2019/20 - Autonomous Driving Agents

Group 34 - Alameda

Student Name  | Number
------------- | ------
Diogo Eusébio | 87650
Rodrigo Lima  | 83559
Simão Nunes   | 86512


Video Demo url: www.youtube.com/watch?v=i_FMALtMSXk

1) Requirements
In order to run this project you will need:
- Unity Developer Tool Version 2019.3.11 (other updates should work aswell, if part of version 3)


2) Opening the project and running
After retrieving the "034_project.zip" file, unzip it, open unity hub and select Add. Now just select the folder "034_project". Then you just need to select the project from the project list to open it.
To make sure you have the right version, in unity hub you can go to the installs menu and choose a specific version to install and then in the projects menu, select the specific version to use for the project.
Once opened, the simulation runs by using the play icon (in the top middle area of the software interface), it can also be paused by using the pause button (in the top middle area aswell) and stopped by clicking the play button again.


3) Testing
In order to make the testing process easier, it is possible to change variable values without going to the code and changing them manually there, that is done by going to the Hierarchy Interface (on the left) and selecting an object, which should display (on the right) an Inspector Interface.
Not all scene objects can/should be changed, so we next cover which objects can be changed and how to do it.

3.1) Car Generation
CarGeneratorObject is the object responsible for generating the cars in the environment. 
The variables that should be changed are:

 - Random Car: if toggled will randomize the car type to be generated next
 - Car Type: type of car (0 -> Individual; 1 -> Mini Cooper; 2 -> Bus; 3 -> Truck)
 - Car Quantity: total number of cars that will be generated
 - Start Node: route starting node index (range 0-216)
 - End Node: route ending node index (range 0-216)
 - Spawn Speed: rate at which a new car is generated (default is 2 seconds)
 - Delete On End: if toggled will delete the cars from the scene once they reach their end node

 Note1: Car Type value will only be used if Random Car box is not toggled
 Note2: End Node values will only be used if the cars used do not have their random route box toggled


3.2) Traffic Lights
TrafficLightManager is the object responsible for managing the environment traffic lights.
The variables that should be changed are:

 - All Green: if toggled will turn all traffic lights to green (to test without traffic light system)
 - Change Time: time for which the green light is on (default is 3 seconds)
 - Reset Time: time for which all traffic lights are red (default is 1 second)


3.3) Car Movement 
Cars is the object that holds the car samples that can be generated. Under that object you can edit
each car properties by selecting it.

Car correspondence:
	
 Car1 -> Individual (Red Car)
 Car2 -> Mini Cooper (Blue Car)
 Car3 -> Bus (Yellow Car)
 Car4 -> Truck (Green Car)

The variables that should be changed are:
	
 - Static Edge Cost: represents an agent's wish to make use of a certain path, regardless of precise timing, used by other agents to simulate cyclic traffic hotspots
 - Dynamic Edge Cost: represents a weighted value of how much traffic a car produces on the edge it's traversing
 - Initial Speed: speed at which the car moves
 - Random Path: if toggled will allow the car to move randomly through the environment (ATTENTION: WON'T USE HEURISTICS TO OPTIMIZE ROUTE!)
 - Gas Multiplier: can be seen as a medium gas consumption value
 - Passenger Cap: passenger capacity of the car
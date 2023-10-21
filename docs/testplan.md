# Game of Life (Nyulak és Rókák) Test Plan 

The purpose of this test plan is to ensure that the both the simulation and the application responsible for handling and displaying it are working as intended. 

Some of the basic functionality like Sim creation, movement and deletion are covered with [unit tests](../GameOfLifeSimTest/GirdTest.cs)
that run automatically with each push to GitHub via a [workflow](../.github/workflows/dotnet.yml).
> 'Sims' and 'Entities' can be used interchangeably within the context of the project.

The rest must be observed through the GUI of the application.\
Usage of the GUI is covered [here](./window.md).


## Requirements

To run the application the [.NET 7](https://dotnet.microsoft.com/en-us/download/dotnet/7.0) runtime is needed.\
The [latest release](https://github.com/Toarexer/GameOfLife/releases/latest) can be run by executing the `dotnet GameOfLifeApp.dll` command in the directory where the DLL is located at. 

The application consists of two main parts, the simulation libraries and the desktop application.
The libraries only provide the logic for the desktop application and they must be managed and displayed by it.
The Desktop application depends both on these libraries and the [GtkSharp](https://github.com/GtkSharp/GtkSharp) wrapper library.
All of these are included within the release.

Building from source requires the [.NET 7](https://dotnet.microsoft.com/en-us/download/dotnet/7.0) SDK.
The [GtkSharp](https://github.com/GtkSharp/GtkSharp) library is included within the project as a NuGET package.


## Expected Behavior

The following sections will detail the expected behavior of the crucial parts of the simulation.

The Sims and the [GameManger](../GameOfLifeSim/GameManager.cs) can take advantage of a global [logger](../GameOfLifeLogger/Logger.cs) that can have multiple logging functions.\
By default an info and an error logger is given to it by the application [window](../GameOfLifeApp/Window.cs) that makes the info and errors of the current step appear in it.\
Sims may print out information about themselves this way and the [GameManger](../GameOfLifeSim/GameManager.cs) may print out the exceptions of the sims as errors.

## [Grid](../GameOfLifeSim/Grid.cs)

- If there is at least **1** grass on the grid, it should eventually fill the whole grid. 
- If there is only **1** rabbit, it should roam for **5** rounds, then die. 
- If there is only **1** rabbit and at least **1** grass, it should eat the grass starting from the 2nd round. 
- If there is only **1** fox, it should roam for **8** rounds, then die.
- If there are at least **2** rabbits, they should make a pair and create new descendant at a empty tile.
- If there are at least **2** foxes, they should make a pair and create new descendant at a empty tile.
- If there are rabbits and foxes, the foxes should eat the rabbits when the time comes.
- If there are rabbits, foxes, and at least **1** grass, the simulation should run.
- If the animals are dead, only grass should remain on all tiles.
- If there are any errors, it should appear in the **errors** tab.
- If there are any logging, it should appear in the **log** tab.

> About loading in a grid into the application please check the [user manual](./window.md)


## [Fox](../GameOfLife/Entities/Fox.cs)

### Constructor

- Check the position of the new Fox if it has spawned at the specified position. 
- Check if the Fox's properties were correctly initialized.

### Update

- Verify that the Rabbit's properties are updated.
- Check that the Rabbit moves to the next position.
- Check that the Eat method works correctly and the HP increases.
- Check if the Rabbit dies when the HP reaches **0**.
- Check if the Age increases by **1**.
- Check if the Invincibility, MatingCooldown, Hp decreases by **1**.
- Ensure that new descendants only spawn under appropriate conditions.
- Check if the mating pairs are handled correctly.

### ShouldEat

- Check if the ShouldEat method returns **true** when the HP is less then MaxHP.
- Check if the method returns **false** when the HP equals MaxHP.

### Eat

- Check if the Eat method works as intended.
- Check if the HP is increasing after eating.
- Check if the method selects the optimal Grass if available.
- Check if the method increases the HP by the grass's nutritional value.

### ShouldDie

- Check if the method returns **true** when the HP reaches **0**.
- Check if the method returns **false** when the HP is greater than **0**.
- Ensure that the Fox dies when the Age is greater than **50**.

### ShouldCreateDescendant

- Check if the method returns **true** when the conditions are met.
- Check if the method returns **false** when the conditions are not met.

### NewDescendant

- Verify that the method creates a new Fox object when the conditions are met.
- Check if the created descendant was created with default properties.

### Move

- Check if the method correctly sets the NextPosition.
- Check if the Fox moves randomly if it should.
- Verify if the Fox moves appropriately when in a mating pair.

### CompareTo

- Check if the method returns **-1** when this Fox's Age is less than the other's.
- Check if the method returns **0** when this Fox's Age are equal to the other's.
- Check if the method returns **1** when this Fox's Age are greater than the other's.


## [Rabbit](../GameOfLife/Entities/Rabbit.cs)

### Constructor

- Check the position of the new Fox if it has spawned at the specified position. 
- Check if the Fox's properties were correctly initialized.

### Update

- Verify that the Fox's properties are updated.
- Check that the Fox moves to the next position.
- Check that the Eat method works correctly and the HP increases.
- Check if the Fox dies when the HP reaches **0**.
- Check if the Age increases by **1**.
- Check if the Invincibility, MatingCooldown, Hp decreases by **1**.
- Ensure that new descendants only spawn under appropriate conditions.
- Check if the Mating pairs are handled correctly.

### ShouldCreateDescendant

- Check if the method returns **true** when the conditions are met.
- Check if the method returns **false** when the conditions are not met.

### NewDescendant

- Verify that the method creates a new Fox object when the conditions are met.
- Check if the created descendant was created with default properties.

### ShouldEat

- Check if the ShouldEat method returns **true** when the HP is less then MaxHP.
- Check if the method returns **false** when the HP equals MaxHP.

### ShouldDie

- Check if the method returns **true** when the HP reaches **0**.
- Check if the method returns **false** when the HP is greater than **0**.
- Ensure that the Rabbit dies when the Age is greater than **30**.

### CanMove
- Verify that the CanMove method correctly determines if the Rabbit can move.
- Check if the method returns true when there are no nearby foxes and the Rabbit has no mating partner.
- Ensure that the method returns false when conditions do not allow the Rabbit to move.

### Move

- Check if the method correctly sets the NextPosition.
- Check if the Rabbit moves randomly if it should.
- Verify if the Rabbit moves appropriately when in a mating pair.

### CanBeEaten

- Verify the method correctly determines if the Rabbit can be eaten by a Fox.
- Check if the method returns **true** when the Rabbit is not invincible and the HP is greater than **0**.
- Check if the method returns **false** when the Rabbit is invincible or the HP is less or equals to **0**.

### GetEaten
- Verify that the GetEaten method returns the correct nutritional value when the Rabbit is eaten.
- Check if the Rabbit's HP is set to **0** after being eaten.

### CompareTo

- Check if the method returns **-1** when this Rabbit's Age is less than the other's.
- Check if the method returns **0** when this Rabbit's Age are equal to the other's.
- Check if the method returns **1** when this Rabbit's Age are greater than the other's.


## [Grass](../GameOfLife/Entities/Grass.cs)

### Constructor

- Check the position of the new Grass if it has spawned at the specified position. 
- Check if the Grass's properties were correctly initialized.

### Update

- Verify that the Update method correctly updates the Grass's properties.
- Ensure that the Grass's age increases by **1**.

### ShouldDie

- Verify that the method always returns false.

### NewDescendant

- Check that the method returns a new Grass with appropriate position when the conditions are met.

### GetEaten

- Check that the method correctly returns the nutritional value based on the Grass's State.
- Check if the State of the Grass decreases by **1** when it has been eaten.

### CompareTo

- Check if the method returns **-1** when this Grass's Age is less than the other's.
- Check if the method returns **0** when this Grass's Age are equal to the other's.
- Check if the method returns **1** when this Grass's Age are greater than the other's.

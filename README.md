# Game of life (Nyulak és Rókák)

The simulation is based on the rule system of John Conway's Game of Life, but with multiple modifications to it.

Please check the [User Manual](./docs/window.md) for instructions regarding the usage of the program.

## Meet the Sims

The simulation can easily be extended with multiple creatures, which are referred to as **Sims** or **Entities**.\
Currently the following three are implemented for demonstration:
- [Fox](./GameOfLife/Entities/Fox.cs)
- [Rabbit](./GameOfLife/Entities/Rabbit.cs)
- [Grass](./GameOfLife/Entities/Grass.cs)

Their expected behavior is detailed in the [Test Plan](./docs/testplan.md).

The simulation is non-deterministic and Sims may decide their next move based on random chance.

## Main features of the Sims

Animals focus on reproduction, hunting, and surviving in their environment.\
Foxes can eat rabbits when they are hungry, and breed in pairs.\
Rabbits can move, eat grass, and breed in pairs.\
Grass has different states of growth and it can spread to neighboring cells randomly.\
Each entity has its own rules and interactions with others, which increases the complexity of the simulation.

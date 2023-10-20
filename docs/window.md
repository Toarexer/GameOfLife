# Application Window and it's Usage

The window of the application is made with the [Gtk 3.22](https://github.com/GNOME/gtk/tree/gtk-3-22) wrapper, [GtkSharp](https://github.com/GtkSharp/GtkSharp).

## Controls

![Game of Life App with markings](./imgs/gol-app-marked.png)

1. Open a file dialog to load a csv file with the correct format to set the initial state of the grid. 
2. If autorun is checked the simulation will step itself with the specified interval.
3. Specifies the interval between automatic steps in milliseconds.
4. Steps the simulation forward by one.
5. Exits the application.

- List of entity types - This will display the type of Sims currently on the grid.
- Simulation Grid - This is where the Sims are displayed. Each cell can hold at maximum 8 Sims at once. If a cell has multiple Sims then they are represented a lines.
- Info and error logs - This can be used by the [GameManager](../GameOfLifeSim/GameManager.cs) and the Sims to log info and errors of the current step.

### Loaded Grid

![Loaded grid](./imgs/gol-app.png)

## CSV Format

The first line specifies the the size of the grid with the `{width}x{height}` format.
The following line specify the Sims that should be placed into the grid with the `{name},{x},{y}` format.\
> The names of the Sims are case sensitive and the positions start from 0.

### Example

```csv
16x16
Grass,6,6
Grass,7,15
Grass,10,7
Grass,15,2
Rabbit,5,5
Rabbit,10,2
Rabbit,11,2
Rabbit,15,6
Fox,3,3
Fox,9,7
```
> The grid is 0 indexed.

### CSV Loading Errors

There are multiple errors that can occur when the csv is incorrectly formatted.\
In these cases a popup window will appear with an error message such as these:

The first line contains a decimal number.\
![Invalid number error](./imgs/gol-error-invalid-num.png)

The third line is missing the y position.\
![Invalid line error](./imgs/gol-error-invalid-line.png)

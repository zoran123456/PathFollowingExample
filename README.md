# Path Following Example

## Overview
A C# console application that solves path-following problems using a backtracking algorithm. The application finds a path through ASCII character mazes, collecting letters along the way to form specified words.

### What is Path Following?
In this project, path following refers to navigating through a 2D grid (maze) represented by ASCII characters. The solver must:
- Start at a designated position marked by `@`
- Follow valid paths made of characters like `-`, `|`, `+`, etc.
- Collect letters (A-Z) along the path to form a target word
- Reach the end position marked by `x`

The algorithm uses backtracking to explore possible paths until it finds a valid solution.

## Features
- Interactive console-based UI
- Ability to choose between different board layouts
- Visual animation showing the path-finding process in real-time
- Multiple pre-defined example boards with solutions
- Comprehensive unit tests to verify algorithm correctness

## Prerequisites
- .NET Core 3.1 or higher
- Visual Studio 2019 or higher (optional, for development)

## Installation
1. Clone this repository or download the source code
2. Open the solution file (`PathFollowingExample.sln`) in Visual Studio, or use the command line

## Usage

### Running the Application
Navigate to the **PathFollowingUI** folder in your terminal and run:

```
dotnet run
```

### Application Flow
1. Select a board (1, 2, or 3) when prompted
2. Choose whether to display the path-following animation (Y/N)
3. The algorithm will find a solution and display the results
4. Press any key to return to the main menu

### Board Format
Boards are text files containing ASCII characters:
- `@` - Starting position
- `x` - Ending position
- `-` - Horizontal path segment
- `|` - Vertical path segment
- `+` - Path junction/intersection
- `A-Z` - Letters to collect along the path

Example board (board1.txt):
```
@---A---+
        |
x-B-+   C
    |   |
    +---+
```

### Running Tests
Navigate to the **PathFollowingSolverTests** folder and run:

```
dotnet test
```

## How It Works
The path-following solver uses a backtracking algorithm to explore all possible paths from the start position. It:

1. Begins at the `@` position
2. Follows valid path segments (`-`, `|`, `+`)
3. Collects letters when encountered, adding them to the solution word
4. Backtracks when it reaches dead ends or invalid paths
5. Succeeds when it reaches the `x` position and has collected the target letters

## License
This project is free to use and modify.

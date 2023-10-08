# unity-chess
Chess engine with integrated GUI. It allows for playing chess matches between two player in all variations: between two humans, a human and a computer, as well as between two computers. It was developed in Unity using the C# programming language.

The primary objective was to create a chess engine that is easy to understand in terms of how it operates. This approach differs from other chess engines that prioritize maximum performance at the expense of readability.

## Table of Contents

- [How to run](#how-to-run)
- [Documentation](#documentation)
  - [Structure](#structure)
  - [Features](#features)
  - [Testing](#testing)
  - [Estimated strength](#estimated-strength)
- [Technology stack](#technology-stack)

## How to run

To run the chess program, follow these steps:

1. Clone the repository: `git clone https://github.com/PrzemekBarczyk/unity-chess.git`
2. Open the Unity project using Unity Editor
3. Build the project for Windows
4. Launch the built executable to start the chess game

## Documentation

### Structure

The project is divided into two main layers:

- Presentation Layer - handles the graphical user interface (GUI) of the chess program
- Chess Engine Layer - is responsible for the chess engine's logic

### Features

The chess engine incorporates several important features:

- Alpha-Beta Pruning
- Quiescence Search
- Transposition Tables and Zobrist Keys
- Opening Book

### Testing

Includes the implementation of the Perft function, which verifies whether a given position generates the correct number of legal moves at various depths. This ensures the correctness of the engine's move generation.

### Estimated strength

The estimated strength of the created program is approximately 1600 Elo points. This estimation is based on a series of games played against computer opponents available on chess.com, which have Elo ratings. By analyzing the results of these games, it was possible to approximate the program's strength to be around 1600 Elo points, with a search time of five seconds per move.

## Technology stack

- Unity 2020.3.1f1
- C#

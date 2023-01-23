## Burning Forest Simulator

Final project for the Software Implementation (MOD002702 TRI1 F01CAM) for
my Master's Degree in Communication and Information Technology. The instructions are
contained within the PDF, however it is covered in brief below.

## Goals

The goal of assignmet is to design an implement a program that models the spread
of a fire across a 2-dimensional forest. Each cell can be in one of three given
states:
- burning, marked by an X,
- a normal tree, marked by an ampersand (&)
- or an empty space, after a fire has burned, marked by an underscore (_)

The fire spreads according to the state of adjacent cells and optional settings as
selected by the user (more on this down the page). This type of simulation is also
called a *cellular automaton*.

## Requirements
- Must be in C# and console-based
- Fire can only spread in cardinal directions
- Fire has a base 50% chance to spread to each adjacent cell

## Design
The program generates a 21x21 array containing character representations of a forest,
with trees represented by ampersand (&), empty plots reperesented by an underscore
(_) and a fire represented by an X. There are optional inputs for the user to select.
The optional settings are to activate wind and/or terrain. Selecting wind will cause
set the windspeed to a between 1 and 3, causing the fire to spread up to 1 to 3 cells
farther than normal in the direction of the wind (N, S, E or W). The terrain setting
sets a random number of cells to be either dry, causing the fire to spread to it 100%
if adjacent to a burning cell or wet, which reduces the chance to 25%. For more
information on the windspeed or terrain algorithms, consult Appendix 3 in the
`simulator-evaluation.pdf`.

The core functionality of the program is a list tuples with coordinates of actively burning cells, beginning with the middle
cell, (10, 10). The list of coordinates grows and shrinks according to the fire
spread. This greatly reduces computing time compared to scanning the base 2-D array
every time the user progresses a turn, and also uses memory more efficiently. When
the list is empty, the simulation is over and the user can either restart the program
or exit. 

## Closing remarks

I quite enjoyed this project, and it was my first attempt at creating anything real
with C#, which was a new language to me at the beginning of the term. I really like
the structure of the language, and how everything is a class. Coming from a
background that is primarily Python, it took a little getting used to to understand
how everything is a class. While in Python this may also be the case, technically,
(the old mantra, "everything is an object")it
is so abstracted away it is not something we often think about when developing in
Python. I found the syntax quite comfortable having also had experience with
JavaScript. It was also a good opportunity to help tutor my classmates, some of whom
had no familiarity with coding or programming at all and it was rewarding to see them
grow.

## Disclaimer
I am making this publicly available after grades have been submitted and the term is over.
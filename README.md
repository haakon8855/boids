# Boids
Boids are an example of an emergent system where complex behaviour 
emerges from seemingly unintelligent individuals. 
In this implementation, a boid is a dot on the screen 
following a direction with a set velocity.

Each boid follows three simple rules:

1. Avoid colliding into nearby boids
1. Follow the average direction of nearby boids
1. Steer toward the average location of nearby boids

When all boids follow these three rules, they will simulate a flock 
of birds, following each other while still avoiding collisions.

In addition to these three rules, a fourth rule is applied in 
this implementation in order to keep the boids on the screen, by making them
avoid the walls in addition to other boids.

# Requirements
## Python
The Python version of boids requires Python 3.9 or newer and the __PyQt5__ and __NumPy__ libraries:

    pip install pyqt5 numpy

## C#
The C# version of boids requires .NET (only tested with .NET 6.0) and XNA/MonoGame libraries for .NET.

# Configuration
No config file has been made, so the constructor in 
the _Boids_-class (Python) or _Boid_-class (C#) has to be manually edited in order to change input parameters.

# License
This code is protected under the [GNU General Public License 3.0](http://www.gnu.org/licenses/gpl-3.0.html)
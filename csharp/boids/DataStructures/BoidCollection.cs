using System;
using System.Collections.Generic;
using System.Linq;

namespace Boids.DataStructures;
public class BoidCollection
{
    public Boid[] Boids { get; private set; }
    public double[][] Positions => Boids.Select(b => b.Position).ToArray();
    public double[] Angles => Boids.Select(b => b.Angle).ToArray();

    public BoidCollection(int numBoids)
    {
        var dimensions = Drawer.Dimensions;
        Boids = new Boid[numBoids];
        Random rand = new Random();

        for (var i = 0; i < numBoids; i++)
            Boids[i] = new Boid(rand.Next(dimensions[0]), rand.Next(dimensions[1]));
    }

    public void Move()
    {
        var viewDist = Boids[0].Attributes["viewDist"];
        List<double[]> positions = new();
        List<double[]> headings = new();
        for (int i = 0; i < Boids.Length; i++)
        {
            positions.Add(Boids[i].Position);
            headings.Add(Boids[i].Heading);
        }

        List<double[]> visiblePositions;
        List<double[]> visibleHeadings;
        for (int i = 0; i < Boids.Length; i++)
        {
            visiblePositions = new();
            visibleHeadings = new();
            for (int j = 0; j < Boids.Length; j++)
            {
                if (Boids[i].DistanceTo(Boids[j]) < viewDist)
                {
                    visiblePositions.Add(positions[j]);
                    visibleHeadings.Add(headings[j]);
                }
            }
            Boids[i].Move(visiblePositions, visibleHeadings);
        }
    }
}
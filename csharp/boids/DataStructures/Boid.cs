using System;
using System.Collections.Generic;
using System.Linq;

namespace Boids.DataStructures;

public class Boid
{
    public double[] Position { get; private set; }
    public double[] Heading { get; private set; } = [0.5, 0.5];
    public double Angle => ((Heading[1] < 0) ? -1 : 1) * Math.Acos(Heading[0]);
    public Dictionary<string, double> Attributes { get; private set; } = new();

    public Boid(double positionX, double positionY)
    {
        Position = [positionX, positionY];
        Initialize();
    }

    private void Initialize()
    {
        // Velocity of boids
        Attributes["velocity"] = 3.5f;
        // Distance when boid will try to avoid other boids
        Attributes["viewDist"] = 65f;
        // View distance of each boid
        Attributes["tooCloseDist"] = 15f;
        // How hard boids will turn when reaching the edge
        Attributes["edgeAvoidance"] = 0.02f;
        // How hard boids will turn when avoiding other boids
        Attributes["avoidance"] = 0.12f;
        // How much boid will try to be in middle of its group
        Attributes["coherence"] = 0.01f;
        // How much boid will try to follow direction of nearby boids
        Attributes["conformity"] = 0.04f;
        // When will edge avoidance kick in
        Attributes["edgeOffset"] = 0.2f;
    }

    private double[] CalculateEdgeAvoidanceVector()
    {
        var offsetX = Drawer.Dimensions[0] * Attributes["edgeOffset"];
        var offsetY = Drawer.Dimensions[1] * Attributes["edgeOffset"];
        double[] edgeAvoidanceVector = [0, 0];

        // X component
        if (Position[0] < offsetX)
            edgeAvoidanceVector[0] = offsetX - Position[0];
        else if (Position[0] > Drawer.Dimensions[0] - offsetX)
            edgeAvoidanceVector[0] = Drawer.Dimensions[0] - offsetX - Position[0];

        // Y component
        if (Position[1] < offsetY)
            edgeAvoidanceVector[1] = offsetY - Position[1];
        else if (Position[1] > Drawer.Dimensions[1] - offsetY)
            edgeAvoidanceVector[1] = Drawer.Dimensions[1] - offsetY - Position[1];

        return edgeAvoidanceVector;
    }

    private double[] CalculateBoidAvoidanceVector(List<double[]> positions)
    {
        double[] avoidVector = [0, 0];
        List<double[]> tooCloseBoids = new();

        foreach (var position in positions)
        {
            if (DistanceBetween(Position, position) < Attributes["tooCloseDist"])
            {
                tooCloseBoids.Add(position);
            }
        }

        double[] diff;
        foreach (var position in tooCloseBoids)
        {
            diff = [
                position[0] - Position[0],
                position[1] - Position[1]
            ];
            avoidVector[0] = avoidVector[0] - diff[0];
            avoidVector[1] = avoidVector[1] - diff[1];
        }

        return avoidVector;
    }

    private double[] CalculateCenterVector(List<double[]> positions)
    {
        return [
            (positions.Select(p => p[0]).Sum() / positions.Count()) - Position[0],
            (positions.Select(p => p[1]).Sum() / positions.Count()) - Position[1]
        ];
    }

    private double[] CalculatePercievedHeadingVector(List<double[]> headings)
    {
        var percievedHeadingVector = new double[] { 0, 0 };
        foreach (var heading in headings)
        {
            for (int i = 0; i < percievedHeadingVector.Length; i++)
            {
                percievedHeadingVector[i] += heading[i];
            }
        }

        for (int i = 0; i < percievedHeadingVector.Length; i++)
        {
            percievedHeadingVector[i] /= headings.Count();
            percievedHeadingVector[i] -= Heading[i];
            percievedHeadingVector[i] *= Attributes["conformity"];
        }

        return percievedHeadingVector;
    }

    private void UpdateHeading(List<double[]> positions, List<double[]> headings)
    {
        var edgeAvoidanceVector = Normalize(CalculateEdgeAvoidanceVector(), Attributes["edgeAvoidance"]);
        var avoidVector = Normalize(CalculateBoidAvoidanceVector(positions), Attributes["avoidance"]);
        var centerVector = Normalize(CalculateCenterVector(positions), Attributes["coherence"]);
        var percievedHeadingVector = CalculatePercievedHeadingVector(headings);

        for (int i = 0; i < Heading.Length; i++)
        {
            Heading[i] = Heading[i] +
                          edgeAvoidanceVector[i] +
                          avoidVector[i] +
                          centerVector[i] +
                          percievedHeadingVector[i];
        }

        Heading = Normalize(Heading, 1f);
    }

    public void Move(List<double[]> positions, List<double[]> headings)
    {
        UpdateHeading(positions, headings);
        for (int i = 0; i < Position.Length; i++)
            Position[i] += Heading[i] * Attributes["velocity"];
    }

    public static double[] Normalize(double[] vector, double scale)
    {
        if (vector[0] == 0 && vector[1] == 0)
            return vector;
        var length = Math.Sqrt(vector.Select(x => x * x).Sum());
        return vector.Select(value => value / length * scale).ToArray();
    }

    public double DistanceTo(Boid boid)
    {
        return DistanceBetween(Position, boid.Position);
    }

    public static double DistanceBetween(double[] pos1, double[] pos2)
    {
        return Math.Sqrt((
            Math.Pow(pos2[0] - pos1[0], 2) +
            Math.Pow(pos2[1] - pos1[1], 2)
        ));
    }
}

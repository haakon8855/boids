namespace Boids.Models;

public class Boid
{
    public double Velocity { get; set; }
    public double ViewDist { get; set; }
    public double TooCloseDist { get; set; }
    public double EdgeAvoidance { get; set; }
    public double Avoidance { get; set; }
    public double Coherence { get; set; }
    public double Conformity { get; set; }
    public double EdgeOffset { get; set; }
}
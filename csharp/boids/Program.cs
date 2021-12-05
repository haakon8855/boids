using System;

namespace boids
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var boidsDrawer = new BoidsDrawer())
                boidsDrawer.Run();
        }
    }
}

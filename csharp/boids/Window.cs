using System.Numerics;
using Boids.DataStructures;
using Boids.Models.Config;
using Raylib_cs;

namespace Boids;

public class Window(Config config)
{
    private Config Config { get; } = config;
    private BoidCollection Boids { get; } = new(config);

    private Color BackgroundColor { get; } = new(
        config.Window.BackgroundColor[0],
        config.Window.BackgroundColor[1],
        config.Window.BackgroundColor[2]
    );

    public void Run()
    {
        Raylib.InitWindow(Config.Window.Width, Config.Window.Height, "Boids");

        while (!Raylib.WindowShouldClose())
        {
            Boids.Move();
            Draw();
        }

        Raylib.CloseWindow();
    }

    private void Draw()
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(BackgroundColor);

        var angles = Boids.Headings;
        var positions = Boids.Positions;
        var size = (float)Config.Boids.BoidSize;

        for (var i = 0; i < positions.Length; i++)
        {
            var posX = (float)positions[i][0];
            var posY = (float)positions[i][1];
            var angX = (float)angles[i][0];
            var angY = (float)angles[i][1];
            
            var vertex1 = new Vector2(
                posX + angX * size,
                posY + angY * size
            );
            var vertex2 = new Vector2(
                posX + (angY - angX) * size / 2,
                posY - (angX + angY) * size / 2
            );
            var vertex3 = new Vector2(
                posX - (angY + angX) * size / 2,
                posY + (angX - angY) * size / 2
            );
            
            Raylib.DrawTriangle(
                vertex1,
                vertex2,
                vertex3,
                new Color(
                    Config.Boids.BoidColor[0],
                    Config.Boids.BoidColor[1],
                    Config.Boids.BoidColor[2]
                )
            );
        }

        Raylib.EndDrawing();
    }
}
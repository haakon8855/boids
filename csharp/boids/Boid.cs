using System;
using System.Linq;

namespace boids
{
    public class Boid
    {

        private float[] _position = { 300f, 300f };
        private float[] _heading = { 0.5f, 0.5f };
        private float _velocity = 5f;

        public float[] GetPosition()
        {
            return _position;
        }

        public float GetAngle()
        {
            float angle = (float)Math.Acos(_heading[0]);
            return _heading[1] < 0 ? -angle : angle;
        }

        private void UpdateHeading()
        {
            RotateHeading((float)Math.PI / 50);
        }

        private void RotateHeading(float angle)
        {
            _heading[0] = (float)(Math.Cos(angle) * _heading[0]
                                - Math.Sin(angle) * _heading[1]);
            _heading[1] = (float)(Math.Sin(angle) * _heading[0]
                                + Math.Cos(angle) * _heading[1]);
            _heading = Normalize(_heading);
        }

        public void Move()
        {
            UpdateHeading();
            for (var i = 0; i < _position.Length; i++)
            {
                _position[i] += _heading[i] * _velocity;
            }
        }

        public static float[] Normalize(float[] vector)
        {
            float length = (float)Math.Sqrt(vector
                                      .Select(value => value * value)
                                      .Sum());
            return vector.Select(value => value / length).ToArray();
        }

    }
}
namespace boids
{
    public class Boid
    {

        private float[] _position = { 50f, 50f };
        private float[] _velocity = { 0.5f, 0.5f };

        public float[] GetPosition()
        {
            return _position;
        }

        public void Move()
        {
            for (var i = 0; i < _position.Length; i++)
            {
                _position[i] += _velocity[i];
            }
        }

    }
}
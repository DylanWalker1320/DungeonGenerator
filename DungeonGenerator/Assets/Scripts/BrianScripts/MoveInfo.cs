using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace BCho
{
    public class MoveInfo
    {
        public Vector2 Direction { get; set; }
        public float Speed { get; set; }

        public MoveInfo(Vector2 direction, float speed)
        {
            Direction = direction;
            Speed = speed;
        }
    }

}

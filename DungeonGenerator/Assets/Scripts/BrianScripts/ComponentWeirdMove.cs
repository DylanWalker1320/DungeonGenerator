using UnityEngine;

namespace BCho
{
    public class ComponentWeirdMove : MonoBehaviour
    {
        [SerializeField] Player player;
        private void Start()
        {
            player.SubMove(Effect);
        }

        private void Effect(MoveInfo info)
        {
            if (info.Direction.x > 0)
            {
                Debug.Log($"Broke ComponentB! Can move down now!");
                Destroy(this);
                return;
            }

            if (info.Direction.y < 0)
            {
                info.Direction = -1 * info.Direction;
            }
        }

        private void OnDestroy()
        {
            player.UnsubMove(Effect);
        }
    }

}
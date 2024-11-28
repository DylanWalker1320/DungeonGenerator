using UnityEngine;

namespace BCho
{
    public class ComponentFastMove : MonoBehaviour
    {
        [SerializeField] Player player;

        /* Treat this as OnCollisionEnter2D on the weapon pickup, and the weapon pickup calls smth like `ApplyComponents()` */
        private void Start()
        {   
            player.SubMove(Effect);
        }

        private void Effect(MoveInfo info)
        {
            info.Speed += 2;
        }

    }

    
}

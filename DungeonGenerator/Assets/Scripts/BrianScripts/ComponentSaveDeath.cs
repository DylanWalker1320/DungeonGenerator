using UnityEngine;

namespace BCho
{
    public class ComponentSaveDeath : MonoBehaviour
    {
        [SerializeField] Player player;

        /* maybe it even acquires a component list so later it can search to see if player has a certain component to stave off death */
        private void Start()
        {
            player.SubDeath(Effect);
        }

        private void Effect(DeathInfo info)
        {
            if (player.transform.position.x < 0)
            {
                /* Problem: 
                 * - the component breaks after being used, but what if there is 
                 *   another 'cursed' component that nullifies the effect?
                 * */
                Debug.Log("Player was saved by ComponentC! ComponentC breaks...");
                info.ShouldDie = false;
                Destroy(this);
            }
        }

        private void OnDestroy()
        {
            player.UnsubDeath(Effect);
        }
    }

}

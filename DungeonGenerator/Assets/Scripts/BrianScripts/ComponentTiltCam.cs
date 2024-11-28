using UnityEngine;

namespace BCho
{
    public class ComponentTiltCam : MonoBehaviour
    {
        [SerializeField] Player player;

        /* maybe it even acquires a component list so later it can search to see if player has a certain component to stave off death */
        private void Start()
        {
            player.SubInteract(Effect);
        }

        private void Effect(InteractInfo info)
        {
            Debug.Log($"Something weird happened!");
            Camera.main.transform.SetLocalPositionAndRotation(Camera.main.transform.position, Quaternion.Euler(0, 0, Random.Range(-10, 10)));
        }
    }

}

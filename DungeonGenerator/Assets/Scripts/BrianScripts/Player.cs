using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
namespace BCho
{
    public class Player : MonoBehaviour
    {
        UnityEvent<MoveInfo> m_eventMove;
        UnityEvent<DeathInfo> m_eventDeath;
        UnityEvent<InteractInfo> m_eventInteract;


        const float BASE_SPEED = 2.0f;

        private void Awake()
        {
            m_eventMove = new();
            m_eventDeath = new();
            m_eventInteract = new();
        }

        // Update is called once per frame
        void Update()
        {
            Vector2 direction = Vector2.zero;
            if (Input.GetKey(KeyCode.A))
            {
                direction += Vector2.left;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                direction += Vector2.right;
            }

            if (Input.GetKey(KeyCode.W))
            {
                direction += Vector2.up;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                direction += Vector2.down;
            }

            if (direction != Vector2.zero)
            {
                MoveInfo moveInfo = new(direction.normalized, BASE_SPEED);
                m_eventMove.Invoke(moveInfo);
                //Debug.Log($"{moveInfo.Direction} : {moveInfo.Speed}");
                transform.position += (Vector3)(moveInfo.Direction.normalized * moveInfo.Speed * Time.deltaTime);
            }

            /* Kill player */
            if (Input.GetKeyDown(KeyCode.Space))
            {
                DeathInfo deathInfo = new();  // by default, player should die
                m_eventDeath.Invoke(deathInfo);
                if (!deathInfo.ShouldDie)
                {

                    Debug.Log($"Player not die  :  ) ");
                }
                else
                {
                    Debug.Log($"Player die :(");
                }
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                InteractInfo interactInfo = new();
                m_eventInteract.Invoke(interactInfo);
                /* no use for interactInfo yet */
            }
        }

        public void SubInteract(UnityAction<InteractInfo> action)
        {
            m_eventInteract.AddListener(action);
        }

        public void UnsubInteract(UnityAction<InteractInfo> action)
        {
            m_eventInteract.RemoveListener(action);
        }

        public void SubMove(UnityAction<MoveInfo> action)
        {
            m_eventMove.AddListener(action);
        }

        public void UnsubMove(UnityAction<MoveInfo> action)
        {
            m_eventMove.RemoveListener(action);
        }

        public void SubDeath(UnityAction<DeathInfo> action)
        {
            m_eventDeath.AddListener(action);
        }

        public void UnsubDeath(UnityAction<DeathInfo> action)
        {
            m_eventDeath.RemoveListener(action);
        }
    }
}

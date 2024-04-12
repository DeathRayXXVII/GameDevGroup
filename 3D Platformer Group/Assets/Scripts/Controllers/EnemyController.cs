using System.Collections;
using System.Collections.Generic;
using Scripts.Managers;
using UnityEngine;
using UnityEngine.AI;

namespace Scripts.Controllers
{
    public class EnemyController: MonoBehaviour
    {
        public float lookRadius = 10f;
        private NavMeshAgent agent;
        public float remainingDistanceNum = 0.5f;
        public List<Transform> patrolPointList;
        private int i;
        public int patrolWaitTime;
        public WaitForSeconds wfsObj;
        private Animator anim;

        private Transform target;

        void Start()
        {
            target = PlayerManager.instance.player.transform;
            agent = GetComponent<NavMeshAgent>();
            anim = GetComponent<Animator>();
            //wfsObj = new WaitForSeconds(patrolWaitTime);
        }

        private void Update()
        {
            if (agent.velocity.magnitude > 0)
            {
                anim.SetBool("IsWalking", true);
            }
            else
            {
                anim.SetBool("IsWalking", false);
            }
            float distacne = Vector3.Distance(target.position, transform.position);
                    
            if (distacne <= lookRadius)
            {
                agent.SetDestination(target.position);
                    
                if (distacne <= agent.stoppingDistance)
                {
                    // Attack the target
                    FaceTarget();
                }
            }
            else
            {
                if (agent.pathPending || !(agent.remainingDistance < remainingDistanceNum))
                {
                    return;
                }
                StartCoroutine(Patrol());
            }
        }
        IEnumerator Patrol()
        {
            agent.destination = patrolPointList[i].position;
            yield return new WaitForSeconds(patrolWaitTime);
            i = (i + 1) % patrolPointList.Count;
        }
        
        void FaceTarget()
        {
            Vector3 direction = (target.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.x));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, lookRadius);
        }
        
        
    
    }
}

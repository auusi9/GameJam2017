using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StepWave : MonoBehaviour
{
    public const float runningSpeed = 2.5f;

    public const float walkignSpeed = 1.5f;

    public const float runningWave = 1f;

    public const float walkingWave = .75f;

    public NavMeshAgent navMeshAgent;

    public Survivor survivor;

    private WaitForSeconds waitForSecondsRunning = new WaitForSeconds(.6f);

    private WaitForSeconds waitForSecondsWalking = new WaitForSeconds(.6f);

    private int id;

    private bool firstStep;

    void Start()
    {
        id = SceneManager.current.AssignSurvivorIndex();

        StartCoroutine(ShowWave());
    }

    private IEnumerator ShowWave()
    {
        while(survivor == null)
        {
            yield return null;
        }
        Debug.Log("He trobat el survivor");
        while (survivor.alive)
        {
            if (navMeshAgent.speed < runningSpeed && navMeshAgent.speed > walkignSpeed)
            {
                int position = (SceneManager.NumWaves - 1) - (id * 2);
                if (!firstStep)
                {
                    position -= 1;
                }

                SceneManager.current.AddWave(transform.position, walkingWave, position);
                firstStep = !firstStep;
                yield return waitForSecondsWalking;
            }
            else if (navMeshAgent.speed > runningSpeed)
            {
                int position = (SceneManager.NumWaves - 1) - (id * 2);
                if (!firstStep)
                {
                    position -= 1;
                }

                SceneManager.current.AddWave(transform.position, runningWave, position);
                firstStep = !firstStep;
                yield return waitForSecondsRunning;
            }

            yield return null;
        }
    }
	
}

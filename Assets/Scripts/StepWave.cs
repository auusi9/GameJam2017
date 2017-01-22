using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StepWave : MonoBehaviour
{
    public const float runningSpeed = 2.5f;

    public const float walkignSpeed = 1.5f;

    public const float runningWave = 2f;

    public const float walkingWave = 1f;

    public NavMeshAgent navMeshAgent;

    private WaitForSeconds waitForSecondsRunning = new WaitForSeconds(.5f);

    private WaitForSeconds waitForSecondsWalking = new WaitForSeconds(.75f);

    void Start()
    {
        StartCoroutine(ShowWave());
    }

    private IEnumerator ShowWave()
    {
        while (true)
        {
            if (navMeshAgent.speed < runningSpeed && navMeshAgent.speed > walkignSpeed)
            {
                //SceneManager.current.AddWave(transform.position, walkingWave);
                yield return waitForSecondsWalking;
            }
            else if (navMeshAgent.speed > runningSpeed)
            {
                //SceneManager.current.AddWave(transform.position, runningWave);
                yield return waitForSecondsRunning;
            }

            yield return null;
        }
    }
	
}

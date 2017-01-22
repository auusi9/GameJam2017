using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SurvivorsManager : MonoBehaviour
{
    public enum State
    {
        wander,
        idle,
        follow,
        flee,
        die
    }

    float counter_strike = 0;
    private Transform player;
    public float radius = 1.0f;
    int survivors_inFlee = 0;
    public static SurvivorsManager singleton;
    
    Survivor[] survivors;
    // Use this for initialization
    private void Awake()
    {
        if (singleton != null)
        {
            Debug.LogWarning("Trying to add more than one SceneManager");
            Destroy(this);
        }
        else
        {
            singleton = this;
        }
    }
        void Start()
    {
        player = SceneManager.current.player;

        survivors = new Survivor[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            survivors[i] = new Survivor(transform.GetChild(i).gameObject);
            transform.GetChild(i).GetComponent<StepWave>().survivor = survivors[i];
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (counter_strike > 0.5)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (survivors[i].game_object == null)
                    for (int j = i; j < transform.childCount; j++)
                    {
                        if (survivors[j + 1] != null)
                            survivors[j] = survivors[j + 1];
                    }
                else
                {
                    switch (survivors[i].state)
                    {
                        case State.wander:
                            {
                                if (Detection(survivors[i].game_object, SceneManager.current.GetLastWaveDistance()) == true)
                                {
                                    ChangeState(survivors[i], State.flee);
                                }
                            }
                            break;
                        case State.flee:
                            {
                                if (Detection(survivors[i].game_object, 15f) == false)
                                {
                                    ChangeState(survivors[i], State.wander);
                                }
                                survivors_inFlee++;
                            }
                            break;
                        case State.follow:
                            {
                                //Debug.Log("FOLLOWING");
                                if (survivors[i].AlphaSurvivor != null)
                                {
                                    Vector3 point = Random.insideUnitSphere;
                                    point *= radius;
                                    survivors[i].game_object.GetComponent<NavMeshAgent>().destination = survivors[i].AlphaSurvivor.GetComponent<NavMeshAgent>().destination + point;

                                    if (Vector3.Distance(survivors[i].AlphaSurvivor.transform.position, survivors[i].game_object.transform.position) > 5)
                                    {
                                        ChangeState(survivors[i], State.wander);
                                    }
                                }
                                if (Detection(survivors[i].game_object, SceneManager.current.GetLastWaveDistance()) == true)
                                {
                                    ChangeState(survivors[i], State.flee);
                                }
                            }
                            break;
                        case State.die:
                            {
                                survivors[i].game_object.GetComponent<NavMeshAgent>().destination = survivors[i].game_object.transform.position;
                            }
                            break;
                    }
                }

                if (survivors[i].state != State.follow && survivors[i].state != State.flee)
                {

                    for (int j = i; j < transform.childCount; j++)
                    {

                        if (survivors[j].game_object != survivors[i].game_object)
                        {

                            if (Mathf.Abs(Vector3.Distance(survivors[j].game_object.transform.position, survivors[i].game_object.transform.position)) < 5)
                            {
                                if (survivors[j].AlphaSurvivor == null)
                                    survivors[j].AlphaSurvivor = survivors[i].game_object;

                                ChangeState(survivors[j], State.follow);
                                ChangeState(survivors[i], State.follow);
                                Debug.Log("IM HEREE");
                                
                            }
                        }
                    }
                }

            }
        }
        else
            counter_strike += Time.deltaTime;
        if (transform.childCount == 0)
            Debug.Log("YOU WIN BABY");

        if (survivors_inFlee == 0 && Time.time > Random.Range(60*5,60*10))
        {
            Debug.Log("All the survivors have scaped");
        }
        survivors_inFlee = 0;

    }

    bool Detection(GameObject game_object, float dist)
    {
        if (Mathf.Abs(Vector3.Distance(game_object.transform.position, player.transform.position)) <= dist)
        {
            return (true);
        }
        else
            return (false);
    }
    public void DetectOnAttack(float dis)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (Detection(survivors[i].game_object, dis) == true)
            {
                survivors[i].alive = false;
                Debug.Log("IM HERE");
                survivors[i].game_object.GetComponent<Animator>().SetBool("Die", true);
                Debug.Log("I:" + i);
                if (survivors[i] != survivors[transform.childCount-1])
                    survivors[i] = survivors[i + 1];
            }
        }

    }
    void ChangeState(Survivor survivor, State state)
    {
        if (state == survivor.state)
        {
            return;
        }

        else if (state == State.flee)
        {
            if (survivor.state == State.wander)
            {
                survivor.game_object.GetComponent<DestinationBehaviour>().enabled = false;
                survivor.game_object.GetComponent<NavMeshAgent>().Stop();
            }
            else if (survivor.state == State.follow)
            {
                survivor.game_object.GetComponent<DestinationBehaviour>().SetFollowing(false);
                survivor.game_object.GetComponent<DestinationBehaviour>().enabled = false;
                survivor.AlphaSurvivor = null;

            }

            survivor.game_object.GetComponent<Flee>().enabled = true;
            survivor.state = State.flee;
        }

        else if (state == State.wander)
        {
            if (survivor.state == State.flee)
            {
                survivor.game_object.GetComponent<Flee>().enabled = false;
            }
            else if (survivor.state == State.follow)
            {
                survivor.game_object.GetComponent<DestinationBehaviour>().SetFollowing(false);
                survivor.AlphaSurvivor = null;
            }

            survivor.game_object.GetComponent<DestinationBehaviour>().enabled = true;
            survivor.state = State.wander;
        }
        else if (state == State.follow)
        {
            if (survivor.state == State.wander)
            {
                if (survivor.AlphaSurvivor != null)
                    survivor.game_object.GetComponent<DestinationBehaviour>().SetFollowing(true);
            }
            if (survivor.state == State.flee)
            {
                survivor.game_object.GetComponent<Flee>().enabled = false;
            }


            survivor.state = State.follow;
        }
    }
}

public class Survivor
{
    public SurvivorsManager.State state = SurvivorsManager.State.wander;
    public GameObject game_object;
    public GameObject AlphaSurvivor = null;
    public bool alive = true;

    public Survivor(GameObject game_object)
    {
        this.game_object = game_object;
    }
}

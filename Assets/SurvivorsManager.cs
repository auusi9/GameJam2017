using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurvivorsManager : MonoBehaviour {

    enum State
    {
        wander,
        idle,
        flee
    }

    int counter_strike = 0;
    public GameObject player;


    class Survivor
    {
        public State state = State.wander;
        public GameObject game_object;

        public Survivor(GameObject game_object)
        {
            this.game_object = game_object;
        }
    }
   Survivor[] survivors;
	// Use this for initialization
	void Start () {
        survivors = new Survivor[transform.childCount];
		for(int i = 0; i < transform.childCount; i++)
        {
            survivors[i] = new Survivor(transform.GetChild(i).gameObject);
        }
	}
	
	// Update is called once per frame
	void Update () {
        for (int i = 0; i < transform.childCount; i++)
        {
            switch(survivors[i].state)
            {
                case State.wander:
                    {
                        if(Detection(survivors[i].game_object,3) == true)
                        {
                            ChangeState(survivors[i], State.flee);
                        }
                    }
                    break;
                case State.flee:
                    {
                        Debug.Log("FLEEEEEEEEEEEEEEEEEEEEEE");
                        if (Detection(survivors[i].game_object,15) == false)
                        {
                            ChangeState(survivors[i], State.wander);
                            
                        }
                    }
                    break;
            }
        }
	}

    bool Detection(GameObject game_object,int dist)
    {
        if (Mathf.Abs(Vector3.Distance(game_object.transform.position, player.transform.position) ) <= dist)
        {
            return(true);
        }
        else
            return(false);
    }

    void ChangeState(Survivor survivor, State state)
    {
        if(state == survivor.state)
        {
            return;
        }

        else if(state == State.flee)
        {
            if(survivor.state == State.wander)
            {
                survivor.game_object.GetComponent<DestinationBehaviour>().enabled = false;
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

            survivor.game_object.GetComponent<DestinationBehaviour>().enabled = true;
            survivor.state = State.wander;
        }
    }
}

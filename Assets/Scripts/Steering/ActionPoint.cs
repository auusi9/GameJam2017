using UnityEngine;
using System.Collections;

public class ActionPoint : MonoBehaviour {

    public int max_targets = 1;
    public int current_targets = 0;
    public bool full = false;

    // --------------------------------------------
    void Start()
    {
        current_targets = 0;

        if (current_targets == max_targets)
            full = true;
    }

    // --------------------------------------------
    void Update ()
    { }

    // --------------------------------------------
    public bool AddTarget()
    {
        bool ret = false;

        if(full == false)
        {
            current_targets++;
            ret = true;

            if (current_targets == max_targets)
                full = true;
        }
        
        return ret;
    }

    // --------------------------------------------
    public void DeleteTarget()
    {
        if (current_targets > 0)
        {
            current_targets--;

            if(full == true)
                full = false;
        }
    }
}

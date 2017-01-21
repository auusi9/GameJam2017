using UnityEngine;
using System.Collections;

static public class SteeringConf
{
	public const int num_priorities = 4;
}

abstract public class SteeringAbstract : MonoBehaviour {

	[Range(0, SteeringConf.num_priorities)]
	public int priority = 0;

}

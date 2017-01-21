using UnityEngine;
using System.Collections.Generic;

public class AiPerceptionEvent
{
    public enum senses { VISION, SOUND };
    public enum types { NEW, LOST };

    public GameObject go;
    public senses sense;
    public types type;
}

public class AiVision : MonoBehaviour {

    public Camera frustum;
    public LayerMask ray_mask;
    public LayerMask mask;
    public string search_tag;

    private List<GameObject> detected;
    private List<GameObject> detected_now;
    private Ray ray;

    // Use this for initialization
    void Start () {
        detected = new List<GameObject>();
        detected_now = new List<GameObject>();
        ray = new Ray();
    }
	
	// Update is called once per frame
	void Update () {
        Collider[] colliders = Physics.OverlapSphere(transform.position, frustum.farClipPlane, mask);
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(frustum);

        detected_now.Clear();

        foreach (Collider col in colliders)
        {
            if (col.gameObject != gameObject && GeometryUtility.TestPlanesAABB(planes, col.bounds))
            {
                RaycastHit hit;
                ray.origin = transform.position;
                ray.direction = (col.transform.position - transform.position).normalized;
                ray.origin = ray.GetPoint(frustum.nearClipPlane);

                if (Physics.Raycast(ray, out hit, frustum.farClipPlane, ray_mask))
                {
                    if (hit.collider.gameObject.CompareTag(search_tag))
                        detected_now.Add(col.gameObject);
                }
            }
        }

        // Compare detected with detected_now -------------------------------------
        foreach (GameObject go in detected_now)
        {
            if (detected.Contains(go) == false)
            {
                AiPerceptionEvent p_event = new AiPerceptionEvent();
                p_event.go = go;
                p_event.type = AiPerceptionEvent.types.NEW;
                p_event.sense = AiPerceptionEvent.senses.VISION;

                SendMessage("AiPerceptionEvent", p_event);
            }
        }

        foreach (GameObject go in detected)
        {
            if (detected_now.Contains(go) == false)
            {
                AiPerceptionEvent p_event = new AiPerceptionEvent();
                p_event.go = go;
                p_event.type = AiPerceptionEvent.types.LOST;
                p_event.sense = AiPerceptionEvent.senses.VISION;

                SendMessage("AiPerceptionEvent", p_event);
            }
        }

        detected.Clear();
        detected.AddRange(detected_now);
    }
}
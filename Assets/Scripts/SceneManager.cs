using UnityEngine;
using System.Collections;

public class SceneManager : MonoBehaviour
{
    /// <summary>
    /// Mida del Array que passem al shader (ha de ser el mateix que al shader)
    /// </summary>
    public const int ArraySize = 32;

    /// <summary>
    /// Distancia minima a la que sempre produim soroll
    /// </summary>
    public float minDistance = 1;

    /// <summary>
    /// Distancia maxima a la que podem produir soroll
    /// </summary>
    public float maxDistance = 10;

    /// <summary>
    /// Objecte MicrophoneListener que llegeix el valor del micro
    /// </summary>
    public MicrophoneListener microphoneListener;

    /// <summary>
    /// Distancia actual a la que "pintem"
    /// </summary>
    public float distance;

    /// <summary>
    /// Distancia a la que pintem el soroll anterior
    /// </summary>
    public float lastDistance;

    /// <summary>
    /// Transform del player
    /// </summary>
    public Transform player;

    /// <summary>
    /// Distancia del peu al centre del player
    /// </summary>
    public float footDistance = 0.25f;

    /// <summary>
    /// Si es el primer pas o el segon que anem fent
    /// </summary>
    public bool firstStep = true;

    /// <summary>
    /// Nom del vector del centre de la posicio al shader
    /// </summary>
    private int waveVectorID;

    /// <summary>
    /// Nom del vector del centre de la posicio anterior al shader
    /// </summary>
    private int oldWaveVectorID;

    /// <summary>
    /// Nom del array de linies al shader
    /// </summary>
    private int linesArrayID;

    /// <summary>
    /// Llargada del vector de linies al shader
    /// </summary>
    private int lenghtID;

    /// <summary>
    /// Distancia a la que pintem linies al shader
    /// </summary>
    private int distanceID;

    /// <summary>
    /// Distancia a la que pintem les linies anterior al shader
    /// </summary>
    private int oldDistanceID;

    void Awake()
    {
        waveVectorID = Shader.PropertyToID("_SonarWaveVector");
        oldWaveVectorID = Shader.PropertyToID("_OldSonarWaveVector");
        linesArrayID = Shader.PropertyToID("_LinesArray");
        lenghtID = Shader.PropertyToID("_Length");
        distanceID = Shader.PropertyToID("_Distance");
        oldDistanceID = Shader.PropertyToID("_oldDistance");
    }


    // Use this for initialization
    void Start()
    {
        StartCoroutine(CalculatePoints());
    }

    void Update()
    {
        float desiredDistance = microphoneListener.value;
        desiredDistance = Mathf.Max(desiredDistance, minDistance);
        desiredDistance = Mathf.Min(desiredDistance, maxDistance);

        distance = Mathf.SmoothStep(distance, desiredDistance, Time.deltaTime * 5);
        Shader.SetGlobalFloat(distanceID, distance);

        lastDistance = Mathf.SmoothStep(lastDistance, 0, Time.deltaTime);
        Shader.SetGlobalFloat(oldDistanceID, lastDistance);
    }

    private IEnumerator CalculatePoints()
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(.5f);
        float[] array = new float[ArraySize];
        Vector3 lastPlayerPosition = player.position;

        for (int i = 0; i < ArraySize; i++)
        {
            array[i] = Mathf.Pow(i / 16f, 2) + Mathf.Pow(i / 16f, 4);
        }

        for(int i = 0; i < ArraySize; i++)
        {
            Debug.Log(array[i]);
        }

        Shader.SetGlobalFloatArray(linesArrayID, array);
        Shader.SetGlobalInt(lenghtID, ArraySize);

        while (true)
        {
            if(firstStep)
            {
                Shader.SetGlobalVector(waveVectorID, player.position + player.right * footDistance);
                Shader.SetGlobalVector(oldWaveVectorID, lastPlayerPosition - player.right * footDistance);
            }
            else
            {
                Shader.SetGlobalVector(waveVectorID, player.position - player.right * footDistance);
                Shader.SetGlobalVector(oldWaveVectorID, lastPlayerPosition + player.right * footDistance);
            }

            lastPlayerPosition = player.position;
            lastDistance = distance;
            firstStep = !firstStep;

            yield return waitForSeconds;
        }
    }

    //void OnDrawGizmos()
    //{
    //    Debug.DrawRay(player.position, Vector3.right * distance, Color.blue);
    //}
}

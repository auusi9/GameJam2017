using UnityEngine;
using System.Collections;

public class SceneManager : MonoBehaviour
{
    public static SceneManager current;
    
    /// <summary>
    /// Mida del Array que passem al shader (ha de ser el mateix que al shader)
    /// </summary>
    public const int ArraySize = 64;

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

    public float[] distances;

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

    private Vector4[] positions;

    private WaveInfo[] wavesInfo;

    private int currentWave = 0;

    private void Awake()
    {
        if(current != null)
        {
            Debug.LogWarning("Trying to add more than one SceneManager");
            Destroy(this);
        }
        else
        {
            current = this;
        }

        wavesInfo = new WaveInfo[4];
        distances = new float[4];
        positions = new Vector4[4];

        waveVectorID = Shader.PropertyToID("_SonarWaveVector");
        oldWaveVectorID = Shader.PropertyToID("_OldSonarWaveVector");
        linesArrayID = Shader.PropertyToID("_LinesArray");
        lenghtID = Shader.PropertyToID("_Length");
        distanceID = Shader.PropertyToID("_Distance");
        oldDistanceID = Shader.PropertyToID("_oldDistance");

        CalculatePoints();
    }

    private void Update()
    {
        float desiredDistance = microphoneListener._AmplitudeBuffer;
        desiredDistance = Mathf.Max(desiredDistance, minDistance);
        desiredDistance = Mathf.Min(desiredDistance, maxDistance);

        for(int i = 0; i < wavesInfo.Length; i++)
        {
            // Mirem si la final distance de la wave actual es menor que la desiredDistance i si ho es la incrementem
            if (i == currentWave)
            {
                if(!wavesInfo[i].forcedFinalDistance && !wavesInfo[i].distanceFixed && wavesInfo[i].finalDistance < desiredDistance)
                {
                    wavesInfo[i].finalDistance = desiredDistance;
                }
                else
                {
                    wavesInfo[i].distanceFixed = true;
                }
            }

            if (wavesInfo[i].distance < wavesInfo[i].finalDistance)
            {
                wavesInfo[i].distance = Mathf.SmoothStep(wavesInfo[i].distance, wavesInfo[i].finalDistance, Time.deltaTime * 10);
            }
            else if(wavesInfo[i].timeInDistance < .2f)
            {
                wavesInfo[i].timeInDistance += Time.deltaTime;
            }
            else
            {
                wavesInfo[i].finalDistance = 0; // Fem reset per a que no entri al primer if
                wavesInfo[i].distance = Mathf.SmoothStep(wavesInfo[i].distance, 0, Time.deltaTime);
            }

            distances[i] = wavesInfo[i].distance;
            positions[i] = wavesInfo[i].position;
        }

        Shader.SetGlobalInt("_CurrentWave", currentWave);
        Shader.SetGlobalFloatArray("_Distances", distances);
        Shader.SetGlobalVectorArray("_StartPositions", positions);
    }

    /// <summary>
    /// Afegim una nova wave. Descartem la mes vella i actualitzem el punter a l'actual
    /// </summary>
    public void AddWave(Vector3 position, float finalDistance = 0)
    {
        currentWave = (++currentWave) % wavesInfo.Length;
        wavesInfo[currentWave].Reset(position, finalDistance);
    }

    private void CalculatePoints()
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(1f);
        float[] array = new float[ArraySize];
        Vector3 lastPlayerPosition = player.position;

        for (int i = 0; i < ArraySize; i++)
        {
            array[i] = Mathf.Pow(i / 16f, 2) + Mathf.Pow(i / 16f, 4);
        }

        for(int i = 0; i < positions.Length; i++)
        {
            positions[0] = player.position;
        }

        Shader.SetGlobalFloatArray(linesArrayID, array);
        Shader.SetGlobalInt(lenghtID, ArraySize);
    }

    //void OnDrawGizmos()
    //{
    //    Debug.DrawRay(player.position, Vector3.right * distance, Color.blue);
    //}

    public struct WaveInfo
    {
        public bool distanceFixed;

        public Vector4 position;

        public float distance;

        public float finalDistance;

        public bool forcedFinalDistance;

        public float timeInDistance;

        public void Reset(Vector4 pos, float fDistance = 0f)
        {
            position = pos;

            distanceFixed = false;

            distance = 0;

            finalDistance = fDistance;

            forcedFinalDistance = !Mathf.Approximately(0, fDistance);

            timeInDistance = 0;
        }
    }
}

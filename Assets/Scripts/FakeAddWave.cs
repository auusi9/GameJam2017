using UnityEngine;

public class FakeAddWave : MonoBehaviour
{
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            SceneManager.current.AddWave(SceneManager.current.player.position);
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            SceneManager.current.AddWave(SceneManager.current.player.position, 2f);
        }
    }
}

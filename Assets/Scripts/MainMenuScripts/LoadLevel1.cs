using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevel1 : MonoBehaviour
{
    /// <summary>
    /// Function called by play game button within main menu UI to load the first game level
    /// </summary>
    public void BeginGame()
    {
        SceneManager.LoadScene("Level1");
    }
}

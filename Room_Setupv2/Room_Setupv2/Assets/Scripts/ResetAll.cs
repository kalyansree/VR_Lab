using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ResetAll : MonoBehaviour {
    /*
     * This class is used solely for the Reset All Button and is used to reset the scene.
     */
    public void ResetAllButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

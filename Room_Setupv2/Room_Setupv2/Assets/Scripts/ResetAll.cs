using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ResetAll : MonoBehaviour {

    public void ResetAllButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

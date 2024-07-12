using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GuiManager : MonoBehaviour
{

    public void Restart_The_Simulation()
    {
        // Loads the current scene which will restart the simulation.
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

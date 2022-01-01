using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void StartAlphaStage()
    {
        SceneManager.LoadScene("AlphaStage");
    }

    public void StartDialogueStage()
    {
        SceneManager.LoadScene("DialogueTesting");
    }
}

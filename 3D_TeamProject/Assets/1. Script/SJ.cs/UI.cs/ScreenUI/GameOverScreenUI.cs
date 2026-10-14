using UnityEngine;

public class GameOverScreenUI : MonoBehaviour
{
    public GameObject GameOverScreenPanel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameOverScreenPanel.SetActive(false);
    }

    public void ActiveGameOverScreen()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        GameOverScreenPanel.SetActive(true);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuBehaviour : MonoBehaviour
{
    [SerializeField] private CanvasGroup Menu;
    [SerializeField] private GameObject GameManager;
    private MainGame mainGame;
    // Start is called before the first frame update
    void Start()
    {

        mainGame = GameManager.GetComponent<MainGame>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (mainGame.getEntering() == false)
            {
                setMenuState();
            }
        }
    }
    public void setMenuState()
    {
        Menu.gameObject.SetActive(!Menu.gameObject.activeSelf);
        if (Menu.gameObject.activeSelf)
        {
            mainGame.GamePause();
            gameObject.SetActive(false);
            Menu.alpha = 1.0f;
            Menu.interactable = true;
            Menu.blocksRaycasts = true;
        }
        else
        {

            mainGame.GameResume();
            Menu.alpha = 0f;
            Menu.interactable = false;
            Menu.blocksRaycasts = false;
        }
    }
}

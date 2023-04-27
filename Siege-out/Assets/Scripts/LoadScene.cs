using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    private float Animatetime = 1f;
    private float timer = 0f;
    private int mode = 0;
    [SerializeField] private int animationType = 0;
    [SerializeField] private GameObject UI;
    private Animator UiAnimation;
    private void Start()
    {
        UiAnimation = UI.GetComponent<Animator>();
    }

    public void setMode(int Mode)
    {
        mode = Mode;
    }
    public void setType(int type)
    {
        animationType = type;
    }
    public void runScene()
    {
        StartCoroutine(SceneLoader());
    }
    public void PlayEntry()
    {
        UiAnimation.Play("Entry");
    }
    private IEnumerator SceneLoader()
    {
      
        if (mode >= 1)
        {

                switch (animationType) 
                {
                    case 1:
                        UiAnimation.Play("Entry");
                        break;
                    case 2:
                    UiAnimation.Play("Leave");
                        break;
                }
            yield return new WaitForSeconds(Animatetime);

                switch (mode)
                {
                    case 1:
                        SceneManager.LoadScene("Game");
                        break;
                    case 2:
                        SceneManager.LoadScene("UIScene");
                        break;
                }
               
            
        }
    }
}

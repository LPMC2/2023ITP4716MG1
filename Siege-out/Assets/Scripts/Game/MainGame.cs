using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class MainGame : MonoBehaviour
{
    [Header("Game Function")]
#if UNITY_EDITOR
[ReadOnly] 
#endif
    [SerializeField] private int GameProcessID = 0;
    [SerializeField] private int RequiredItemCount = 7;
    [SerializeField] private GameObject SiegeMode1;
    [SerializeField] private GameObject SiegeMode2;
    [SerializeField] private GameObject Boss;

    private float GameTime;
    private int CurrentItemCount = 0;
    private PlayerUI playerUI;
    private string WordString = "";
    [Header("Player & Enemy")]
    [SerializeField] private GameObject PlayerCamera;
    [SerializeField] private GameObject Player;
    [SerializeField] private GameObject Target1;
    [SerializeField] private GameObject Target2;
    [Header("Game UI")]
   
    [SerializeField] private GameObject SiegeMenuUI;
    [SerializeField] private GameObject WorkBrenchName;
    public CanvasGroup uiCanvasGroup;
    private FirstPersonController FpsController;
    private bool isChose = false;
    private int SiegeMode = 0;
   
    // Start is called before the first frame update
    public int GetGameID()
    {
        return GameProcessID;
    }
    public void setCursor(bool BooleanDetect)
    {
        if(BooleanDetect == false)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
        }
        if(BooleanDetect == true)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
    void Start()
    {
<<<<<<< Updated upstream
        setCursor(false);
=======
        Vector3 spawnPosition = new Vector3(-32.6105042f, 5.78000021f, -33.2821121f);
        Instantiate(Target2, spawnPosition, Quaternion.identity);
        EnterUI = EnterUIObject.GetComponent<CanvasGroup>();
        EnterUIObject.SetActive(true);
        isEntering = true;
        ObjectiveUI.SetActive(false);
        endtimer = EndTime;
        Time.timeScale = 0.0f;
        EnterUI.alpha = 1.0f;
        EnterUI.interactable = true;
        EnterUI.blocksRaycasts = true;
>>>>>>> Stashed changes
        FpsController = Player.GetComponent<FirstPersonController>();
        playerUI = GetComponent<PlayerUI>();
        WorkBrenchName.SetActive(false);
        SiegeMode1.SetActive(false);
        SiegeMode2.SetActive(false);
        CurrentItemCount = 0;
        GameProcessID = 0;
        playerUI.UpdateText("Material(s) Required to Collect: " + (RequiredItemCount - CurrentItemCount));
    }
    public void setCurrentItemCount(int count)
    {
       
        CurrentItemCount += count;
        if (playerUI)
        {
            playerUI.UpdateText("Material(s) Required to Collect: " + (RequiredItemCount - CurrentItemCount));
        }
        
    }
    // Update is called once per frame
    void Update()
    {

        switch(GameProcessID)
        {
            case 0:
                isChose = false;
                SiegeMenuUI.SetActive(false);
                WordString = "";
                GameProcessID++;
                break;
            case 1:
                if(CurrentItemCount >= RequiredItemCount)
                {
                    playerUI.UpdateText("Go to the Construction Work Brench");
                    WorkBrenchName.SetActive(true);
                    CurrentItemCount = 0;
                    RequiredItemCount = 1;
                    WordString = "||||||||||";
                    Boss.SetActive(true);
                    GameProcessID++;
                    
                }
                break;
            case 2:
                if(CurrentItemCount >= RequiredItemCount)
                {

                    if (isChose == true)
                    {
                        GameTime += Time.deltaTime;
                        if (GameTime >= 6f && WordString.Length > 0)
                        {
                            WordString = WordString.Remove(WordString.Length - 1);
                            GameTime = 0;
                        }
                        if (WordString.Length == 0)
                        {
                            switch(SiegeMode)
                            {
                                case 1:
                                SiegeMode1.SetActive(true);
                                    playerUI.UpdateText("Break the Wall and Escape!");
                                    break;
                                case 2:
                                SiegeMode2.SetActive(true);
                                    playerUI.UpdateText("Attack!");
                                    break;

                            }
                            
                            GameProcessID++;
                        } else
                        playerUI.UpdateText("Creating Siege Machine\n " + WordString);

                    }
                }
                break;

            case 3:
                switch (SiegeMode)
                {
                    case 1:
                        if(!Target1)
                        playerUI.UpdateText("Game End!");
                        break;
                    case 2:
<<<<<<< Updated upstream
                        if(!Target2)
                        playerUI.UpdateText("Game End!");
=======
                        if (!Target2)
                        {
                            winGame();
                            playerUI.UpdateText("Game End!");
                        }
>>>>>>> Stashed changes
                        break;

                }
                break;
        }
        
    }
    
    public void GetSiegeMenu()
    {
        if (isChose == false && GameProcessID ==2)
        {
            
            Time.timeScale = 0.0f;
                uiCanvasGroup.alpha = 1.0f;
                uiCanvasGroup.interactable = true;
                uiCanvasGroup.blocksRaycasts = true;


            setCursor(true);
            CurrentItemCount++;
            SiegeMenuUI.SetActive(true);
         if(Cursor.visible)
            {
                Debug.Log("is visible");
            }
          
        }
    }
    public void GamePause()
    {
        setCursor(true);
        FollowPlayer followPlayer = PlayerCamera.GetComponent<FollowPlayer>();
        followPlayer.enabled = false;
        Time.timeScale = 0f;
       
    }
    public void GameResume()
    {
        setCursor(false);
        FollowPlayer followPlayer = PlayerCamera.GetComponent<FollowPlayer>();
        followPlayer.enabled = true;
        Time.timeScale = 1.0f;
        
    }
    public void setMode1()
    {
      
        SiegeMode = 1;
        isChose = true;
        SiegeMenuUI.SetActive(false);
        Time.timeScale = 1.0f;
        uiCanvasGroup.alpha = 0.0f;
        uiCanvasGroup.interactable = false;
        uiCanvasGroup.blocksRaycasts = false;
        setCursor(false);
    }
    public void setMode2()
    {
       
       
        SiegeMode = 2;
        isChose = true;
        SiegeMenuUI.SetActive(false);
        Time.timeScale = 1.0f;
        uiCanvasGroup.alpha = 0.0f;
        uiCanvasGroup.interactable = false;
        uiCanvasGroup.blocksRaycasts = false;
        setCursor(false);
    }

}

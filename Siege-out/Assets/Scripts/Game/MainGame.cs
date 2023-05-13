using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    [SerializeField] private GameObject EnterUIObject;
    [SerializeField] private GameObject ObjectiveUI;
    [SerializeField] private float EndTime = 5f;
    [SerializeField] private GameObject WinUI;
    [SerializeField] private GameObject LoseUI;
    [SerializeField] private GameObject SiegeMenuUI;
    [SerializeField] private GameObject WorkBrenchName;
    [SerializeField] private AudioClip WinSound;
    [SerializeField] private AudioClip LoseSound;
    private AudioSource audioSource;
    private CanvasGroup EnterUI;
    public CanvasGroup uiCanvasGroup;
    private FirstPersonController FpsController;
    private bool isChose = false;
    private int SiegeMode = 0;
    private bool isEnd = false;
    private float endtimer;
    private HealthBehaviour spawnerhealth;
    private float initialHealth;
    private bool isAttackable = false;
    private bool isEntering = false;
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
    public bool getEntering()
    {
        return isEntering;
    }
    private void DisableGun()
    {
        GunController[] gunControllers = PlayerCamera.GetComponentsInChildren<GunController>(false);

        // Loop through each child GunController and disable its GameObject
        foreach (GunController gunController in gunControllers)
        {
            gunController.gameObject.SetActive(false);
        }

    }
    private void EnableGun()
    {
        // Get all the child GunController components
        GunController[] gunControllers = PlayerCamera.GetComponentsInChildren<GunController>(true);

        // Loop through each child GunController and enable its GameObject
        foreach (GunController gunController in gunControllers)
        {
            gunController.gameObject.SetActive(true);
        }

    }
    void Start()
    {
 
        EnterUI = EnterUIObject.GetComponent<CanvasGroup>();
        EnterUIObject.SetActive(true);
        isEntering = true;
        ObjectiveUI.SetActive(false);
        endtimer = EndTime;
        Time.timeScale = 0.0f;
        EnterUI.alpha = 1.0f;
        EnterUI.interactable = true;
        EnterUI.blocksRaycasts = true;
        FpsController = Player.GetComponent<FirstPersonController>();
        spawnerhealth = Target2.GetComponent<HealthBehaviour>();
        playerUI = GetComponent<PlayerUI>();
        WorkBrenchName.SetActive(false);
        SiegeMode1.SetActive(false);
        SiegeMode2.SetActive(false);
        CurrentItemCount = 0;
        GameProcessID = 0;
        playerUI.UpdateText("Material(s) Required to Collect: " + (RequiredItemCount - CurrentItemCount));
        GamePause();
    }
    public void setCurrentItemCount(int count)
    {
       
        CurrentItemCount += count;
        if (playerUI)
        {
            playerUI.UpdateText("Material(s) Required to Collect: " + (RequiredItemCount - CurrentItemCount));
        }
        
    }
    public void disableEnterUI()
    {
        EnterUI.alpha = 0f;
        EnterUI.interactable = false;
        EnterUI.blocksRaycasts = false;
        LoadScene loadScene = GetComponent<LoadScene>();
        loadScene.PlayEntry(); ;
        isEntering = false;
    }
    // Update is called once per frame
    void Update()
    {
        if (spawnerhealth.GetInitialHealth() >= 1 && initialHealth <= 0)
        {
            initialHealth = spawnerhealth.GetInitialHealth();
        }
            switch (GameProcessID)
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
                                    playerUI.UpdateText("Attack the Spawner!");
                                    isAttackable = true;
                                   
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
                        if (!Target1)
                        {
                            playerUI.UpdateText("Game End!");
                            winGame();
                        }
                        break;
                    case 2:
                        if (!Target2)
                        {
                            winGame();
                            playerUI.UpdateText("Game End!");
                        }
                        break;

                }
                break;
        }
        if(isEnd == true)
        {
            ending();
        }
        if(isAttackable == false)
        {

                spawnerhealth.HealthSetter(initialHealth);
        }
    }
    
    public void GetSiegeMenu()
    {
        if (isChose == false && GameProcessID ==2)
        {
            ObjectiveUI.SetActive(false);
            Time.timeScale = 0.0f;
                uiCanvasGroup.alpha = 1.0f;
                uiCanvasGroup.interactable = true;
                uiCanvasGroup.blocksRaycasts = true;


            setCursor(true);
            CurrentItemCount++;
            SiegeMenuUI.SetActive(true);
         if(Cursor.visible)
            {
               
            }
          
        }
    }
    public void GamePause()
    {
        ObjectiveUI.SetActive(false);
        setCursor(true);
        FollowPlayer followPlayer = PlayerCamera.GetComponent<FollowPlayer>();
        followPlayer.enabled = false;
        Time.timeScale = 0f;
        DisableGun();
    }
    public void GameResume()
    {
        ObjectiveUI.SetActive(true);
        setCursor(false);
        FollowPlayer followPlayer = PlayerCamera.GetComponent<FollowPlayer>();
        followPlayer.enabled = true;
        Time.timeScale = 1.0f;
        EnableGun();
    }
    public void loseGame()
    {
        
        audioSource = GetComponent<AudioSource>();
        if(audioSource != null)
        {
            audioSource.clip = LoseSound;
            audioSource.Play();
        }
        ObjectiveUI.SetActive(false);
        LoseUI.SetActive(true);
        WinUI.SetActive(false);
        isEnd = true;
        Target1.SetActive(false);
        Target2.SetActive(false);
        SiegeMode1.SetActive(false);
        SiegeMode2.SetActive(false);
    }
    public void winGame()
    {
       
        audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.clip = WinSound;
            audioSource.Play();
        }
        ObjectiveUI.SetActive(false);
        WinUI.SetActive(true);
        isEnd = true;
    }
    public void setMode1()
    {
        ObjectiveUI.SetActive(true);
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

        ObjectiveUI.SetActive(true);
        SiegeMode = 2;
        isChose = true;
        SiegeMenuUI.SetActive(false);
        Time.timeScale = 1.0f;
        uiCanvasGroup.alpha = 0.0f;
        uiCanvasGroup.interactable = false;
        uiCanvasGroup.blocksRaycasts = false;
        setCursor(false);
    }
    
    private void ending()
    {
        
        endtimer -= Time.deltaTime;
        if(WinUI.activeSelf)
        {
            Animator animator = WinUI.GetComponent<Animator>();
            animator.Play("UI_Animate");
            Transform textfield = WinUI.transform.GetChild(0).gameObject.GetComponent<Transform>();
            Text txt = textfield.gameObject.GetComponent<Text>();
            txt.text = "Return in " + Mathf.Ceil(endtimer).ToString("0") + "s...";
        }
        if (LoseUI.activeSelf)
        {
            
            Animator animator = LoseUI.GetComponent<Animator>();
            animator.Play("Animate_Lose");
            Transform textfield = LoseUI.transform.GetChild(0).gameObject.GetComponent<Transform>();
            Text txt = textfield.gameObject.GetComponent<Text>();
            txt.text = "Return in " + Mathf.Ceil(endtimer).ToString("0") + "s...";
        }
        if (endtimer <= 0)
        {
            LoadScene loadScene = GetComponent<LoadScene>();
            loadScene.setMode(2);
            loadScene.setType(2);
            loadScene.runScene();
           
            setCursor(true);
        }
    }
}

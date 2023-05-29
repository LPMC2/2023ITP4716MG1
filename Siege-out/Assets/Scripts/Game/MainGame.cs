using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
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
    [SerializeField] private GameObject TimeUI;
    [Header("Settings")]
    [SerializeField] private bool isMain = true;
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
    private int score = 0;
    private GameObject weapon;
    private float TimeLeft = 10f;
    private int reqScore = 100;
    private PlayerUI timeUIScore;
    // Start is called before the first frame update
    public int GetGameID()
    {
        return GameProcessID;
    }
    public bool getIsEnd()
    {
        return isEnd;
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
    public void setReqScore(int rScore)
    {
        reqScore = rScore;
        GameObject rScoreObj = ObjectiveUI.transform.GetChild(3).gameObject;
        PlayerUI rScoreUI = rScoreObj.GetComponent<PlayerUI>();
        rScoreUI.UpdateText("Required Score: " + reqScore);

    }
    public bool getEntering()
    {
        return isEntering;
    }
    public GameObject getPlayer()
    {
        return Player;
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
        if(TimeUI != null)
        {
            timeUIScore = TimeUI.GetComponent<PlayerUI>();
        }
        if (EnterUIObject != null)
        {
            EnterUI = EnterUIObject.GetComponent<CanvasGroup>();
            EnterUIObject.SetActive(true);
        }
        isEntering = true;
        if (ObjectiveUI != null)
        {
            ObjectiveUI.SetActive(false);
        }
        endtimer = EndTime;
       
        Time.timeScale = 0.0f;
        if (EnterUI != null)
        {
            EnterUI.alpha = 1.0f;
            EnterUI.interactable = true;
            EnterUI.blocksRaycasts = true;
        }
        if (Player != null)
        {
            FpsController = Player.GetComponent<FirstPersonController>();
        }
        if (PlayerCamera != null)
        {
            PlayerCamera = Player.transform.GetChild(0).gameObject;
        }
        if (Target2 != null)
        {
            spawnerhealth = Target2.GetComponent<HealthBehaviour>();
        }
        playerUI = GetComponent<PlayerUI>();
        if (WorkBrenchName != null)
        {
            WorkBrenchName.SetActive(false);
        }
        if (SiegeMode1 != null)
        {
            SiegeMode1.SetActive(false);
        }
        if (SiegeMode2 != null)
        {
            SiegeMode2.SetActive(false);
        }
        CurrentItemCount = 0;
        GameProcessID = 0;
        if (isMain)
        {
            playerUI.UpdateText("Material(s) Required to Collect: " + (RequiredItemCount - CurrentItemCount));
            
        } else
        {
            playerUI.UpdateText("Score: " + score);
        }
        GamePause();
    }
    public void setCurrentItemCount(int count)
    {
       
        
        if (isMain)
        {
            CurrentItemCount += count;
            if (playerUI)
            {
                playerUI.UpdateText("Material(s) Required to Collect: " + (RequiredItemCount - CurrentItemCount));
            }
        }
        else
        {
            playerUI.UpdateText("Score: " + score);
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
    public void setTime(float time)
    {
        TimeLeft = time;
    }
    void Training()
    {
        weapon = PlayerCamera.transform.GetChild(1).gameObject;
        if(weapon != null)
        {
            GunController gunController = weapon.GetComponent<GunController>();
            if(gunController != null)
            {
                if (gunController.GetRange() != 999)
                {
                    gunController.setRange(999);
                }
                if (gunController.GetBulletCount() == 1 && gunController.GetSpreadMul() != 0.01f)
                {
                    gunController.setSpreadMul(0.01f);
                }
                else
                {
                    if (gunController.GetRemainAmmo() != 999)
                    {
                        gunController.SetRemainAmmo(1000);
                    }
                }
                if (gunController.GetTotalAmmo() != 999)
                {
                    gunController.SetTotalAmmo(999);

                    gunController.UpdateInv();
                }
            }
        }
        if(TimeUI != null && TimeLeft >= 0 && isEnd == false)
        {
            TimeLeft -= Time.deltaTime;
            
            timeUIScore.UpdateText("Time Left: " + Mathf.Round(TimeLeft) + "s");
            if (TimeLeft <= 10f)
            {
                TextMeshProUGUI timeUiText = TimeUI.GetComponent<TextMeshProUGUI>();
                timeUiText.color = Color.red;
            } else
            {
                TextMeshProUGUI timeUiText = TimeUI.GetComponent<TextMeshProUGUI>();
                timeUiText.color = Color.white;
            }
            if(TimeLeft <= 0)
            {
                loseGame();
            }
            if(score >= reqScore && TimeLeft > 0)
            {
                winGame();
            }
        }
        if(isEnd == true)
        {
            score = 0;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (isMain == false)
        {
            Training();
        }
        if (isMain)
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
                    if (CurrentItemCount >= RequiredItemCount)
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
                    if (CurrentItemCount >= RequiredItemCount)
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
                                switch (SiegeMode)
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
                            }
                            else
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

            if (isAttackable == false)
            {

                spawnerhealth.HealthSetter(initialHealth);
            }
        }
        if (isEnd == true)
        {
            ending();
        }
    }
    public void addScore(int Score)
    {
        score += Score;
        setCurrentItemCount(score);
        GameObject scoreObj = ObjectiveUI.transform.GetChild(2).gameObject;
        PlayerUI scoreUi = scoreObj.GetComponent<PlayerUI>();
        scoreUi.UpdateText("+" + Score);
        Animator scoreAni = ObjectiveUI.GetComponent<Animator>();
        scoreAni.Play("fadein_up");
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
        if (ObjectiveUI == null)
        {
            ObjectiveUI = GameObject.Find("ObjectiveField");
        }
            ObjectiveUI.SetActive(false);
        
        setCursor(true);
        if(PlayerCamera == null)
        {
            PlayerCamera = GameObject.Find("FirstPersonCharacter");
        }
        FollowPlayer followPlayer = PlayerCamera.GetComponent<FollowPlayer>();
        followPlayer.enabled = false;
        Time.timeScale = 0f;
        DisableGun();
    }
    public void GameResume()
    {
        if (ObjectiveUI != null)
        {
            ObjectiveUI.SetActive(true);
        }
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
        if (Target1 != null && Target2 != null)
        {
            Target1.SetActive(false);
            Target2.SetActive(false);
        }
        if (SiegeMode1 != null && SiegeMode2 != null)
        {
            SiegeMode1.SetActive(false);
            SiegeMode2.SetActive(false);
        }
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

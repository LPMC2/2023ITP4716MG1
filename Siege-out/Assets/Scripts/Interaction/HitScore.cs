using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitScore : MonoBehaviour
{
    [SerializeField] private GameObject MainGameObj;
    [SerializeField] private int Score;
    [SerializeField] private float delayTime = 5f;
    private HitScore hitScore;
    private Animator dummy;
    #if UNITY_EDITOR
    [ReadOnly]
#endif
    [SerializeField]
    private GameObject[] hitScoreObj;
#if UNITY_EDITOR
    [ReadOnly, SerializeField]
#endif
    private GameObject parentObj;
    int index = 0;
    // Start is called before the first frame update
    void Start()
    {
        if(MainGameObj == null)
        {
             MainGameObj = GameObject.Find("MainGame");
        }
        index = 0;
        parentObj = gameObject.transform.parent.gameObject;
      
        dummy = gameObject.transform.parent.gameObject.GetComponent<Animator>();
        int childCount = parentObj.transform.childCount;
        hitScoreObj = new GameObject[childCount];
        
        for(int i=0; i<childCount; i++)
        {
            Transform childTransform = parentObj.transform.GetChild(i);
            GameObject childObject = childTransform.gameObject;
            HitScore hitScore = childObject.GetComponent<HitScore>();
            if(hitScore != null)
            {
                hitScoreObj[index] = childObject;
                index++;
            }
        }
    }
    public void Hit()
    {
        MainGame mainGame = MainGameObj.GetComponent<MainGame>();
        float distance = Vector3.Distance(gameObject.transform.position, mainGame.getPlayer().transform.position);
        int scoreToAdd = Mathf.RoundToInt(Score * distance/5);
        mainGame.addScore(scoreToAdd);
        dummy.Play("Hit");
        Invoke("Recover", delayTime);
        setComState(0);
    }
    private void setComState(int state)
    {
        for(int c=0; c<index; c++)
        {
            hitScore = hitScoreObj[c].GetComponent<HitScore>();
            switch (state)
            {
                case 0:
                    hitScore.enabled = false;
                    break;
                case 1:
                    hitScore.enabled = true;
                    break;
            }
        }
    }
    void Recover()
    {
        
        if (dummy != null)
        {
            dummy.Play("Rec");
            setComState(1);
        }
    }
}

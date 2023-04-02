using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoilController : MonoBehaviour, IRecoilable
{
    public float recoilDuration;
    [SerializeField] private float maxRecoilAmount = 0.1f;
    private AnimationCurve recoilCurve;
    private float currentRecoilAmount = 0.0f;

    void Start()
    {
        recoilCurve = AnimationCurve.Linear(0.0f, 0.0f, recoilDuration, 1.0f);
    }
    public IEnumerator RecoilCoroutine()
    {
        float elapsedTime = 0.0f;
        while (elapsedTime < recoilDuration)
        {
            float recoilAmount = maxRecoilAmount * recoilCurve.Evaluate(elapsedTime / recoilDuration);
            currentRecoilAmount += recoilAmount;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        while (currentRecoilAmount > 0.0f)
        {
            currentRecoilAmount -= Time.deltaTime;
            yield return null;
        }
        currentRecoilAmount = 0.0f;
    }
}

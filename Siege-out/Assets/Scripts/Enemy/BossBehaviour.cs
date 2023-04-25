using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class BossBehaviour : MonoBehaviour
{
#if UNITY_EDITOR
[ReadOnly]
#endif
    private bool isDistance = false;
    [SerializeField] private float switchDistance = 2f;
    [SerializeField] private float switchDamage = 2f;
    [SerializeField] private float switchAttackCD = 2f;
    [SerializeField] private float switchPreAttackCD = 2f;
    private float[] storeData = new float[3];
    private Transform target;
    private EnemyController enemyController;
    // Start is called before the first frame update
    void Start()
    {
        enemyController = GetComponent<EnemyController>();
        target = enemyController.getTarget();
    }

    // Update is called once per frame
    void Update()
    {
        target = enemyController.getTarget();
        float distance = Vector3.Distance(target.position, transform.position);
        if (distance <= switchDistance && isDistance == false)
        {
            storeData[0] = enemyController.getDamage();
            storeData[1] = enemyController.getAttackCD();
            storeData[2] = enemyController.getPreAttackCD();
            enemyController.setAttackData(switchDamage, switchAttackCD, switchPreAttackCD);
            enemyController.changeAttackType(1);
            isDistance = true;
        }
        if (distance > switchDistance && isDistance == true)
        {
            enemyController.setAttackData(storeData[0], storeData[1], storeData[2]);
            enemyController.changeAttackType(2);
            isDistance = false;
        }
    }
}

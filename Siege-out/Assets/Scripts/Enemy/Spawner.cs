using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class MonsterData
{
    public GameObject Monster;
    public float SpawnChance;

    [HideInInspector]
    public string SpawnChanceString;

    public void UpdateSpawnChanceString()
    {
        SpawnChanceString = "%";
    }
}


[System.Serializable]
public class Spawner : MonoBehaviour
{
    private List<int> IdList = new List<int>();

    [SerializeField] private List<MonsterData> monsters = new List<MonsterData>();
    [SerializeField] private float SpawnTime;
    [SerializeField] private int SpawnCountLimit = 10;
    [SerializeField] private int SpawnAmout = 1;
#if UNITY_EDITOR
[ReadOnly]
#endif
    [SerializeField] private int SpawnCounter = 0;
    private int CurrentCounter;
    public List<MonsterData> Monsters
    {
        get { return monsters; }
        set { monsters = value; }
    }
    // Start is called before the first frame update
    void Start()
    {
        SpawnCounter = 0;
        InvokeRepeating("Spawn", 0f, SpawnTime);
    }

    public void rmSpawnCounter()
    {
        SpawnCounter--;
        if(SpawnCounter < 0) { SpawnCounter = 0; }
    }
    private List<GameObject> spawnedMonsters = new List<GameObject>(); // List to keep track of spawned monsters

    void Spawn()
    {

        if (SpawnCounter < SpawnCountLimit)
        {
           
            for (int i = 0; i < SpawnAmout; i++)
            {
                if (SpawnCounter < SpawnCountLimit)
                {


                    foreach (MonsterData monsterData in monsters)
                    {
                        if (Random.Range(0f, 100f) <= monsterData.SpawnChance)
                        {
                            Vector3 spawnPosition = transform.position + Random.insideUnitSphere * 3f;
                            GameObject newMonster = Instantiate(monsterData.Monster, spawnPosition, Quaternion.identity) as GameObject;
                            Physics.IgnoreCollision(newMonster.GetComponent<Collider>(), GetComponent<Collider>());
                            MonsterId monsterId = newMonster.GetComponent<MonsterId>();
                            if(newMonster != null)
                            {
                                SpawnCounter++;
                            }
                            if (monsterId != null)
                            {
                                IdList.Add(monsterId.GetID());
                                
                            }

                            
                            spawnedMonsters.Add(newMonster); // Add the spawned monster to the list
                            break;
                        }
                    }
                }
            }
        }

        // Check for destroyed monsters and remove them from the list
        spawnedMonsters.RemoveAll(m => m == null);
    }
}

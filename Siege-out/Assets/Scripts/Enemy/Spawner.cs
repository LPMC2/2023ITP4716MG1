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
    private int SpawnCounter = 0;
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


    private List<GameObject> spawnedMonsters = new List<GameObject>(); // List to keep track of spawned monsters

    void Spawn()
    {
        CurrentCounter = 0;
        for (int amount = 0; amount < IdList.Count; amount++)
        {
            foreach (int id in IdList)
            {
                if (IdList[amount] == IdList[id] && amount != id)
                {
                    CurrentCounter++;
                }
            }
        }
        if (CurrentCounter < SpawnCountLimit && SpawnCounter < SpawnCountLimit)
        {
            CurrentCounter = SpawnCounter;
            for (int i = 0; i < SpawnAmout; i++)
            {
                if (CurrentCounter < SpawnCountLimit && SpawnCounter < SpawnCountLimit)
                {


                    foreach (MonsterData monsterData in monsters)
                    {
                        if (Random.Range(0f, 100f) <= monsterData.SpawnChance)
                        {
                            Vector3 spawnPosition = transform.position + Random.insideUnitSphere * 3f;
                            GameObject newMonster = Instantiate(monsterData.Monster, spawnPosition, Quaternion.identity) as GameObject;
                            Physics.IgnoreCollision(newMonster.GetComponent<Collider>(), GetComponent<Collider>());
                            MonsterId monsterId = newMonster.GetComponent<MonsterId>();
                            if (monsterId != null)
                            {
                                IdList.Add(monsterId.GetID());
                            }
                            AIController aiController = newMonster.GetComponent<AIController>();
                            if (aiController != null)
                            {
                               
                            }
                            SpawnCounter++;
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

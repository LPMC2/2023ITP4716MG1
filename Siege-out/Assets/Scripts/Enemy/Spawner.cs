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
    [SerializeField] private float minSpawnTime = 15f;
    [SerializeField] private float maxSpawnTime = 20f;
    [SerializeField] private float minSpawnRadius = 3f;
    [SerializeField] private float maxSpawnRadius = 3f;
    [SerializeField] private int SpawnCountLimit = 10;
    [SerializeField] private int SpawnAmout = 1;
    [SerializeField] private GameObject TargetObject;
    [SerializeField] private float StayTime = 0f;
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
        if (TargetObject == null)
        {
            InvokeRepeating("Spawn", 0f, Random.Range(minSpawnTime, maxSpawnTime));
        }
    }
    public void startSpawn()
    {
        InvokeRepeating("Spawn", 0f, Random.Range(minSpawnTime, maxSpawnTime));
    }
    public void setMinSpSpeed(float min)
    {
        minSpawnTime = min;
    }
    public void setMaxSpSpeed(float max)
    {
        maxSpawnTime = max;
    }
    public void setStayTime(float time)
    {
        StayTime = time;
    }
    public void setSpawnCount(int count)
    {
        SpawnAmout = count;
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
                            Vector2 randomPoint = Random.insideUnitCircle.normalized * Random.Range(minSpawnRadius, maxSpawnRadius);
                            Vector3 spawnPosition = transform.position + new Vector3(randomPoint.x, 0f, randomPoint.y);
                            
                            GameObject newMonster = Instantiate(monsterData.Monster, spawnPosition, Quaternion.identity) as GameObject;
                            if (newMonster.GetComponent<Collider>() != null)
                            {
                                Physics.IgnoreCollision(newMonster.GetComponent<Collider>(), GetComponent<Collider>());
                            }
                            if (TargetObject != null)
                            {
                                newMonster.transform.LookAt(new Vector3(TargetObject.transform.position.x, newMonster.transform.position.y, TargetObject.transform.position.z), Vector3.up);

                            }
                            if (StayTime > 0)
                            {
                                Destroy(newMonster,StayTime);
                            }
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
    private IEnumerator FadeOut(GameObject obj, float duration, float stayTime)
    {
        // Get the object's material and store its initial color
        Renderer renderer = obj.GetComponent<Renderer>();
        Color initialColor = renderer.material.color;

        // Wait for the specified stay time before starting to fade out the object
        yield return new WaitForSeconds(stayTime);

        // Gradually decrease the object's alpha value over time
        float startTime = Time.time;
        float endTime = startTime + duration;
        while (Time.time < endTime)
        {
            float t = (Time.time - startTime) / duration;
            Color newColor = new Color(initialColor.r, initialColor.g, initialColor.b, Mathf.Lerp(initialColor.a, 0f, t));
            renderer.material.color = newColor;
            yield return null;
        }

        // Destroy the object once it has completely faded out
        Destroy(obj);
    }
}

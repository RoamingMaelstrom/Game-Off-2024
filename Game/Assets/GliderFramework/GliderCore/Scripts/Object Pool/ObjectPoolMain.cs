using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SOEvents;

public class ObjectPoolMain : MonoBehaviour
{
    [Tooltip("Used to return valid GameObjects to their (Object)pool without accessing an ObjectPoolMain class instance directly.")]
    [SerializeField] List<GameObjectFloatSOEvent> returnToPoolFloatEvents = new();
    [Tooltip("Used to return valid GameObjects to their (Object)pool without accessing an ObjectPoolMain class instance directly.")]
    [SerializeField] List<GameObjectSOEvent> returnToPoolEvents = new();
    [SerializeField] List<PoolInfo> poolInfoObjects = new();
    [SerializeField] List<PoolInfoGroup> poolInfoGroupObjects = new();
    private List<Pool> objectPools = new();

    [Tooltip("Guides the rate at which the pool will fill up. Note: Called every FixedUpdate, but the value is divided by 50 and then cast as an int.")]
    [SerializeField] int maxPoolFillRate = 50;

    [Tooltip("Determines whether pools that are not full will have new Objects added to them.")]
    [SerializeField] bool allowNewGameObjectInstantiation = true;
    [Tooltip("Determines whether IDs are automatically generated upon the scene loading (starts from 0, overrides any editor assigned IDs)")]
    [SerializeField] bool autoAssignIDs = false;

    public int numberOfFixedUpdates {get; private set;}

    // Used when spawning an object from a pool with no free Objects. Assigns furthest from trackingObject if trackingObject exists, else FIFO.
    [SerializeField] GameObject trackingObject;



    private void Awake() 
    { 
        CreatePoolsFromPoolInfoObjects();
        CreatePoolsFromPoolInfoGroupObjects();
        if (autoAssignIDs) AssignPoolIDs();
        else CheckForIdCollisionsOnStart();
        PopulatePoolsOnAwake();

        foreach (var returnToPoolEvent in returnToPoolFloatEvents) returnToPoolEvent.AddListener(ReturnObject);
        foreach (var returnToPoolEvent in returnToPoolEvents) returnToPoolEvent.AddListener(ReturnObject);    
    }

    private void CreatePoolsFromPoolInfoGroupObjects()
    {
        foreach (var group in poolInfoGroupObjects)
        {
            for (int i = 0; i < group.objectPrefabList.Count; i++)
            {
                objectPools.Add(new Pool(group.startPoolID + i, string.Format("{0} {1}", group.basePoolName, i), group.objectPrefabList[i], group.maxPoolSize, group.dynamicOptions));
            }
        }
    }

    private void CreatePoolsFromPoolInfoObjects()
    {
        foreach (var poolInfo in poolInfoObjects)
        {
            objectPools.Add(new Pool(poolInfo.poolID, poolInfo.poolName, poolInfo.objectPrefab, poolInfo.maxPoolSize, poolInfo.dynamicOptions));
        }
    }

    // Starts process of filling up pools to instance Pool.suggestedPoolSize.
    private void Start() => StartCoroutine(PopulatePools());

    private void FixedUpdate()
    {
        numberOfFixedUpdates ++;
        foreach (var pool in objectPools) 
        {
            pool.HandleDynamicExpansion(this);
            pool.HandleDynamicShrinking(numberOfFixedUpdates);
            pool.UpdateEditorObjectsFreeNumberDisplays();
        }
    }

    public void DisableNewGameObjectInstantiation() => allowNewGameObjectInstantiation = false;
    public void EnableNewGameObjectInstantiation() => allowNewGameObjectInstantiation = true;

    // Part of class API. 
    // Returns the number of Objects in use for a pool with provied poolID. Returns -1 if cannot find pool.
    public int GetNumberOfObjectsInUse(int poolID)
    {
        foreach (var pool in objectPools)
        {
            if (pool.poolID == poolID) return pool.objectsInUse.Count;
        }
        return -1;
    }

    // Returns the number of Objects in use for a pool with provied poolName. Returns -1 if cannot find pool.
    public int GetNumberOfObjectsInUse(string poolName)
    {
        foreach (var pool in objectPools)
        {
            if (pool.poolName == poolName) return pool.objectsInUse.Count;
        }
        return -1;
    }

    // Part of class API. Gets an object from one of the pools under this class's management.
    // (ID is assigned by ObjectPoolMain.AssignPoolIDs).
    public GameObject GetObject(int poolID)
    {
        foreach (var pool in objectPools)
        {
            if (pool.poolID == poolID) return GetObjectFromPool(pool);
        }

        // Called if cannot find pool with provided ID.
        throw new System.Exception(string.Format("CUSTOM ERROR: Invalid pool ID provided (Could not find Pool with poolID = {0})", poolID));
    }

    // Gets an object from one of the pools under this class's management.
    // (Name assigned in Unity Inspector and stored in Pool class)
    public GameObject GetObject(string poolName)
    {
        foreach (var pool in objectPools)
        {
            if (pool.poolName == poolName) return GetObjectFromPool(pool);
        }
        // Called if cannot find pool with provided name.
        throw new System.Exception(string.Format("CUSTOM ERROR: Invalid pool name provided (Could not find Pool with poolName = {0})", poolName));
    }

    private GameObject GetObjectFromPool(Pool pool)
    {
        if (trackingObject == null) return pool.GetObject();
        if (!trackingObject.activeInHierarchy) return pool.GetObject();

        return pool.GetObject(trackingObject.transform.position);
    }

    // Returns objects to one of the pools under this class's management.
    public void ReturnObject(GameObject objectToReturn)
    {
        int poolID = objectToReturn.GetComponent<ObjectIdentifier>().poolID;
        foreach (var pool in objectPools)
        {
            if (pool.poolID == poolID) 
            {
                ReturnObject(objectToReturn, pool);
                return;
            }
        }
        // throw exception if cannot find poolID in objectPools that matches poolID in Object ObjectIdentifier instance.
        throw new System.Exception(string.Format("INVALID_POOL_ID_ERROR: Could not find Pool with poolID = {0}",
         poolID));
    }


    
    // Assigns each pool a unique ID, starting at 0. Based on the pools position in objectPools variable.
    // Overrides any IDs set in the editor
    private void AssignPoolIDs()
    {
        for (int i = 0; i < objectPools.Count; i++)
        {
            objectPools[i].poolID = i;
        }
    }

    private bool CheckForIdCollision(int poolID)
    {
        int collisions = 0;
        foreach (var pool in objectPools)
        {
            if (pool.poolID == poolID) collisions ++;
            if (collisions >= 2) return true;
        }
        return false;
    }

    private void CheckForIdCollisionsOnStart()
    {
        foreach (var pool in objectPools)
        {
            if (CheckForIdCollision(pool.poolID))
            {
                throw new System.Exception(string.Format("POOL_ID_COLLISION_ERROR: Two or more pools have the ID {0}. Object Pool IDs should be unique.", pool.poolID));
            }
        }
    }


    // Used to do the majority of ObjectPool Filling. 
    // Will constantly run until all pools are filled up.
    IEnumerator PopulatePools()
    {
        // Initial setup of variables used in method.
        List<float> poolFillPercentages = new List<float>();
        foreach (var pool in objectPools)
        {
            poolFillPercentages.Add(0);
        }

        float minFill;
        int leastFullPoolIndex = 0;
        float tempFill;

        // Calls every Max(physics update (default 0.02s) OR 1 / maxPoolFillRate)
        while (allowNewGameObjectInstantiation)
        {
            minFill = 1;
            // Find the pool index which is the lowest % full.
            for (int i = 0; i < objectPools.Count; i++)
            {
                tempFill = (float)(objectPools[i].objectsFree.Count + objectPools[i].objectsInUse.Count) / (float)objectPools[i].maxPoolSize;
                if (tempFill < minFill)
                {
                    leastFullPoolIndex = i;
                    minFill = tempFill;
                }
            }

            // Instantiate more objects if pool is not full.
            if (minFill < 1) 
            {
                for (int i = 0; i < Mathf.Max(1, maxPoolFillRate / 50.0f); i++)
                {
                    objectPools[leastFullPoolIndex].CreatePoolObject(this);
                }
            }

            yield return new WaitForSeconds(Mathf.Max(1 / maxPoolFillRate, Time.fixedDeltaTime));
        }

        yield return null;
    }

    // Call on awake to ensure there are always some objects in the pool if some other objects requests one.
    // Defaults to providing either Max(10, Min(500 / number of pools, or 2% of the pools suggestedPoolSize variable)).
    void PopulatePoolsOnAwake(){
        for (int i = 0; i < objectPools.Count; i++)
        {
            int minFillAtStart = Mathf.Min(10, objectPools[i].maxPoolSize);
            int suggestedFillAtStart = Mathf.Min(50, (int)(objectPools[i].maxPoolSize / 5));
            int maxFillAtStart = Mathf.Max(100, objectPools[i].maxPoolSize);

            for (int j = 0; j < Mathf.Clamp(suggestedFillAtStart, minFillAtStart, maxFillAtStart); j++)
            {
                objectPools[i].CreatePoolObject(this);
            }
        }
    }

    // Allows for compatability with GameObjectFloatSOEvents
    void ReturnObject(GameObject objectToReturn, float arg0)
    {
        ReturnObject(objectToReturn);
    }

    // Used internally by ObjectPoolMain to return an object to its pool.
    // Uses naive approach of looping through each object in the pool and checking if its InstanceID matches.
    // Objects are deactivated (SetActive(false) when returned to their pool.
    // If the object could not be found in the pool, no actions are performed on it.
    void ReturnObject(GameObject objectToReturn, Pool pool)
    {
        foreach (var item in pool.objectsInUse)
        {
            if (item.GetInstanceID() != objectToReturn.GetInstanceID()) continue;
            
            pool.objectsFree.Push(objectToReturn);
            pool.objectsInUse.Remove(objectToReturn);
            objectToReturn.transform.SetParent(this.transform);
            objectToReturn.SetActive(false);
            return;
        }
    }
}

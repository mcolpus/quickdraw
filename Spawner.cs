using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Spawner : NetworkBehaviour
{
    public List<GameObject> spawnObjects;//chooses a random one
    public float timeBetweenSpawns;
    public Transform spawnLocation;
    public float maxDistance;//if item goes beyond this a new one will spawn

    public Transform spawned;

    [ServerCallback]
	void OnEnable()
    {
        StopAllCoroutines();
        StartCoroutine(Run());
    }
    
    [Server]
    void Spawn()
    {
        int i = Random.Range(0, spawnObjects.Count);
        GameObject obj = Instantiate(spawnObjects[i], spawnLocation.position, spawnLocation.rotation);
        NetworkServer.Spawn(obj);
        spawned = obj.transform;
    }

    IEnumerator Run()
    {
        while(true)
        {
            if(spawned==null)
                Spawn();
            while(spawned!=null && Vector2.Distance(spawned.position, spawnLocation.position) < maxDistance)
            {
                yield return new WaitForSeconds(0.1f);
            }
            spawned = null;
            yield return new WaitForSeconds(timeBetweenSpawns);
        }
    }
}

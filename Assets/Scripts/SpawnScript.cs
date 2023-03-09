using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnScript : MonoBehaviour
{
public float SpawnSizeMin=0.05f;
public float SpawnSizeMax=2f;
    void SpawnSize(){
        float spawnSize = Random.Range(SpawnSizeMin, SpawnSizeMax); //picks the spawn size randomly
        transform.localScale = new Vector3(spawnSize, spawnSize, spawnSize); //sets the spawn size
    }

    void SpawnPos(){
        float spawnX = Random.Range(-7.5f, 7.5f);
        float spawnY = Random.Range(-4f, 4f);
        transform.position = new Vector2(spawnX, spawnY);
    }
    void Start(){
        SpawnSize();
        SpawnPos();
    }
}

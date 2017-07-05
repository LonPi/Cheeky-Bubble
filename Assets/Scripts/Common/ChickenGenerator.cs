using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenGenerator : MonoBehaviour {

    public float minScale, maxScale;
    public float minHeight, maxHeight;
    public float minInterval, maxInterval;
    public Vector2 direction;
    public GameObject BirdPrefab;
    float timer;
    float randScale, randHeight, randInterval;
    float reducedTime;

    // Use this for initialization
    void Start ()
    {
        randInterval = Random.Range(minInterval, maxInterval);
	}
	
	// Update is called once per frame
	void Update ()
    {
        timer += Time.deltaTime;
        
        if (timer >= randInterval)
        {
            randScale = Random.Range(minScale, maxScale);
            randHeight = transform.position.y + Random.Range(minHeight, maxHeight);
            Vector2 position = new Vector2(transform.position.x, randHeight);
            GameObject birdObj = PoolManager.instance.GetObjectfromPool(BirdPrefab);
            birdObj.GetComponent<Chicken>().SetInitialParams(direction, new Vector2(randScale, randScale), position);
            timer = 0;
            randInterval = Random.Range(minInterval, maxInterval) - reducedTime;
            randInterval = Mathf.Clamp(randInterval, 1f, maxInterval);
        }
    }

    public void EquipChickenFeed(float _reducedTime)
    {
        reducedTime = _reducedTime;
    }

    public void UnequipChickenFeed()
    {
        reducedTime = 0;
    }
}

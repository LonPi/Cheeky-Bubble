using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenguinGenerator : MonoBehaviour {

    public float minHeight, maxHeight;
    public float minInterval, maxInterval;
    public Vector2 direction;
    public GameObject PenguinPrefab;
    float timer;
    float randHeight, randInterval;
    float reducedTime;

    // Use this for initialization
    void Start ()
    {
        randInterval = Random.Range(minInterval, maxInterval);
    }
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;

        if (timer >= randInterval)
        {
            randHeight = transform.position.y + Random.Range(minHeight, maxHeight);
            Vector2 position = new Vector2(transform.position.x, randHeight);
            GameObject penguinObj = PoolManager.instance.GetObjectfromPool(PenguinPrefab);
            penguinObj.GetComponent<Penguin>().SetInitialParams(direction, position);
            timer = 0;
            randInterval = Random.Range(minInterval, maxInterval) - reducedTime;
            randInterval = Mathf.Clamp(randInterval, 1f, maxInterval);
        }
    }

    public void EquipPenguinFeed(float _reducedTime)
    {
        reducedTime = _reducedTime;
    }

    public void UnequipPenguinFeed()
    {
        reducedTime = 0;
    }
}

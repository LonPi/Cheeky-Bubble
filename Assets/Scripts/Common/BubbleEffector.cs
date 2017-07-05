using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleEffector : MonoBehaviour {

    public float interval;
    AreaEffector2D effector;
    float direction;
    float timer;
    float bubbleScale;
    float areaEffectorForce;

	// Use this for initialization
	void Start () {
        effector = GetComponent<AreaEffector2D>();
        InitAngle();
        areaEffectorForce = effector.forceMagnitude;
	}

    void InvertDirection()
    {
        direction *= -1;
    }

    void InitAngle()
    {
        direction = Random.Range(-1.0f, 1.0f);
        effector.forceAngle = direction > 0 ? 0 : -180;
    }

    void SetWiggleAmtByScale()
    {
        bubbleScale = transform.parent.localScale.x;
        effector.forceMagnitude = 1 - bubbleScale + Mathf.Abs(areaEffectorForce);
    }

    private void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;
        if (timer >= interval)
        {
            InvertDirection();
            effector.forceAngle = direction > 0 ? 0 : -180;
            SetWiggleAmtByScale();
            timer = 0f;
        }
    }
}

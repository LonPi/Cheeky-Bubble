using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface FlyingObjectInterface {

    bool IsAttractableToBubble();
    void FitIntoBubble(CircleCollider2D bubbleCollider);
    void IncreaseVelocity();
    void SetBubbleCollider(CircleCollider2D bubbleCollider);
    GameObject GetGameObject();
}

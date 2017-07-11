using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Penguin : MonoBehaviour, FlyingObjectInterface
{

    public float minSpeed, maxSpeed;
    public bool isBeingAttractedIntoBubble { get; set; }
    public CircleCollider2D bubbleCollider { get; set; }
    public bool isAttractableToBubble { get; set; }
    public bool onGround { get; set; }
    public GameObject coin;
    
    float lerpAmount = 0.01f;
    const float speedIncreaseAmt = 3f;
    Vector2 direction;
    Rigidbody2D rb2d;
    BoxCollider2D myCollider;
    Animator animator;
    float speed;
    bool isFreeFalling;
    bool velocityIncreased;
    bool isInsideBubble;

    // Use this for initialization
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        SetVelocity();
    }

    private void Update()
    {
        if (!InCameraView())
        {
            PoolManager.instance.ReturnObjectToPool(gameObject);
        }
    }

    private void FixedUpdate()
    {
        isAttractableToBubble = !onGround && !isBeingAttractedIntoBubble && !isFreeFalling &&!isInsideBubble;

        // this is not supposed to happen, log an error if that happens
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Fly") && onGround)
        {
            Debug.LogError("Something weird happened: isInPool: " + PoolManager.instance.isInPool(gameObject) + " position: " + transform.position);
        }

        if (onGround)
        {
            rb2d.isKinematic = true;
            rb2d.velocity = Vector2.zero;
            myCollider.enabled = false;
            AnimateCoin();
        }

        if (isBeingAttractedIntoBubble)
        {
            if (bubbleCollider)
            {
                rb2d.position = Vector2.Lerp(rb2d.position, bubbleCollider.gameObject.transform.position, lerpAmount);
                lerpAmount += 0.01f;
                animator.SetTrigger("chickenCatch");
            }
            else
            {
                rb2d.isKinematic = false;
                rb2d.gravityScale = 1;
                rb2d.velocity = Vector2.zero;
                isBeingAttractedIntoBubble = false;
                isFreeFalling = true;
                animator.SetTrigger("chickenFall");
                SoundManager.Instance.BirdPlayOneShot(SoundManager.Instance.penguinCatch);
            }
        }
    }

    void SetVelocity()
    {
        speed = Random.Range(minSpeed, maxSpeed);
        rb2d.velocity = direction * speed;
    }

    bool InCameraView()
    {
        Vector3 screenPoint = GameManager.instance._cameraRef.WorldToViewportPoint(transform.position);
        bool onScreen = screenPoint.x > 0 && screenPoint.x < 1;
        return onScreen;
    }

    public bool IsAttractableToBubble()
    {
        return isAttractableToBubble;
    }

    public void FitIntoBubble(CircleCollider2D _bubbleCollider)
    {
        isBeingAttractedIntoBubble = true;
        bubbleCollider = _bubbleCollider;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public void SetBubbleCollider(CircleCollider2D collider)
    {
        bubbleCollider = collider;
    }

    public void IncreaseVelocity()
    {
        if (velocityIncreased || isFreeFalling || onGround || bubbleCollider /* dont increase speed if penguin is still inside a bubble*/ || isInsideBubble)
            return;

        speed = speed + speedIncreaseAmt;
        rb2d.velocity = speed * direction;
        animator.SetTrigger("chickenRun");
        SoundManager.Instance.BirdPlayOneShot(SoundManager.Instance.birdRunAway);
        velocityIncreased = true;
    }

    void ResetParams()
    {
        isBeingAttractedIntoBubble = false;
        isAttractableToBubble = false;
        onGround = false;
        isFreeFalling = false;
        isInsideBubble = false;
        bubbleCollider = null;
        velocityIncreased = false;
        lerpAmount = 0.01f;
    }

    public void SetInitialParams(Vector2 dir, Vector2 position)
    {
        ResetParams();
        direction = dir;
        transform.position = position;

        // sprite is facing right side by default
        if (Mathf.Sign(transform.localScale.x) > 0 && direction == Vector2.left || Mathf.Sign(transform.localScale.x) < 0 && direction == Vector2.right)
        {
            Vector3 curScale = transform.localScale;
            curScale.x *= -1;
            transform.localScale = curScale;
        }
        if (rb2d)
        {
            SetVelocity();
            myCollider.enabled = true;
            rb2d.isKinematic = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.tag == "Ground")
        {
            onGround = true;
            //GameManager.instance.IncrementPenguinCount();
        }
    }

    void AnimateCoin()
    {
        GameObject CoinObj = PoolManager.instance.GetObjectfromPool(coin);
        CoinObj.GetComponent<CoinFly>().InitParams(transform.position, CoinFly.Type.PENGUIN);

        // return penguin object back to pool
        PoolManager.instance.ReturnObjectToPool(gameObject);
    }
}

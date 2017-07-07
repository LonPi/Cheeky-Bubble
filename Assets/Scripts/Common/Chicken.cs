using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chicken : MonoBehaviour, FlyingObjectInterface {

    public bool isBeingAttractedIntoBubble { get; set; }
    public CircleCollider2D bubbleCollider { get; set; }
    public bool onGround { get; set; }
    public GameObject coin;

    float lerpAmount = 0.01f;
    const float speedIncreaseAmt = 3f;
    Vector2 direction;
    Rigidbody2D rb2d;
    BoxCollider2D myCollider;
    Animator animator;
    float speed;
    bool velocityIncreased;
    bool isAttractableToBubble;
    bool isFreeFalling;
    bool isInsideBubble;

    // Use this for initialization
    void Start () {
        rb2d = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        SetVelocityByScale();
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

        if (isFreeFalling && Mathf.Abs(rb2d.velocity.x) > 0 )
        {
            Debug.LogError("bird is straying, id:" + this.GetInstanceID() + " velocity: " + rb2d.velocity);
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
                isInsideBubble = true;
            }
            else
            {
                rb2d.isKinematic = false;
                rb2d.gravityScale = 1;
                rb2d.velocity = Vector2.zero;
                isBeingAttractedIntoBubble = false;
                isFreeFalling = true;
                animator.SetTrigger("chickenFall");
                SoundManager.Instance.BirdPlayOneShot(SoundManager.Instance.chickenCatch);
            }
        }
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

    bool InCameraView()
    {
        Vector3 screenPoint = GameManager.instance._cameraRef.WorldToViewportPoint(transform.position);
        bool onScreen = screenPoint.x > -0.1f && screenPoint.x < 1.1f;
        return onScreen;
    }

    void SetVelocityByScale()
    {
        speed = 1 / Mathf.Abs(transform.localScale.x) + 2f;
        rb2d.velocity = speed * direction;
    }

    public void IncreaseVelocity()
    {
        if (velocityIncreased || isFreeFalling || onGround || bubbleCollider /* dont increase speed if chicken is still inside a bubble*/ || isInsideBubble)
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

    public void SetInitialParams(Vector2 dir, Vector2 scale, Vector2 position)
    {
        ResetParams();
        direction = dir;
        transform.localScale = scale;
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
            SetVelocityByScale();
            myCollider.enabled = true;
            rb2d.isKinematic = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.tag == "Ground")
        {
            onGround = true;
            //GameManager.instance.IncrementChickenCount();
        }
    }

    void AnimateCoin()
    {
        Instantiate(coin, this.transform.position, Quaternion.identity).name = "Chicken_Drop";
        for (int x = 0; x < GameManager.instance.coinToAdd; x++)
        {
            Instantiate(coin, this.transform.position, Quaternion.identity).name = "Chicken_Drop";
        }
        PoolManager.instance.ReturnObjectToPool(gameObject);
    }
}

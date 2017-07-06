using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour
{

    public bool isReleased { get; set; }
    public ParticleSystem burst;
    public int multiplier;

    Rigidbody2D rb2d;
    CircleCollider2D myCollider;
    FlyingObjectInterface script;
    float upwardVelocity;
    float lifespanBeforePop = 2f;
    float sizeReductionFactor = 0.99f;
    float buffRadius = 0; // buff stat
    float speedDecreaseAmt;
    static int combo;
    bool containedBird;
    bool isBeingPopped;
    Animator animator;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<CircleCollider2D>();
        animator = GetComponent<Animator>();
        rb2d.isKinematic = true;
        combo = 0;
    }

    private void Update()
    {
        if (isReleased && InCameraView())
        {
            // update velocity based on current bubble size
            SetVelocityByScale();
            if (!containedBird)
            {
                // check if overlap with birds
                if (!isBeingPopped)
                    CheckOverlap();
            }

            // don't shrink when it carries a bird
            else
            {
                CancelInvoke("Shrink");
                Decelerate();
            }
        }

        if (isReleased && !InCameraView())
        {
            if (!containedBird)
            {
                // character is angry when bubble never catches a bird
                // prevent fail state trigger from accumulating
                combo = 0;
                GameManager.instance.SetCoin(combo, multiplier);
                if (!BlowBubble.Instance.IsInFailState && !BlowBubble.Instance.IsInAngryState && !BlowBubble.Instance.IsInPopState)
                {
                    BlowBubble.Instance.SetAnimatorTrigger("fail");
                }
                PoolManager.instance.ReturnObjectToPool(gameObject);
            }
        }
    }

    bool InCameraView()
    {
        Vector3 screenPoint = GameManager.instance._cameraRef.WorldToViewportPoint(transform.position);
        bool onScreen = screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
        return onScreen;
    }

    void Shrink()
    {
        // bubble reduces its size as it travels upwards
        float currentScale = transform.localScale.x;
        currentScale *= sizeReductionFactor;
        Vector3 newScale = new Vector3(currentScale, currentScale, currentScale);
        transform.localScale = newScale;
    }

    public void SetScale(Vector3 scale)
    {
        transform.localScale = scale;
    }

    public void OnRelease()
    {
        rb2d.isKinematic = false;
        InvokeRepeating("Shrink", 0, 0.5f);
        isReleased = true;
        SetVelocityByScale();
    }

    public void EquipMagnet(float radius)
    {
        buffRadius = radius;
    }

    public void UnequipMagnet()
    {
        buffRadius = 0;
    }

    void SetVelocityByScale()
    {
        upwardVelocity = 1 / transform.localScale.x - 1;
        rb2d.velocity = new Vector2(rb2d.velocity.x, upwardVelocity);
    }

    void Decelerate()
    {
        // decelerate based on size of bird it carries
        speedDecreaseAmt = script.GetGameObject().transform.localScale.x + 0.5f;
        Vector2 speed = new Vector2(rb2d.velocity.x, rb2d.velocity.y - speedDecreaseAmt);
        rb2d.velocity = speed;
    }

    void CheckOverlap()
    {
        Bounds bounds = myCollider.bounds;
        // radius of which flying objects can be attracted
        float attractorRadius = myCollider.radius * transform.localScale.x + buffRadius;

        // radius of which flying object can be fit into this bubble
        // also radius of which flying object can be scared
        float radius = myCollider.radius * transform.localScale.x;
        Collider2D attractedBird = Physics2D.OverlapCircle(bounds.center, attractorRadius, 1 << LayerMask.NameToLayer("Bird"));

        if (attractedBird)
        {
            script = attractedBird.gameObject.GetComponent<FlyingObjectInterface>();

            if (script.IsAttractableToBubble())
            {
                // if bird's size fit into this bubble
                if (attractedBird.bounds.size.x <= 2 * radius)
                {
                    script.FitIntoBubble(myCollider);
                    containedBird = true;
                    StartCoroutine(PopBubble());
                }
                // bubble pops and bird flies faster
                else
                {
                    // Make sure bubble does not belong to a bird first before calling the PopBubble routine
                    if (!containedBird)
                    {
                        Collider2D scaredBird = Physics2D.OverlapCircle(bounds.center, radius, 1 << LayerMask.NameToLayer("Bird"));
                        // If a bird is lerping to the middle of this bubble, and this bubble pops to other bigger bird, this currently lerping bird
                        // will appear to be dangling because the bubble that it belongs to has just popped to a bigger bird.
                        if (scaredBird)
                        {
                            script = scaredBird.gameObject.GetComponent<FlyingObjectInterface>();
                            StartCoroutine(PopBubble());

                        }
                    }

                }

            }
        }
    }

    IEnumerator PopBubble()
    {
        // indicate that this coroutine is running, so we don't allow this bubble to catch bird when bubble
        // is about to pop
        isBeingPopped = true;

        if (!containedBird)
        {
            yield return new WaitForSeconds(0.1f);
            SoundManager.Instance.BubblePlayOneShot(SoundManager.Instance.bubblePopBlank);
            animator.SetTrigger("bubblePop");
            script.IncreaseVelocity();
            // prevent angry state trigger from accumulating
            if (!BlowBubble.Instance.IsInFailState && !BlowBubble.Instance.IsInAngryState && !BlowBubble.Instance.IsInPopState)
            {
                BlowBubble.Instance.SetAnimatorTrigger("angry");
            }
            CancelInvoke("Shrink");
            Instantiate(burst, this.transform.position, Quaternion.identity);
            combo++;
            GameManager.instance.SetCoin(combo, multiplier);
            PoolManager.instance.ReturnObjectToPool(gameObject);
        }

        else
        {
            yield return new WaitForSeconds(lifespanBeforePop);
            SoundManager.Instance.BubblePlayOneShot(SoundManager.Instance.bubblePopCatch);
            animator.SetTrigger("bubblePop");
            script.SetBubbleCollider(null);
            script = null;
            CancelInvoke("Shrink");
            Instantiate(burst, this.transform.position, Quaternion.identity);
            combo++;
            GameManager.instance.SetCoin(combo, multiplier);
            PoolManager.instance.ReturnObjectToPool(gameObject);
        }

    }
    
    void ResetParams()
    {
        isBeingPopped = false;
        isReleased = false;
        script = null;
        containedBird = false;
        transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
    }

    public void SetInitialParams(Vector2 position)
    {
        ResetParams();
        transform.position = position;
        if (rb2d)
        {
            rb2d.isKinematic = true;
            rb2d.velocity = Vector2.zero;
        }
    }
}

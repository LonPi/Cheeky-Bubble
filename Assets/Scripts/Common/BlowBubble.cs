using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlowBubble : MonoBehaviour
{
    public AnimatorOverrideController GunAnimator;
    public RuntimeAnimatorController BearAnimator;
    private static BlowBubble instance;
    public static BlowBubble Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.FindObjectOfType<BlowBubble>();
            return instance;
        }
    }

    public bool IsInBlowState { get; set; }
    public bool IsInPopState { get; set; }
    public bool IsInFailState { get; set; }
    public bool IsInAngryState { get; set; }
    public bool MagnetBubblePurchased { get; set; }
    public GameObject BubblePrefab;
    public ParticleSystem burst;
    public Transform mouth;

    float minScale = 0.2f;
    float maxScale = 0.6f;
    float sizeIncreaseFactor = 1.01f;
    public GameObject instantiatedBubble { get; set; }
    public bool animatorChanged;
    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!GameManager.instance.startGame)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 origin = Camera.main.ScreenPointToRay(Input.mousePosition).origin;
            RaycastHit2D hit = Physics2D.Raycast(origin, Vector3.forward, 1 << LayerMask.NameToLayer("Player"));
            if (hit)
            {
                OnMouseClick();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            OnMouseRelease();
        }
    }

    public void EquipBubbleGun(float _sizeIncreaseFactor)
    {
        sizeIncreaseFactor = _sizeIncreaseFactor;
        SwitchAnimatorController();
    }

    public void UnequipBubbleGun()
    {
        sizeIncreaseFactor = 1.01f;
        RestoreAnimatorController();
    }

    public void SetAnimatorTrigger(string param)
    {
        animator.SetTrigger(param);
    }

    public void ChangeBubbleSize()
    {
        Vector3 curScale = instantiatedBubble.transform.localScale;
        curScale.x = curScale.y = curScale.z *= sizeIncreaseFactor;
        if (instantiatedBubble)
        {
            if (instantiatedBubble.transform.localScale.x > maxScale)
            {
                PoolManager.instance.ReturnObjectToPool(instantiatedBubble);
                Instantiate(burst, mouth.transform.position, Quaternion.identity);
                animator.SetTrigger("pop");
                instantiatedBubble = null;
            }
            else
                instantiatedBubble.GetComponent<Bubble>().SetScale(curScale);
        }
    }
    
    public void InstantiateBubble()
    {
        instantiatedBubble = PoolManager.instance.GetObjectfromPool(BubblePrefab);
        instantiatedBubble.GetComponent<Bubble>().SetInitialParams(mouth.position);
        Vector3 scale = new Vector3(minScale, minScale, minScale);
        instantiatedBubble.GetComponent<Bubble>().SetScale(scale);

        if (MagnetBubblePurchased)
        {
            // add to book keeping list to control bubble speed globally
            BuffEffectManager.instance.magnetBubble.SetAndAddToList(instantiatedBubble);
        }
    }

    public void ReleaseBubble()
    {
        if (instantiatedBubble)
        {
            instantiatedBubble.GetComponent<Bubble>().OnRelease();
            instantiatedBubble = null;
        }
    }

    public void OnEndGame()
    {
        animator.SetBool("blow", false);
    }

    void SwitchAnimatorController()
    {
        // use GetComponent<Animator>() because at this point, the variable "animator" might not have been initialized yet in Start()
        GetComponent<Animator>().runtimeAnimatorController = GunAnimator;
        animatorChanged = true;
    }

    void RestoreAnimatorController()
    {
        animator.runtimeAnimatorController = BearAnimator;
        animatorChanged = true;
    }

    void OnMouseClick()
    {
        animator.SetBool("blow", true);
    }

    void OnMouseRelease()
    {
        // important to note that if animator.SetTrigger("pop") is called at the same frame
        // as animator.SetBool("blow", false), BlowBehaviour State will exit twice. This means
        // that the second time we enter BlowBehaviour state, the state will quit instantly and 
        // so the bubble will just get released prematurely.
        if (animator.GetBool("pop") == false)
        {
            animator.SetBool("blow", false);
        }
    }
}

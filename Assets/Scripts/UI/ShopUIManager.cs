using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ShopUIManager : MonoBehaviour {

    public const int
        CHICKEN_FEED_PRICE_IN_CHICKENCURRENCY = 80,
        CHICKEN_FEED_PRICE_IN_PENGUINCURRENCY = 8,
        PENGUIN_FEED_PRICE_IN_CHICKENCURRENCY = 80,
        PENGUIN_FEED_PRICE_IN_PENGUINCURRENCY = 8,
        BUBBLE_GUN_PRICE_IN_CHICKENCURRENCY = 100,
        BUBBLE_GUN_PRICE_IN_PENGUINCURRENCY = 10,
        MAGNET_IN_CHICKENCURRENCY = 50,
        MAGNET_IN_PENGUINCURRENCY = 5;

    public Text chickenCurrencyText, penguinCurrencyText;
    public Button PayChickenBtn, PayPenguinBtn;
    public Text ChickenFeed_ChickenCurrency, ChickenFeed_PenguinCurrency,
        PenguinFeed_ChickenCurrency, PenguinFeed_PenguinCurrency,
        BubbleGun_ChickenCurrency, BubbleGun_PenguinCurrency,
        Magnet_ChickenCurrency, Magnet_PenguinCurrency;
    public Text PayChickenText, PayPenguinText;
    Item clickedItem;

    enum Item
    {
        CHICKEN_FEED,
        PENGUIN_FEED,
        BUBBLE_GUN,
        MAGNET
    }

    int chickenCurrency, penguinCurrency;

	void Start ()
    {
        // set the price tag for items
        ChickenFeed_ChickenCurrency.text = CHICKEN_FEED_PRICE_IN_CHICKENCURRENCY.ToString();
        ChickenFeed_PenguinCurrency.text = CHICKEN_FEED_PRICE_IN_PENGUINCURRENCY.ToString();
        PenguinFeed_ChickenCurrency.text = PENGUIN_FEED_PRICE_IN_CHICKENCURRENCY.ToString();
        PenguinFeed_PenguinCurrency.text = PENGUIN_FEED_PRICE_IN_PENGUINCURRENCY.ToString();
        BubbleGun_ChickenCurrency.text = BUBBLE_GUN_PRICE_IN_CHICKENCURRENCY.ToString();
        BubbleGun_PenguinCurrency.text = BUBBLE_GUN_PRICE_IN_PENGUINCURRENCY.ToString();
        Magnet_ChickenCurrency.text = MAGNET_IN_CHICKENCURRENCY.ToString();
        Magnet_PenguinCurrency.text = MAGNET_IN_PENGUINCURRENCY.ToString();
        PayChickenBtn.gameObject.SetActive(false);
        PayPenguinBtn.gameObject.SetActive(false);
	}
	
	void Update ()
    {
        chickenCurrency = GameDataManager.instance.GetCaughtChickenCount();
        penguinCurrency = GameDataManager.instance.GetCaughtPenguinCount();

        // update the top left inventory icons
        chickenCurrencyText.text = chickenCurrency.ToString();
        penguinCurrencyText.text = penguinCurrency.ToString();

        // update button
        if (clickedItem == Item.CHICKEN_FEED)
        {
            if (chickenCurrency < CHICKEN_FEED_PRICE_IN_CHICKENCURRENCY)
            {
                // grey out UI
                PayChickenBtn.GetComponent<Button>().interactable = false;
            }
            
            if (penguinCurrency < CHICKEN_FEED_PRICE_IN_PENGUINCURRENCY)
            {
                // grey out UI
                PayPenguinBtn.GetComponent<Button>().interactable = false;
            }
        }

        if (clickedItem == Item.PENGUIN_FEED)
        {
            if (chickenCurrency < PENGUIN_FEED_PRICE_IN_CHICKENCURRENCY)
            {
                // grey out UI
                PayChickenBtn.GetComponent<Button>().interactable = false;
            }

            if (penguinCurrency < PENGUIN_FEED_PRICE_IN_PENGUINCURRENCY)
            {
                // grey out UI
                PayPenguinBtn.GetComponent<Button>().interactable = false;
            }
        }

        if (clickedItem == Item.BUBBLE_GUN)
        {
            if (chickenCurrency < BUBBLE_GUN_PRICE_IN_CHICKENCURRENCY)
            {
                // grey out UI
                PayChickenBtn.GetComponent<Button>().interactable = false;
            }

            if (penguinCurrency < BUBBLE_GUN_PRICE_IN_PENGUINCURRENCY)
            {
                // grey out UI
                PayPenguinBtn.GetComponent<Button>().interactable = false;
            }
        }

        if (clickedItem == Item.MAGNET)
        {
            if (chickenCurrency < MAGNET_IN_CHICKENCURRENCY)
            {
                // grey out UI
                PayChickenBtn.GetComponent<Button>().interactable = false;
            }

            if (penguinCurrency < MAGNET_IN_PENGUINCURRENCY)
            {
                // grey out UI
                PayPenguinBtn.GetComponent<Button>().interactable = false;
            }
        }
    }

    void ResetButtonInteractivity()
    {
        PayChickenBtn.GetComponent<Button>().interactable = true;
        PayPenguinBtn.GetComponent<Button>().interactable = true;
    }

    public void OnSelectBackButton()
    {
        SoundManager.Instance.UiPlayOneShot(SoundManager.Instance.buttonClick);
        string previousSceneName = GameManager.instance.GetPreviousSceneName();
        Debug.Log("OnSelectBackBtn: " + previousSceneName);
        if (previousSceneName != "")
        {
            GameManager.instance.GoToScene(previousSceneName);
        }

        else
        {
            Debug.Log("can't switch scene b/c no previous scene found.");
        }
    }

    public void OnSelectPayChicken()
    {
        int remainedChickenCurrency;

        if (clickedItem == Item.CHICKEN_FEED)
        {
            remainedChickenCurrency = GameDataManager.instance.GetCaughtChickenCount() - CHICKEN_FEED_PRICE_IN_CHICKENCURRENCY;
            GameDataManager.instance.SetChickenCount(remainedChickenCurrency);
            BuffEffectManager.instance.chickenFeed.OnPurchase();
        }

        if (clickedItem == Item.PENGUIN_FEED)
        {
            remainedChickenCurrency = GameDataManager.instance.GetCaughtChickenCount() - PENGUIN_FEED_PRICE_IN_CHICKENCURRENCY;
            GameDataManager.instance.SetChickenCount(remainedChickenCurrency);
            BuffEffectManager.instance.penguinFeed.OnPurchase();
        }

        if (clickedItem == Item.BUBBLE_GUN)
        {
            remainedChickenCurrency = GameDataManager.instance.GetCaughtChickenCount() - BUBBLE_GUN_PRICE_IN_CHICKENCURRENCY;
            GameDataManager.instance.SetChickenCount(remainedChickenCurrency);
            BuffEffectManager.instance.bubbleGun.OnPurchase();
        }

        if (clickedItem == Item.MAGNET)
        {
            remainedChickenCurrency = GameDataManager.instance.GetCaughtChickenCount() - MAGNET_IN_CHICKENCURRENCY;
            GameDataManager.instance.SetChickenCount(remainedChickenCurrency);
            BuffEffectManager.instance.magnetBubble.OnPurchase();
        }
        SoundManager.Instance.ShopPlayOneShot(SoundManager.Instance.shopBuy);
    }

    public void OnSelectPayPenguin()
    {
        int remainedPenguinCurrency;

        if (clickedItem == Item.CHICKEN_FEED)
        {
            remainedPenguinCurrency = GameDataManager.instance.GetCaughtPenguinCount() - CHICKEN_FEED_PRICE_IN_PENGUINCURRENCY;
            GameDataManager.instance.SetPenguinCount(remainedPenguinCurrency);
            BuffEffectManager.instance.chickenFeed.OnPurchase();
        }

        if (clickedItem == Item.PENGUIN_FEED)
        {
            remainedPenguinCurrency = GameDataManager.instance.GetCaughtPenguinCount() - PENGUIN_FEED_PRICE_IN_PENGUINCURRENCY;
            GameDataManager.instance.SetPenguinCount(remainedPenguinCurrency);
            BuffEffectManager.instance.penguinFeed.OnPurchase();
        }

        if (clickedItem == Item.BUBBLE_GUN)
        {
            remainedPenguinCurrency = GameDataManager.instance.GetCaughtPenguinCount() - BUBBLE_GUN_PRICE_IN_PENGUINCURRENCY;
            GameDataManager.instance.SetPenguinCount(remainedPenguinCurrency);
            BuffEffectManager.instance.bubbleGun.OnPurchase();
        }

        if (clickedItem == Item.MAGNET)
        {
            remainedPenguinCurrency = GameDataManager.instance.GetCaughtPenguinCount() - MAGNET_IN_PENGUINCURRENCY;
            GameDataManager.instance.SetPenguinCount(remainedPenguinCurrency);
            BuffEffectManager.instance.magnetBubble.OnPurchase();
        }
        SoundManager.Instance.ShopPlayOneShot(SoundManager.Instance.shopBuy);
    }

    public void OnSelectChickenFeed()
    {
        SoundManager.Instance.UiPlayOneShot(SoundManager.Instance.buttonClick);
        PayChickenBtn.gameObject.SetActive(true);
        PayChickenText.text = ChickenFeed_ChickenCurrency.text;
        PayPenguinBtn.gameObject.SetActive(true);
        PayPenguinText.text = ChickenFeed_PenguinCurrency.text;

        ResetButtonInteractivity();
        clickedItem = Item.CHICKEN_FEED;
    }

    public void OnSelectPenguinFeed()
    {
        SoundManager.Instance.UiPlayOneShot(SoundManager.Instance.buttonClick);
        PayChickenBtn.gameObject.SetActive(true);
        PayChickenText.text = PenguinFeed_ChickenCurrency.text;
        PayPenguinBtn.gameObject.SetActive(true);
        PayPenguinText.text = PenguinFeed_PenguinCurrency.text;


        ResetButtonInteractivity();
        clickedItem = Item.PENGUIN_FEED;
    }

    public void OnSelectBubbleGun()
    {
        SoundManager.Instance.UiPlayOneShot(SoundManager.Instance.buttonClick);
        PayChickenBtn.gameObject.SetActive(true);
        PayChickenText.text = BubbleGun_ChickenCurrency.text;
        PayPenguinBtn.gameObject.SetActive(true);
        PayPenguinText.text = BubbleGun_PenguinCurrency.text;

        ResetButtonInteractivity();
        clickedItem = Item.BUBBLE_GUN;
    }

    public void OnSelectMagnet()
    {
        SoundManager.Instance.UiPlayOneShot(SoundManager.Instance.buttonClick);
        PayChickenBtn.gameObject.SetActive(true);
        PayChickenText.text = Magnet_ChickenCurrency.text;
        PayPenguinBtn.gameObject.SetActive(true);
        PayPenguinText.text = Magnet_PenguinCurrency.text;

        ResetButtonInteractivity();
        clickedItem = Item.MAGNET;
    }
}

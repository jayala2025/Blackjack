using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DeckScript : MonoBehaviour
{
    public Sprite[] cardSprites;
    private Sprite cardBack;
    public Color cardBackColor = Color.white;

    // public Sprite[] newCardBacks;
    int[] cardValues = new int[53];
    int currIndex = 0;
    void Start()
    {
        GetCardValues();
        cardBack = cardSprites[0];
    }

    // Update is called once per frame
    void GetCardValues()
    {
        int num = 0;
        // Loop through all 52 cards to assign values to them
        for (int i = 0; i < cardSprites.Length; i++)
        {
            num = i; //Count up to the amount of cards, which is 52
            num %= 13;
            if (num > 10 || num == 0)
            {
                num = 10;
            }
            cardValues[i] = num++;
        }
        //currIndex = 1;
    }

    public void Shuffle()
    {
        // We are just doing a standard array data swapping technique
        for(int i = cardSprites.Length - 1; i > 0; --i)
        {
            int j = Mathf.FloorToInt(Random.Range(0.0f, 1.0f) * cardSprites.Length - 1) + 1;// Random might be an issue here
            Sprite face = cardSprites[i];
            cardSprites[i] = cardSprites[j];
            cardSprites[j] = face;

            int val = cardValues[i];
            cardValues[i] = cardValues[j];
            cardValues[j] = val;
        }
        currIndex = 1;
    }

    public int DealCard(CardScript cardScript)
    {
        cardScript.SetSprite(cardSprites[currIndex]);
        cardScript.SetCardValue(cardValues[currIndex]);//increment index after setting card value 
        cardScript.SetColor(Color.white);
        currIndex++;
        return cardScript.GetCardValue();
    }

    public Sprite GetCardBack()
    {
        return cardBack;
    }

    public void SetCardBackColor(Color newColor)
    {
        cardBackColor = newColor;
        // Update all face-down cards
        CardScript[] allCards = FindObjectsOfType<CardScript>();
        foreach (CardScript card in allCards)
        {
            if (!card.IsFaceUp())
            {
                card.SetColor(newColor);
            }
        }
    }
}

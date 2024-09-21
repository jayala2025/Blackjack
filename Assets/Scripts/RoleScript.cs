using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleScript : MonoBehaviour
{
    // It is called the Role script because it is required for both roles: Player and Dealer
    public CardScript cardScript;
    public DeckScript deckScript;

    public int handValue = 0;

    private int money = 1000;

    public GameObject[] hand;

    public int cardIndex = 0;

    List<CardScript> aceList = new List<CardScript>();

    public void StartHand()
    {
        GetCard();
        GetCard();
    }

    public int GetCard()
    {
        int cardVal = deckScript.DealCard(hand[cardIndex].GetComponent<CardScript>());
        hand[cardIndex].GetComponent<Renderer>().enabled = true;
        handValue += cardVal;

        if (cardVal == 1){
            aceList.Add(hand[cardIndex].GetComponent<CardScript>());
        }
        AceCheck();
        cardIndex++;
        return handValue;
    }

    public void AceCheck()
    {   // foreach Ace check if it will go over 21, adjust value accordingly
        foreach(CardScript ace in aceList)
        {
            if (handValue + 10 <= 21 && ace.GetCardValue() == 1)
            {
                ace.SetCardValue(11);
                handValue += 10;
            } else if (handValue > 21 && ace.GetCardValue() == 11)
            {
                ace.SetCardValue(1);
                handValue -= 10;
            }
        }
    }

    public void AdjustMoney(int amount)
    {
        money += amount;
    }

    // output players current money
    public int GetMoney()
    {
        return money;
    }

    public void ResetHand()
    {
        for (int i = 0; i < hand.Length; i++)
        {
            CardScript cardScript = hand[i].GetComponent<CardScript>();
            cardScript.ResetCard();
            cardScript.SetColor(deckScript.cardBackColor); // Set to current card back color
            hand[i].GetComponent<Renderer>().enabled = false;
        }
        cardIndex = 0;
        handValue = 0;
        aceList = new List<CardScript>();
    }
}

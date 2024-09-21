using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Button dealButton;
    public Button hitButton;
    public Button stayButton;
    public Button betButton;

    public TextMeshProUGUI stayButtonText;

    private int stayClickCounter = 0;
    private int potOfMoney;

    //Access hands
    public RoleScript playerScript;
    public RoleScript dealerScript;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI dealerHandText;
    public TextMeshProUGUI betText;
    public TextMeshProUGUI cashText;
    public TextMeshProUGUI mainText;
    public GameObject hideCard;

    public Canvas startupScreen;
    public Button startGameButton;
    public Button changeCardBackButton;
    public Button readRulesButton;
    public Button quitGameButton;

    public Canvas rulesScreen;
    public Button returnToMenuButton;

    
    
    void Start()
    {
        rulesScreen.gameObject.SetActive(false);
        dealButton.onClick.AddListener(() => DealClicked());
        hitButton.onClick.AddListener(() => HitClicked());
        stayButton.onClick.AddListener(() => StayClicked());
        betButton.onClick.AddListener(() => BetClicked());
        startGameButton.onClick.AddListener(() => StartGameClicked());
        changeCardBackButton.onClick.AddListener(() => ChangeCardBackClicked());
        readRulesButton.onClick.AddListener(() => ReadRulesClicked());
        quitGameButton.onClick.AddListener(() => QuitGameClicked());
        returnToMenuButton.onClick.AddListener(() => ReturnToMenuClicked());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void DealClicked()
    {
        // Resetting our hands
        playerScript.ResetHand();
        dealerScript.ResetHand();
        

        mainText.gameObject.SetActive(false);
        dealerHandText.gameObject.SetActive(false);
        GameObject.Find("Deck").GetComponent<DeckScript>().Shuffle();
        playerScript.StartHand();
        dealerScript.StartHand();
        
        //update score display
        scoreText.text = "Hand: " + playerScript.handValue.ToString();
        dealerHandText.text = "Hand: " + dealerScript.handValue.ToString();
        hideCard.GetComponent<Renderer>().enabled = true;

        // adjusting the visibility of our buttons
        hideCard.GetComponent<Renderer>().enabled = true;
        dealButton.gameObject.SetActive(false);
        hitButton.gameObject.SetActive(true);
        stayButton.gameObject.SetActive(true);
        stayButtonText.text = "Stay";
        // establishing standard pot size
        potOfMoney = 40;
        betText.text = potOfMoney.ToString();
        playerScript.AdjustMoney(-20);
        cashText.text = "$"+ playerScript.GetMoney().ToString();
    }

    private void HitClicked()
    {
        // We are checking if we have room on the table
        if(playerScript.cardIndex <= 10)
        {
            playerScript.GetCard();
            scoreText.text = "Hand: " + playerScript.handValue.ToString();
            if(playerScript.handValue > 20) RoundOver();
        }
    }

    private void StayClicked()
    {
        stayClickCounter++;
        if(stayClickCounter > 1) RoundOver();
        HitDealer();
        stayButtonText.text = "Call";
    }

    private void HitDealer()
    {
        while(dealerScript.handValue < 16 && dealerScript.cardIndex < 10)
        {
            dealerScript.GetCard();
            dealerHandText.text = "Hand: " + dealerScript.handValue.ToString();
            if(dealerScript.handValue > 20) RoundOver();
        }
    }

    // Check for winner and loser, by checking if hand is over
    void RoundOver()
    {
        // checking for bust and blackjack
        bool playerBust = playerScript.handValue > 21;
        bool dealerBust = dealerScript.handValue > 21;
        bool playerBlackjack = playerScript.handValue == 21;
        bool dealerBlackjack = dealerScript.handValue == 21;

        if(stayClickCounter < 2 && !playerBust && !dealerBust && !playerBlackjack && !dealerBlackjack) return;

        bool roundOver = true;
        if(playerBust && dealerBust)
        {
            mainText.text = "Everybody busted: Bets returned";
            playerScript.AdjustMoney(potOfMoney/2);
        }
        // if only player busts
        else if(playerBust || (!dealerBust && dealerScript.handValue > playerScript.handValue))
        {
            mainText.text = "Dealer wins";
        } else if(dealerBust || playerScript.handValue > dealerScript.handValue)
        {
            mainText.text = "You win!";
            playerScript.AdjustMoney(potOfMoney);
        } else if(playerScript.handValue == dealerScript.handValue)
        {
            mainText.text = "Push, returning bets";
            playerScript.AdjustMoney(potOfMoney/2);
        } else
        {
            roundOver = false;
            mainText.text = "Push";
        }

        if (roundOver)
        {   
            hitButton.gameObject.SetActive(false);
            stayButton.gameObject.SetActive(false);
            dealButton.gameObject.SetActive(true);
            betButton.gameObject.SetActive(true);
            mainText.gameObject.SetActive(true);
            scoreText.gameObject.SetActive(true);
            hideCard.GetComponent<Renderer>().enabled = false;
            cashText.text = "$" + playerScript.GetMoney().ToString();
            stayClickCounter = 0;
            ShrinkAllCards();
        }
    }

    void BetClicked()
    {
        TextMeshProUGUI newBet = betButton.GetComponentInChildren(typeof(TextMeshProUGUI)) as TextMeshProUGUI;
        int intBet = int.Parse(newBet.text.ToString().Remove(0, 1));
        playerScript.AdjustMoney(-intBet);
        cashText.text = "$" + playerScript.GetMoney().ToString();
        potOfMoney += (intBet * 2);
        betText.text = "$" + potOfMoney.ToString();
    }

    void StartGameClicked()
    {
        startupScreen.gameObject.SetActive(false);
        changeCardBackButton.gameObject.SetActive(false);
        readRulesButton.gameObject.SetActive(false);
        quitGameButton.gameObject.SetActive(false);
    }

    void ChangeCardBackClicked()
    {
        // Create a new random color
        Color newColor = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, 1f);

        // Get the DeckScript reference
        DeckScript deckScript = GameObject.Find("Deck").GetComponent<DeckScript>();

        if (deckScript != null)
        {
            // Set the new card back color
            deckScript.SetCardBackColor(newColor);
            Debug.Log("Card back color changed successfully.");
        }
        else
        {
            Debug.LogWarning("DeckScript not found.");
        }
    }

    private void ShrinkAllCards()
    {
        Debug.Log("Called ShrinkAllCards()");

        foreach (GameObject card in playerScript.hand)
        {
            
            CardScript cardScript = card.GetComponent<CardScript>();

            // if(cardScript.animator != null)
            // {
            //     cardScript.animator.SetTrigger("ShrinkDisappear");// Trigger the animation
            // }
            cardScript.ShrinkAndDisappear(); // Call the shrink and disappear animation
        }

        foreach (GameObject card in dealerScript.hand)
        {
            CardScript cardScript = card.GetComponent<CardScript>();
            // if(cardScript.animator != null)
            // {
            //     cardScript.animator.SetTrigger("ShrinkDisappear");// Trigger the animation
            // }
            cardScript.ShrinkAndDisappear(); // Call the shrink and disappear animation
        }
    }




    void ReadRulesClicked()
    {
        rulesScreen.gameObject.SetActive(true);
    }

    void QuitGameClicked()
    {
        Application.Quit();
    }

    void ReturnToMenuClicked()
    {
        rulesScreen.gameObject.SetActive(false);
    }
    
}

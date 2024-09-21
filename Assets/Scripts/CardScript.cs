using UnityEngine;

public class CardScript : MonoBehaviour
{
    public int value = 0;
    private SpriteRenderer spriteRenderer;

    public Animator animator;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void ShrinkAndDisappear()
    {
        if (animator != null)
        {
            animator.SetTrigger("CardShrinkDisappear");
        }
    }

    public int GetCardValue()
    {
        return value;
    }

    public void SetCardValue(int newVal)
    {
        value = newVal;
    }

    public string GetSpriteName()
    {
        return spriteRenderer.sprite.name;
    }

    public void SetSprite(Sprite newSprite)
    {
        spriteRenderer.sprite = newSprite;
    }

    public void SetColor(Color newColor)
    {
        spriteRenderer.color = newColor;
    }

    public void ResetCard()
    {
        Sprite back = GameObject.Find("Deck").GetComponent<DeckScript>().GetCardBack();
        spriteRenderer.sprite = back;
        spriteRenderer.color = Color.white; // Reset color to white
        value = 0;
    }

    public bool IsFaceUp()
    {
        return spriteRenderer.sprite != GameObject.Find("Deck").GetComponent<DeckScript>().GetCardBack();
    }
}
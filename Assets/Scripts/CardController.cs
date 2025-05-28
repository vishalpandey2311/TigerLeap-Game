using UnityEngine;
using System.Collections;

public class CardController : MonoBehaviour
{
    private bool isFlipped = false;
    private bool isBusy = false;          // To prevent double clicks
    private Quaternion faceRotation;      // Face-up rotation
    private Quaternion backRotation;      // Face-down rotation
    private Collider col;                 // Collider for interaction
    private bool isMatched = false;       // Flag for matched cards

    [HideInInspector]
    public int cardTypeId = -1;           // Card type identifier for matching

    // Static flag to ensure the countdown is only triggered once
    private static bool countdownStarted = false;

    void Start()
    {
        // It will be face-up at the start
        transform.rotation = faceRotation;

        // Wait for game to actually start before doing anything
        StartCoroutine(WaitForGameStart());
    }

    // NEW: Wait for GameManager to signal game start
    IEnumerator WaitForGameStart()
    {
        // Wait until game is started from GameManager
        while (GameManager.Instance == null || !GameManager.Instance.gameStarted)
        {
            yield return null;
        }
        
        // Now start the initial show and flip
        StartCoroutine(InitialShowAndFlip());
        
        // Trigger the countdown only once from one card
        if (!countdownStarted && GameManager.Instance != null)
        {
            countdownStarted = true;
            GameManager.Instance.StartInitialCountdown();
        }
    }

    void Awake()
    {
        col = GetComponent<Collider>();

        // Initially, the card will be face-up
        // X-axis par 180° rotate karke card ko face-down rakho
        backRotation = Quaternion.Euler(180f, 0f, 0f);

        // Face-up position (flat position)
        faceRotation = Quaternion.Euler(0f, 0f, 0f);
    }

    // Initial seconds ke liye cards ko face-up dikhane ke liye
    IEnumerator InitialShowAndFlip()
    {
        // Card ko initially interaction se disable rakho
        col.enabled = false;
        isBusy = true;

        // Use the time from GameManager instead of hardcoding
        float viewTime = GameManager.Instance != null ? 
            GameManager.Instance.initialViewTime : 3f;
            
        // Wait for the view time
        yield return new WaitForSeconds(viewTime);

        // Card ko face-down karo with animation
        yield return RotateCard(backRotation, 0.5f);

        // Card ke interaction ko enable karo
        col.enabled = true;
        isBusy = false;
    }

    void OnMouseDown()
    {
        // Don't allow interaction until game has started
        if (GameManager.Instance != null && !GameManager.Instance.gameStarted) return;
        
        if (isBusy) return;               // Agr abhi animation chal rahi hai to ignore
        if (isMatched) return;            // Already matched cards ko ignore karo
        
        // Increment attempts when a card is clicked
        if (GameManager.Instance != null && !isFlipped)
        {
            GameManager.Instance.IncrementAttempts();
        }
        
        StartCoroutine(FlipRoutine());
    }

    IEnumerator FlipRoutine()
    {
        isBusy = true;                    // Busy ho gaya
        col.enabled = false;              // Collider disable

        if (!isFlipped)
        {
            // Card ko face-up karo
            yield return RotateCard(faceRotation, 0.5f);
            isFlipped = true;

            // Check if this card matches with any player hand card
            bool hasMatch = CheckForPlayerHandMatch();

            if (hasMatch)
            {
                // Card match ho gaya, permanently face-up rakho
                isMatched = true;

                // Notify GameManager that a match has been found (pass this card reference)
                GameManager.Instance.CardMatched(cardTypeId, this);

                // Keep the card flipped, but don't enable the collider yet
                // The collider will be controlled by the MoveToCollectionGrid method
            }
            else
            {
                // Play wrong match sound
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.PlayWrongMatchSound();
                }
                
                // No match, wait and flip back
                yield return new WaitForSeconds(2f);

                // Wapas face-down position mein jaao
                yield return RotateCard(backRotation, 0.5f);
                isFlipped = false;

                // Enable interaction again
                yield return new WaitForSeconds(0.5f);
                col.enabled = true;
                isBusy = false;
            }
        }
    }

    // Check if this card matches any player hand card
    bool CheckForPlayerHandMatch()
    {
        // Find Player Hand cards
        GameObject playerHandParent = GameObject.FindGameObjectWithTag("PlayerHand");
        if (playerHandParent == null) return false;

        // Get all child objects which are the player hand cards
        foreach (Transform cardTransform in playerHandParent.transform)
        {
            // Get the PlayerHandCardController to access card ID
            PlayerHandCardController handCard = cardTransform.GetComponent<PlayerHandCardController>();
            if (handCard != null && handCard.cardTypeId == this.cardTypeId)
            {
                // Match found!
                return true;
            }
        }

        return false;
    }

    // Smooth rotation Coroutine
    IEnumerator RotateCard(Quaternion targetRot, float duration)
    {
        Quaternion startRot = transform.rotation;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(startRot, targetRot, elapsed / duration);
            yield return null;
        }
        transform.rotation = targetRot;
    }

    // Method to set card type ID (called from CardSpawner)
    public void SetCardTypeId(int id)
    {
        cardTypeId = id;
    }

    // Add a new method to move the card to the collection grid
    public void MoveToCollectionGrid(Vector3 targetPosition)
    {
        // Start the card movement animation
        StartCoroutine(MoveCardToCollection(targetPosition));
    }

    // Coroutine to animate the card moving to the collection
    IEnumerator MoveCardToCollection(Vector3 targetPosition)
    {
        // Disable interactions during animation
        isBusy = true;
        col.enabled = false;
        
        // Store starting position
        Vector3 startPos = transform.position;
        
        // Animate card moving up then to target position
        float moveUpDuration = 0.5f;
        float moveOverDuration = 0.7f;
        float elapsed = 0f;
        
        // First move the card up
        while (elapsed < moveUpDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / moveUpDuration);
            Vector3 liftedPos = startPos + Vector3.up * 2f * t; // Move 2 units up
            transform.position = liftedPos;
            yield return null;
        }
        
        // Then move to the target position
        Vector3 liftedStartPos = transform.position;
        elapsed = 0f;
        
        while (elapsed < moveOverDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / moveOverDuration);
            transform.position = Vector3.Lerp(liftedStartPos, targetPosition + Vector3.up * 0.1f, t);
            yield return null;
        }
        
        // Ensure final position is exact
        transform.position = targetPosition + Vector3.up * 0.1f;
        
        // Keep the card in matched state
        isFlipped = true;
        isMatched = true;
        isBusy = false;
    }

    // Reset the static flag when the scene is unloaded
    void OnDestroy()
    {
        // Reset the static flag when any card is destroyed
        // This ensures it works properly when restarting the game
        countdownStarted = false;
    }

    // Apply a style to collected cards
    // public void ApplyCollectedCardStyle()
    // {
    //     // Change the material or add effects to make collected cards look different
    //     Renderer cardRenderer = GetComponent<Renderer>();
    //     if (cardRenderer != null)
    //     {
    //         // Apply a different material or tint to show it's collected
    //         cardRenderer.material.color = new Color(0.8f, 0.8f, 1f); // Slight blue tint
            
    //         // You could also add a particle effect or other visual indication
    //         // Instantiate(collectionParticleEffect, transform.position, Quaternion.identity);
    //     }
    // }
}
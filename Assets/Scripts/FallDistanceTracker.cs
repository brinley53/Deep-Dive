using UnityEngine;
using TMPro;

public class FallDistanceTracker : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI distanceText;
    private float startY;
    private float maxFallDistance;
    private bool isFalling;

    void Start()
    {
        startY = transform.position.y;
        maxFallDistance = 0f;
    }

    void Update()
    {
        float currentHeight = transform.position.y;
        float currentFallDistance = startY - currentHeight;

        // Only track distance when falling
        if (currentHeight < startY)
        {
            isFalling = true;
            maxFallDistance = Mathf.Max(maxFallDistance, currentFallDistance);
        }

        // Update UI text
        distanceText.text = $"Fall Distance: {maxFallDistance:F1}m";
    }
} 
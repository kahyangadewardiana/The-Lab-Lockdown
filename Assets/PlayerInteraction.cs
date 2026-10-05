using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("References")]
    public Transform cameraTransform;

    [Tooltip("Opsional: UI Text untuk menampilkan prompt interaksi")]
    public Text promptText;

    [Header("Interaction Settings")]
    public float interactDistance = 3f;
    public LayerMask interactableMask = ~0;
    public Key interactKey = Key.E;

    private IInteractable currentTarget;

    private void Update()
    {
        // Jangan deteksi/terima interaksi apapun waktu panel soal terbuka
        if (GameState.IsUIOpen)
        {
            if (promptText != null) promptText.enabled = false;
            currentTarget = null;
            return;
        }

        DetectInteractable();

        bool interactPressed = Keyboard.current != null &&
            Keyboard.current[interactKey].wasPressedThisFrame;

        if (currentTarget != null && interactPressed)
        {
            currentTarget.Interact();
        }
    }

    private void DetectInteractable()
    {
        if (cameraTransform == null)
        {
            currentTarget = null;
            return;
        }

        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactableMask))
        {
            IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();

            if (interactable != null)
            {
                currentTarget = interactable;
                if (promptText != null)
                {
                    promptText.text = interactable.GetPrompt();
                    promptText.enabled = true;
                }
                return;
            }
        }

        currentTarget = null;
        if (promptText != null)
        {
            promptText.enabled = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (cameraTransform == null) return;
        Gizmos.color = currentTarget != null ? Color.green : Color.yellow;
        Gizmos.DrawLine(cameraTransform.position,
            cameraTransform.position + cameraTransform.forward * interactDistance);
    }
}

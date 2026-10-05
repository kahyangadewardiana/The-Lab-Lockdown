// DoorAutoCloser.cs
// Pasang ke GameObject 'DoorTrigger' (Box Collider dengan 'Is Trigger'
// dicentang), diposisikan di celah/lubang pintu. Lihat instruksi setup
// lengkap di komentar DoorInteractable.cs bagian 'AUTO-CLOSE'.

using UnityEngine;

public class DoorAutoCloser : MonoBehaviour
{
    [Tooltip("Drag pintu (GameObject yang punya komponen DoorInteractable) ke sini")]
    public DoorInteractable door;

    [Tooltip("Berapa detik setelah Player 'lewat' sebelum pintu otomatis menutup")]
    public float closeDelay = 1f;

    private void OnTriggerExit(Collider other)
    {
        // Cek apakah yang keluar dari trigger ini Player (lewat komponen
        // FirstPersonController, tidak perlu setting Tag segala)
        if (other.GetComponentInParent<FirstPersonController>() == null) return;

        if (door == null) return;
        if (!door.IsOpen) return; // sudah tertutup, tidak perlu apa-apa

        CancelInvoke(nameof(CloseDoor));
        Invoke(nameof(CloseDoor), closeDelay);
    }

    private void CloseDoor()
    {
        if (door != null && door.IsOpen)
        {
            door.ForceClose();
        }
    }
}

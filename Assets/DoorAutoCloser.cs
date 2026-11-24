// DoorAutoCloser.cs
// Pasang ke GameObject 'DoorTrigger' (Box Collider dengan 'Is Trigger'
// dicentang), diposisikan di celah/lubang pintu. Bisa untuk pintu PUTAR
// (DoorInteractable) maupun pintu GESER (DrawerInteractable).
//
// CARA SETUP:
// 1. Buat GameObject kosong 'DoorTrigger' di celah pintu. PENTING: jangan
//    jadikan child dari daun pintu yang berputar atau bergeser, karena
//    posisinya akan ikut bergerak waktu pintu dibuka. Taruh sejajar di
//    root Hierarchy atau di bawah bingkai/dinding yang diam.
// 2. Add Component > Box Collider, centang 'Is Trigger', tarik ukurannya
//    supaya menutupi seluruh celah pintu.
// 3. Add Component > DoorAutoCloser.
// 4. Isi SALAH SATU field:
//    - 'Door'         untuk pintu putar (yang pakai DoorInteractable)
//    - 'Sliding Door' untuk pintu geser (yang pakai DrawerInteractable)
//    Boleh juga mengisi keduanya kalau satu celah punya dua daun pintu.

using UnityEngine;

public class DoorAutoCloser : MonoBehaviour
{
    [Tooltip("Pintu PUTAR (object yang punya komponen DoorInteractable)")]
    public DoorInteractable door;

    [Tooltip("Pintu GESER (object yang punya komponen DrawerInteractable)")]
    public DrawerInteractable slidingDoor;

    [Tooltip("Berapa detik setelah Player 'lewat' sebelum pintu otomatis menutup")]
    public float closeDelay = 1f;

    private bool AnyDoorOpen()
    {
        bool rotatingOpen = door != null && door.IsOpen;
        bool slidingOpen = slidingDoor != null && slidingDoor.IsOpen;
        return rotatingOpen || slidingOpen;
    }

    private void OnTriggerExit(Collider other)
    {
        // Cek apakah yang keluar dari trigger ini Player (lewat komponen
        // FirstPersonController, tidak perlu setting Tag segala)
        if (other.GetComponentInParent<FirstPersonController>() == null) return;

        if (door == null && slidingDoor == null) return;
        if (!AnyDoorOpen()) return; // sudah tertutup, tidak perlu apa-apa

        CancelInvoke(nameof(CloseDoor));
        Invoke(nameof(CloseDoor), closeDelay);
    }

    private void CloseDoor()
    {
        if (door != null && door.IsOpen)
        {
            door.ForceClose();
        }

        if (slidingDoor != null && slidingDoor.IsOpen)
        {
            slidingDoor.ForceClose();
        }
    }
}
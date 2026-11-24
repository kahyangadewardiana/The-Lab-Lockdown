// CollectibleFuse.cs
// Pasang ke object yang mau jadi "sekring/part" yang bisa diambil
// (bisa pakai furniture kecil apapun sebagai penanda visual, misal
// dari prefab kit baru Anda -- kotak kecil, alat, dll).
//
// CARA SETUP:
// 1. Pilih 3-4 object di ruangan baru untuk jadi sekring.
// 2. Untuk masing-masing: Add Component > Box Collider (kalau belum ada).
// 3. Add Component > CollectibleFuse.
// 4. (Opsional) Isi 'Fuse Name' beda-beda per object kalau mau ada variasi
//    nama (misal "Sekring A", "Sekring B"), atau biarkan default semua.

using UnityEngine;

public class CollectibleFuse : MonoBehaviour, IInteractable
{
    public string fuseName = "Sekring";

    [Header("Petunjuk Berikutnya")]
    [TextArea(2, 4)]
    public string hintToNext = "";
    [Tooltip("Opsional: sekring berikutnya (atau panel listrik kalau ini sekring terakhir), untuk arah & jarak")]
    public Transform nextTarget;
    public bool showDirection = false;

    [Header("Suara (opsional)")]
    public AudioClip pickupSound;

    private bool collected = false;

    public void Interact()
    {
        if (collected) return;
        collected = true;
        AudioManager.PlayAt(pickupSound, transform.position);

        if (FusePanelManager.Instance != null)
        {
            FusePanelManager.Instance.CollectFuse();
        }

        ShowPickupMessage();

        // Sembunyikan object ini (sudah diambil). Bisa diganti Destroy(gameObject)
        // kalau tidak butuh dipakai lagi sama sekali.
        gameObject.SetActive(false);
    }

    private void ShowPickupMessage()
    {
        if (ClueUIManager.Instance == null) return;

        string message = $"Mengambil {fuseName}";

        if (FusePanelManager.Instance != null)
        {
            message += $" ({FusePanelManager.Instance.GetCollectedCount()}/{FusePanelManager.Instance.totalFusesNeeded})";

            if (FusePanelManager.Instance.AllFusesCollected())
            {
                message += "\nSemua sekring sudah terkumpul. Pasang di panel listrik.";
            }
        }

        string hint = NextHint.Compose(hintToNext, nextTarget, showDirection);
        if (hint.Length > 0) message += "\n" + hint;

        ClueUIManager.Instance.ShowMessage(message, 6f);
    }

    public string GetPrompt()
    {
        return $"Tekan E untuk ambil {fuseName}";
    }
}
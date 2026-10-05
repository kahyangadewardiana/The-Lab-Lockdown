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
    private bool collected = false;

    public void Interact()
    {
        if (collected) return;
        collected = true;

        if (FusePanelManager.Instance != null)
        {
            FusePanelManager.Instance.CollectFuse();
        }

        // Sembunyikan object ini (sudah diambil). Bisa diganti Destroy(gameObject)
        // kalau tidak butuh dipakai lagi sama sekali.
        gameObject.SetActive(false);
    }

    public string GetPrompt()
    {
        return $"Tekan E untuk ambil {fuseName}";
    }
}

// RepairableServer.cs
// Pasang script ini ke SETIAP object server yang mau bisa "diperbaiki"
// (pilih 3-5 object 'server'/'server_2' yang Anda mau, tidak perlu semua).
//
// CARA SETUP:
// 1. Pastikan object server itu punya Collider (Box Collider) supaya
//    raycast PlayerInteraction bisa kena -- kalau belum ada, Add Component
//    > Box Collider dulu.
// 2. Add Component > RepairableServer ke object server itu.
// 3. (Opsional) Kalau mau ada efek visual saat sudah diperbaiki, drag
//    sebuah GameObject (misal lampu indikator hijau kecil, atau cukup
//    Renderer server itu sendiri) ke field 'Fixed Indicator' / biarkan
//    kosong kalau tidak perlu efek.

using UnityEngine;

public class RepairableServer : MonoBehaviour, IInteractable
{
    [Header("Status")]
    public bool isFixed = false;

    [Header("Opsional: efek visual saat sudah diperbaiki")]
    [Tooltip("GameObject yang akan DIAKTIFKAN setelah server ini diperbaiki, " +
             "misal lampu indikator hijau kecil. Boleh dikosongkan.")]
    public GameObject fixedIndicator;

    [Tooltip("Kalau diisi, warna material server ini akan diganti jadi warna " +
             "ini setelah diperbaiki (indikasi visual sederhana tanpa perlu " +
             "prefab tambahan). Boleh dikosongkan kalau tidak perlu.")]
    public bool tintGreenWhenFixed = true;

    private Renderer rend;

    private void Awake()
    {
        rend = GetComponent<Renderer>();

        if (fixedIndicator != null)
        {
            fixedIndicator.SetActive(false);
        }
    }

    // =====================================================
    // IInteractable
    // =====================================================

    public void Interact()
    {
        if (isFixed)
        {
            return; // sudah beres, tidak perlu apa-apa lagi
        }

        if (CodeKeypadManager.Instance != null)
        {
            CodeKeypadManager.Instance.OpenKeypad(this);
        }
        else
        {
            Debug.LogWarning("CodeKeypadManager belum ada di scene! " +
                "Pastikan sudah setup sesuai instruksi di CodeKeypadManager.cs");
        }
    }

    public string GetPrompt()
    {
        return isFixed
            ? "Server ini sudah diperbaiki"
            : "Tekan E untuk masukkan kode perbaikan";
    }

    // =====================================================
    // Dipanggil oleh ServerPuzzleManager kalau jawaban BENAR
    // =====================================================

    public void OnRepaired()
    {
        isFixed = true;

        if (fixedIndicator != null)
        {
            fixedIndicator.SetActive(true);
        }

        if (tintGreenWhenFixed && rend != null)
        {
            // Ganti warna material jadi kehijauan sebagai tanda "sudah beres".
            // Pakai instance material (bukan shared) supaya server lain
            // tidak ikut berubah warna.
            rend.material.color = Color.green;
        }

        MissionManager.Instance?.NotifyServerFixed();
    }
}

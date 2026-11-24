// GhostWarningUI.cs
// Pasang ke GameObject kosong di scene gameplay, misal "GhostWarningUI".
// JANGAN pasang di GameObject Text-nya sendiri, karena Text itu akan
// disembunyikan dan script ikut berhenti.
//
// FUNGSI: Menampilkan tulisan peringatan berkedip di layar (misal
// "Ada sesuatu yang mendekat...") beberapa detik sebelum hantu muncul.
// Dipanggil otomatis oleh GhostAI.
//
// CARA SETUP:
// 1. Di Canvas, klik kanan > UI > Legacy > Text, rename 'GhostWarningText'.
//    Atur warna merah, font besar (misal 36), alignment tengah, taruh di
//    bagian atas-tengah layar. Teks isinya bebas, nanti ditimpa script.
// 2. Buat GameObject kosong 'GhostWarningUI', Add Component > GhostWarningUI.
// 3. Drag 'GhostWarningText' ke field 'Warning Text'.
//    (Tulisan peringatannya sendiri diatur di field 'Warning Message'
//    pada komponen GhostAI.)

using UnityEngine;
using UnityEngine.UI;

public class GhostWarningUI : MonoBehaviour
{
    public static GhostWarningUI Instance { get; private set; }

    public Text warningText;

    [Tooltip("Seberapa cepat tulisan berkedip")]
    public float blinkSpeed = 8f;

    [Tooltip("Transparansi paling redup saat berkedip (0 sampai 1)")]
    [Range(0f, 1f)] public float minAlpha = 0.3f;

    private bool showing = false;
    private float hideAtTime;

    private void Awake()
    {
        Instance = this;

        if (warningText != null) warningText.gameObject.SetActive(false);
    }

    public void Show(string message, float duration)
    {
        if (warningText == null) return;

        warningText.text = message;
        warningText.gameObject.SetActive(true);

        showing = true;
        hideAtTime = Time.time + duration;
    }

    public void Hide()
    {
        showing = false;

        if (warningText != null) warningText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!showing || warningText == null) return;

        if (Time.time >= hideAtTime)
        {
            Hide();
            return;
        }

        // Kedip halus: transparansi naik-turun
        float wave = (Mathf.Sin(Time.time * blinkSpeed) + 1f) * 0.5f;

        Color color = warningText.color;
        color.a = Mathf.Lerp(minAlpha, 1f, wave);
        warningText.color = color;
    }
}

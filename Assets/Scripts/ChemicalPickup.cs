// ChemicalPickup.cs
// Pasang ke object bahan kimia di lab (botol, labu, dll).
//
// CARA SETUP:
// 1. Pilih 4 object bahan kimia. Untuk masing-masing: Add Component >
//    Box Collider (kalau belum ada), lalu Add Component > ChemicalPickup.
// 2. Isi 'Chemical Name' beda-beda. Nama ini HARUS SAMA PERSIS dengan
//    nama tombol di FormulaMixerManager (termasuk huruf besar/kecil).
//
// PETUNJUK KE BAHAN BERIKUTNYA (opsional, tapi disarankan):
// 3. 'Hint To Next': tulis petunjuk tempat bahan berikutnya, misal
//    "Terdengar denting kaca dari rak di sudut kiri ruangan."
//    Petunjuk ini muncul sesaat setelah bahan ini diambil.
// 4. (Opsional) 'Next Chemical': drag object ChemicalPickup bahan
//    berikutnya ke sini, lalu centang 'Show Direction To Next'. Maka
//    pesan juga menampilkan arah dan jarak ke bahan itu, misal
//    "Arah bahan berikutnya: kiri-depan, sekitar 8 meter".
//
// Catatan: Player bebas mengambil bahan dalam urutan apa saja, jadi
// anggap petunjuk ini dorongan halus, bukan jalur wajib. Kalau bahan
// berikutnya ternyata sudah diambil, arah tidak ditampilkan.
// Pesan memakai ClueUIManager (popup bawah layar) yang sama dengan clue
// angka dan catatan cerita.

using UnityEngine;

public class ChemicalPickup : MonoBehaviour, IInteractable
{
    [Tooltip("Nama bahan kimia ini, HARUS SAMA PERSIS dengan nama tombol " +
             "yang dipasang di FormulaMixerManager")]
    public string chemicalName = "Biru";

    [Header("Petunjuk ke Bahan Berikutnya")]
    [TextArea(2, 4)]
    public string hintToNext = "";

    [Tooltip("Opsional: bahan kimia berikutnya, dipakai untuk menampilkan arah & jarak")]
    public ChemicalPickup nextChemical;

    [Tooltip("Kalau dicentang (dan Next Chemical terisi), pesan menyertakan arah & jarak")]
    public bool showDirectionToNext = false;

    [Tooltip("Berapa lama pesan ditampilkan (detik)")]
    public float hintDuration = 6f;

    [Header("Suara (opsional)")]
    public AudioClip pickupSound;

    private bool collected = false;

    public void Interact()
    {
        if (collected) return;
        collected = true;
        AudioManager.PlayAt(pickupSound, transform.position);

        if (FormulaMixerManager.Instance != null)
        {
            FormulaMixerManager.Instance.CollectChemical(chemicalName);
        }

        ShowPickupMessage();

        gameObject.SetActive(false);
    }

    private void ShowPickupMessage()
    {
        if (ClueUIManager.Instance == null) return;

        string message = $"Mengambil bahan kimia: {chemicalName}";

        bool allCollected = FormulaMixerManager.Instance != null &&
                            FormulaMixerManager.Instance.AllChemicalsCollected();

        if (allCollected)
        {
            message += "\nSemua bahan sudah terkumpul. Kembali ke meja eksperimen untuk meracik.";
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(hintToNext))
            {
                message += "\n" + hintToNext;
            }

            if (showDirectionToNext && nextChemical != null && !nextChemical.collected)
            {
                string direction = DescribeDirection(nextChemical.transform.position);
                if (direction != null)
                {
                    message += "\nArah bahan berikutnya: " + direction;
                }
            }
        }

        ClueUIManager.Instance.ShowMessage(message, hintDuration);
    }

    // Arah relatif terhadap posisi & arah hadap Player saat ini.
    private string DescribeDirection(Vector3 targetPosition)
    {
        FirstPersonController player = FindFirstObjectByType<FirstPersonController>();
        if (player == null) return null;

        Vector3 local = player.transform.InverseTransformPoint(targetPosition);
        local.y = 0f;

        float distance = local.magnitude;
        float angle = Mathf.Atan2(local.x, local.z) * Mathf.Rad2Deg; // 0 = depan, positif = kanan

        string[] names =
        {
            "depan", "kanan-depan", "kanan", "kanan-belakang",
            "belakang", "kiri-belakang", "kiri", "kiri-depan"
        };

        int index = Mathf.RoundToInt(((angle + 360f) % 360f) / 45f) % 8;
        return $"{names[index]}, sekitar {Mathf.RoundToInt(distance)} meter";
    }

    public string GetPrompt()
    {
        return $"Tekan E untuk ambil bahan kimia ({chemicalName})";
    }
}
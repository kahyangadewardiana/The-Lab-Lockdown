// ChemicalPickup.cs
// Pasang ke object bahan kimia di lab (botol, labu, dll dari asset lab
// yang baru diimport).
//
// CARA SETUP:
// 1. Pilih 4 object bahan kimia (misal florence_flask, china dish,
//    lab glass, crucible -- bebas 4 model apa saja dari asset lab).
// 2. Untuk masing-masing: Add Component > Box Collider (kalau belum ada).
// 3. Add Component > ChemicalPickup.
// 4. Isi 'Chemical Name' beda-beda per object -- nama ini yang dipakai
//    di tombol panel racik nanti, HARUS SAMA PERSIS antara sini dengan
//    yang di FormulaMixerManager (termasuk huruf besar/kecil).

using UnityEngine;

public class ChemicalPickup : MonoBehaviour, IInteractable
{
    [Tooltip("Nama bahan kimia ini, HARUS SAMA PERSIS dengan nama tombol " +
             "yang nanti dipasang di FormulaMixerManager")]
    public string chemicalName = "Biru";

    private bool collected = false;

    public void Interact()
    {
        if (collected) return;
        collected = true;

        if (FormulaMixerManager.Instance != null)
        {
            FormulaMixerManager.Instance.CollectChemical(chemicalName);
        }

        gameObject.SetActive(false);
    }

    public string GetPrompt()
    {
        return $"Tekan E untuk ambil bahan kimia ({chemicalName})";
    }
}

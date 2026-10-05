// ExperimentTable.cs
// Pasang ke meja eksperimen di lab (object 'table with drawers' atau
// meja lain yang cocok).
//
// CARA SETUP:
// 1. Pastikan object ini punya Collider.
// 2. Add Component > ExperimentTable.

using UnityEngine;

public class ExperimentTable : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        if (FormulaMixerManager.Instance == null)
        {
            Debug.LogWarning("ExperimentTable: FormulaMixerManager belum ada di scene!");
            return;
        }

        if (!FormulaMixerManager.Instance.AllChemicalsCollected())
        {
            return; // belum cukup bahan, prompt sudah kasih tahu lewat GetPrompt()
        }

        FormulaMixerManager.Instance.OpenPanel();
    }

    public string GetPrompt()
    {
        if (FormulaMixerManager.Instance != null && !FormulaMixerManager.Instance.AllChemicalsCollected())
        {
            return $"Butuh {FormulaMixerManager.Instance.GetRemainingCount()} bahan kimia lagi";
        }
        return "Tekan E untuk meracik di meja ini";
    }
}

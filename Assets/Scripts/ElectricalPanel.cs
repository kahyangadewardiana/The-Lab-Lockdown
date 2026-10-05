// ElectricalPanel.cs
// Pasang ke object panel listrik di ruangan baru (misal box listrik,
// atau furniture apapun yang cocok jadi 'panel').
//
// CARA SETUP:
// 1. Pastikan object ini punya Collider.
// 2. Add Component > ElectricalPanel.
// 3. Di field 'Lights To Enable', drag semua Light/GameObject lampu
//    yang mau MENYALA setelah panel diperbaiki (bisa banyak sekaligus).
// 4. (Opsional) Di field 'Doors To Unlock', drag pintu (yang punya
//    DoorInteractable dengan 'Locked Until Mission Complete') yang mau
//    otomatis terbuka kuncinya setelah panel ini diperbaiki.

using System.Collections.Generic;
using UnityEngine;

public class ElectricalPanel : MonoBehaviour, IInteractable
{
    [Tooltip("Semua GameObject lampu yang akan DIAKTIFKAN setelah panel diperbaiki")]
    public List<GameObject> lightsToEnable = new List<GameObject>();

    [Tooltip("Pintu-pintu yang otomatis ke-unlock setelah panel ini diperbaiki (opsional)")]
    public List<DoorInteractable> doorsToUnlock = new List<DoorInteractable>();

    private bool isFixed = false;

    public void Interact()
    {
        if (isFixed) return;

        if (FusePanelManager.Instance == null)
        {
            Debug.LogWarning("ElectricalPanel: FusePanelManager belum ada di scene!");
            return;
        }

        if (!FusePanelManager.Instance.AllFusesCollected())
        {
            // Belum cukup sekring, belum bisa diperbaiki.
            // (prompt teksnya sudah otomatis kasih tahu lewat GetPrompt())
            return;
        }

        isFixed = true;

        foreach (GameObject light in lightsToEnable)
        {
            if (light != null) light.SetActive(true);
        }

        foreach (DoorInteractable door in doorsToUnlock)
        {
            if (door != null) door.lockedUntilMissionComplete = false;
        }
    }

    public string GetPrompt()
    {
        if (isFixed)
        {
            return "Panel listrik sudah diperbaiki";
        }

        if (FusePanelManager.Instance != null && !FusePanelManager.Instance.AllFusesCollected())
        {
            return $"Butuh {FusePanelManager.Instance.GetRemaining()} sekring lagi";
        }

        return "Tekan E untuk pasang semua sekring";
    }
}

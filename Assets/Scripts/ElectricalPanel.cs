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

    [Tooltip("Pintu GESER (yang pakai DrawerInteractable) yang ikut ke-unlock (opsional)")]
    public List<DrawerInteractable> drawersToUnlock = new List<DrawerInteractable>();

    [Header("Petunjuk Setelah Panel Diperbaiki")]
    [TextArea(2, 4)]
    public string hintAfterFixed = "";
    [Tooltip("Opsional: tujuan berikutnya (misal pintu yang baru terbuka), untuk arah & jarak")]
    public Transform nextTarget;
    public bool showDirection = false;

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
        MissionEvents.Complete(MissionId.ElectricalPanel);
        ShowFixedMessage();

        foreach (GameObject light in lightsToEnable)
        {
            if (light != null) light.SetActive(true);
        }

        foreach (DoorInteractable door in doorsToUnlock)
        {
            if (door != null) door.Unlock();
        }

        foreach (DrawerInteractable drawer in drawersToUnlock)
        {
            if (drawer != null) drawer.Unlock();
        }
    }

    private void ShowFixedMessage()
    {
        if (ClueUIManager.Instance == null) return;

        string message = "Panel listrik menyala kembali.";

        string hint = NextHint.Compose(hintAfterFixed, nextTarget, showDirection);
        if (hint.Length > 0) message += "\n" + hint;

        ClueUIManager.Instance.ShowMessage(message, 6f);
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
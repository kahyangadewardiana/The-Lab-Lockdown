// FusePanelManager.cs
// Pasang ke GameObject kosong, misal "FusePanelManager".
//
// FUNGSI: Melacak berapa sekring sudah dikumpulkan, menampilkan progress
// di layar (opsional), dan menyediakan status "semua sekring sudah
// terkumpul" untuk dicek oleh ElectricalPanel.cs.
//
// CARA SETUP:
// 1. Buat GameObject kosong "FusePanelManager", pasang script ini.
// 2. Isi 'Total Fuses Needed' sesuai jumlah CollectibleFuse yang Anda
//    sebar (misal 4).
// 3. (Opsional) Buat UI Text di Canvas untuk progress ("Sekring: 0/4"),
//    drag ke field 'Progress Text'.

using UnityEngine;
using UnityEngine.UI;

public class FusePanelManager : MonoBehaviour
{
    public static FusePanelManager Instance { get; private set; }

    public int totalFusesNeeded = 4;
    public Text progressText;

    private int collectedCount = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateUI();
    }

    public void CollectFuse()
    {
        collectedCount++;
        UpdateUI();
    }

    // Dipakai ScoreManager untuk menghitung skor akhir.
    public int GetCollectedCount() => collectedCount;

    public bool AllFusesCollected()
    {
        return collectedCount >= totalFusesNeeded;
    }

    public int GetRemaining()
    {
        return Mathf.Max(0, totalFusesNeeded - collectedCount);
    }

    private void UpdateUI()
    {
        if (progressText != null)
        {
            progressText.text = $"Sekring: {collectedCount}/{totalFusesNeeded}";
        }
    }
}
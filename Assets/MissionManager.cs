// MissionManager.cs
// Pasang script ini ke GameObject kosong, misal "MissionManager"
// (boleh sejajar dengan ServerPuzzleManager).
//
// CARA SETUP:
// 1. Setelah semua RepairableServer sudah dipasang ke 3-5 server pilihan
//    Anda, drag SEMUA object server itu (yang punya RepairableServer) ke
//    field 'Servers To Fix' di Inspector script ini (drag satu-satu, atau
//    select semuanya sekaligus di Hierarchy lalu drag bareng ke List).
// 2. (Opsional) Buat UI Text baru di Canvas (klik kanan Canvas > UI >
//    Text), beri nama 'MissionProgressText', taruh di pojok layar
//    (misal kiri atas). Drag Text itu ke field 'Progress Text' di sini.
// 3. (Opsional) Buat UI Text/Panel lain untuk pesan 'Misi Selesai!',
//    drag ke field 'Mission Complete Panel', biarkan tidak aktif di awal.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance { get; private set; }

    [Header("Daftar server yang perlu diperbaiki")]
    public List<RepairableServer> serversToFix = new List<RepairableServer>();

    [Header("UI (opsional)")]
    public Text progressText;
    public GameObject missionCompletePanel;

    private int fixedCount = 0;

    private void Awake()
    {
        Instance = this;

        if (missionCompletePanel != null)
        {
            missionCompletePanel.SetActive(false);
        }
    }

    private void Start()
    {
        UpdateProgressUI();
    }

    public void NotifyServerFixed()
    {
        fixedCount++;
        UpdateProgressUI();

        if (fixedCount >= serversToFix.Count)
        {
            OnMissionComplete();
        }
    }

    // Dipakai script lain (misal DoorInteractable) untuk cek apakah
    // misi sudah selesai semua.
    // Dipakai ScoreManager untuk menghitung skor akhir.
    public int GetFixedCount() => fixedCount;
    public int GetTotalServers() => serversToFix.Count;

    public bool IsMissionComplete()
    {
        return fixedCount >= serversToFix.Count;
    }

    private void UpdateProgressUI()
    {
        if (progressText != null)
        {
            progressText.text = $"Server diperbaiki: {fixedCount}/{serversToFix.Count}";
        }
    }

    private void OnMissionComplete()
    {
        Debug.Log("Misi selesai! Semua server sudah diperbaiki.");

        if (missionCompletePanel != null)
        {
            missionCompletePanel.SetActive(true);
        }
    }
}
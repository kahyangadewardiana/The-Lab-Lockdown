// ScoreManager.cs
// Pasang ke GameObject kosong di SCENE GAMEPLAY UTAMA, misal "ScoreManager".
//
// FUNGSI: Menjumlahkan skor dari semua sistem misi yang ada (server
// diperbaiki, sekring terkumpul, formula selesai), jadi 1 angka Score
// dan 1 angka Total Score.
//
// CARA SETUP:
// 1. Buat GameObject kosong "ScoreManager", pasang script ini.
// 2. Drag "MissionManager", "FusePanelManager", "FormulaMixerManager"
//    (yang sudah ada di scene Anda) ke field masing-masing. Kosongkan
//    field yang memang tidak dipakai di scene Anda (aman, otomatis
//    dilewati).

using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("Drag semua manager misi yang ada di scene Anda")]
    public MissionManager missionManager;
    public FusePanelManager fusePanelManager;
    public FormulaMixerManager formulaMixerManager;

    private void Awake()
    {
        Instance = this;
    }

    public int GetScore()
    {
        int score = 0;

        if (missionManager != null) score += missionManager.GetFixedCount();
        if (fusePanelManager != null) score += fusePanelManager.GetCollectedCount();
        if (formulaMixerManager != null) score += formulaMixerManager.IsComplete() ? 1 : 0;

        return score;
    }

    public int GetTotalScore()
    {
        int total = 0;

        if (missionManager != null) total += missionManager.GetTotalServers();
        if (fusePanelManager != null) total += fusePanelManager.totalFusesNeeded;
        if (formulaMixerManager != null) total += 1;

        return total;
    }
}

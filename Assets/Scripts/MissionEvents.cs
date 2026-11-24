// MissionEvents.cs
// Taruh di Assets/Scripts/. TIDAK perlu ditempel ke GameObject manapun.
//
// FUNGSI: "Papan pengumuman" pusat. Setiap misi memanggil
// MissionEvents.Complete(...) begitu selesai, dan setiap pintu/laci
// yang memilih misi itu di Inspector-nya otomatis terbuka kuncinya.
// Jadi berlaku untuk SEMUA misi, tanpa perlu drag-drag ke banyak list.
//
// Kalau nanti Anda bikin misi baru, tinggal:
// 1. Tambahkan nama misinya di enum MissionId di bawah ini.
// 2. Panggil MissionEvents.Complete(MissionId.NamaMisiBaru); di script
//    misi itu saat selesai.

using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public enum MissionId
{
    None,             // tidak dibuka otomatis oleh misi manapun
    ServerRepair,     // perbaiki semua server (MissionManager)
    ElectricalPanel,  // panel listrik + sekring (ElectricalPanel)
    UploadData,       // upload data tahan E (UploadTerminal)
    FormulaMix        // racik formula di lab (FormulaMixerManager)
}

public static class MissionEvents
{
    public static event Action<MissionId> OnMissionCompleted;

    private static readonly HashSet<MissionId> completed = new HashSet<MissionId>();

    public static void Complete(MissionId id)
    {
        if (id == MissionId.None) return;
        if (!completed.Add(id)) return; // sudah pernah diumumkan, abaikan

        OnMissionCompleted?.Invoke(id);
    }

    public static bool IsCompleted(MissionId id)
    {
        return completed.Contains(id);
    }

    // Reset otomatis supaya Restart/main lagi mulai dari nol
    // (status ini static, jadi tidak hilang sendiri waktu pindah scene).
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Init()
    {
        completed.Clear();
        OnMissionCompleted = null;

        SceneManager.sceneUnloaded -= HandleSceneUnloaded;
        SceneManager.sceneUnloaded += HandleSceneUnloaded;
    }

    private static void HandleSceneUnloaded(Scene scene)
    {
        completed.Clear();
    }
}

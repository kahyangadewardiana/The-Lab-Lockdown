// GameState.cs
// Static kecil buat komunikasi antar script: kasih tahu FirstPersonController
// dan PlayerInteraction supaya BERHENTI merespon input WASD/mouse/E waktu
// panel soal logika sedang terbuka.
//
// Tidak perlu ditempel ke GameObject manapun, cukup taruh file ini di
// Assets/Scripts/.

public static class GameState
{
    // True kalau ada UI (misal panel soal logika) yang sedang terbuka
    // dan harus menghentikan sementara kontrol Player.
    public static bool IsUIOpen = false;
}

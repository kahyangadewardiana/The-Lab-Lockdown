// GameScoreData.cs
// Static kecil buat "membawa" data skor & waktu dari scene gameplay ke
// scene WinScene (karena pindah scene menghapus semua GameObject lama,
// data perlu disimpan di tempat yang tidak ikut hilang).
//
// Tidak perlu ditempel ke GameObject manapun, cukup taruh file ini di
// Assets/Scripts/.

public static class GameScoreData
{
    public static int FinalScore;
    public static int FinalTotalScore;
    public static string FinalTimeFormatted = "00:00";
}

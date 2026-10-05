// LogicQuestion.cs
// Struktur data 1 soal logika: pertanyaan + jawaban yang benar (teks bebas).
// Taruh di Assets/Scripts/.

using System;
using UnityEngine;

[Serializable]
public class LogicQuestion
{
    [TextArea(2, 4)]
    public string questionText;

    // Jawaban benar, dibandingkan tanpa peduli huruf besar/kecil dan spasi
    // di awal/akhir. Untuk jawaban angka, cukup tulis angkanya saja
    // (misal "8"), untuk jawaban kata cukup tulis kata itu (misal "ikan").
    public string correctAnswer;

    public bool CheckAnswer(string playerInput)
    {
        if (string.IsNullOrEmpty(playerInput)) return false;

        string a = playerInput.Trim().ToLowerInvariant();
        string b = correctAnswer.Trim().ToLowerInvariant();

        return a == b;
    }
}

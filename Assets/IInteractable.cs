// IInteractable.cs
// Interface umum untuk semua objek yang bisa di-interact (pintu, tombol,
// laci, dll). Simpan di Assets/Scripts/.
//
// Nanti kalau mau bikin interaksi lain (misal tombol lift, laci meja),
// tinggal buat script baru yang "implements IInteractable" seperti
// DoorInteractable.cs, tidak perlu ubah script Player.

public interface IInteractable
{
    // Dipanggil saat player menekan tombol interact (E) sambil menatap objek ini
    void Interact();

    // Teks yang muncul di prompt UI, misal "Tekan E untuk buka pintu"
    string GetPrompt();
}

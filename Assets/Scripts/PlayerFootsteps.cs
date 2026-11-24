// PlayerFootsteps.cs
// Pasang ke GameObject 'Player' (yang punya Rigidbody dan
// FirstPersonController).
//
// FUNGSI: Memutar suara langkah kaki sesuai jarak yang ditempuh, jadi
// otomatis lebih cepat saat lari (Shift) dan berhenti saat diam atau
// melompat.
//
// CARA SETUP:
// 1. Klik 'Player', Add Component > PlayerFootsteps.
// 2. Di 'Footstep Clips', ubah Size sesuai jumlah file suara langkah
//    (disarankan 3-4 variasi), lalu drag file-file suara itu.
// 3. Atur 'Volume' dan 'Step Length' (jarak per langkah, makin kecil
//    makin rapat langkahnya).
// 4. Kalau suara langkah tidak keluar padahal Player jalan, naikkan
//    'Ground Check Distance' sedikit (default 1.3, cocok untuk capsule
//    setinggi 2 meter dengan titik tengah Player di tengah capsule).

using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerFootsteps : MonoBehaviour
{
    public AudioClip[] footstepClips;

    [Range(0f, 1f)] public float volume = 0.6f;

    [Tooltip("Jarak (meter) yang ditempuh untuk satu langkah")]
    public float stepLength = 2f;

    [Tooltip("Kecepatan minimum supaya dianggap sedang berjalan")]
    public float minSpeed = 0.5f;

    [Tooltip("Jarak sinar ke bawah untuk mengecek Player sedang menapak tanah")]
    public float groundCheckDistance = 1.3f;

    [Tooltip("Variasi nada acak supaya tidak terdengar monoton")]
    public Vector2 pitchRange = new Vector2(0.92f, 1.08f);

    private Rigidbody rb;
    private AudioSource source;
    private float traveled;
    private int lastClipIndex = -1;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        source = gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 0f;
    }

    private void Update()
    {
        if (footstepClips == null || footstepClips.Length == 0) return;

        Vector3 horizontalVelocity = rb.linearVelocity;
        horizontalVelocity.y = 0f;
        float speed = horizontalVelocity.magnitude;

        bool grounded = Physics.Raycast(
            transform.position, Vector3.down, groundCheckDistance,
            ~0, QueryTriggerInteraction.Ignore);

        if (GameState.IsUIOpen || !grounded || speed < minSpeed)
        {
            // Saat diam, siapkan supaya langkah pertama berbunyi cepat
            // begitu mulai berjalan lagi.
            traveled = stepLength * 0.7f;
            return;
        }

        traveled += speed * Time.deltaTime;

        if (traveled >= stepLength)
        {
            traveled -= stepLength;
            PlayStep();
        }
    }

    private void PlayStep()
    {
        int index = Random.Range(0, footstepClips.Length);

        // Hindari memutar klip yang sama dua kali berturut-turut
        if (footstepClips.Length > 1 && index == lastClipIndex)
        {
            index = (index + 1) % footstepClips.Length;
        }
        lastClipIndex = index;

        AudioClip clip = footstepClips[index];
        if (clip == null) return;

        source.pitch = Random.Range(pitchRange.x, pitchRange.y);
        source.PlayOneShot(clip, volume);
    }
}

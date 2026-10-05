// BatchAddCeilingLights.cs
// Taruh di folder: Assets/Editor/
//
// FUNGSI: Cari semua GameObject yang namanya mengandung kata kunci
// tertentu (misal 'ceiling_light'), lalu otomatis tambahkan komponen
// Point Light ke masing-masing (kalau belum ada), dengan intensitas
// dan warna yang bisa diatur sekaligus untuk semuanya -- cocok untuk
// bikin suasana redup/horor di ruangan server tanpa setting manual
// satu-satu.
//
// CARA PAKAI:
// 1. Buka menu Unity: Tools > Batch Add Ceiling Lights
// 2. Isi 'Name Contains' dengan kata kunci nama object, misal 'ceiling_light'
// 3. Atur Intensity, Range, Color sesuai selera (mulai redup dulu,
//    misal Intensity 0.5, Range 3)
// 4. Klik 'Add / Update Lights on Matching Objects'
// 5. Ulangi kapan saja untuk menyesuaikan intensitas semua lampu itu
//    sekaligus (script ini update lampu yang SUDAH ada, bukan cuma nambah).

using UnityEditor;
using UnityEngine;

public class BatchAddCeilingLights : EditorWindow
{
    private string nameContains = "ceiling_light";
    private float intensity = 0.6f;
    private float range = 3f;
    private Color lightColor = new Color(0.85f, 0.9f, 1f); // putih kebiruan, kesan dingin/horor
    private LightType lightType = LightType.Point;
    private Vector3 localOffset = new Vector3(0f, -0.2f, 0f); // sedikit di bawah mesh lampu
    private bool onlyInsideParent = true;
    private Transform searchParent;

    [MenuItem("Tools/Batch Add Ceiling Lights")]
    public static void ShowWindow()
    {
        GetWindow<BatchAddCeilingLights>("Batch Ceiling Lights");
    }

    private void OnGUI()
    {
        GUILayout.Label("Batch Add / Update Ceiling Lights", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "Mencari semua GameObject yang namanya mengandung kata kunci, " +
            "lalu menambahkan/update komponen Light (Point Light) sebagai " +
            "CHILD dari object itu (supaya mesh lampu tidak perlu diubah).",
            MessageType.Info);

        nameContains = EditorGUILayout.TextField("Name Contains", nameContains);

        onlyInsideParent = EditorGUILayout.Toggle("Batasi ke Parent tertentu", onlyInsideParent);
        if (onlyInsideParent)
        {
            searchParent = (Transform)EditorGUILayout.ObjectField(
                "Search Parent (misal GeneratedLayout)", searchParent, typeof(Transform), true);
        }

        EditorGUILayout.Space();
        lightType = (LightType)EditorGUILayout.EnumPopup("Light Type", lightType);
        intensity = EditorGUILayout.FloatField("Intensity", intensity);
        range = EditorGUILayout.FloatField("Range", range);
        lightColor = EditorGUILayout.ColorField("Color", lightColor);
        localOffset = EditorGUILayout.Vector3Field("Local Offset dari mesh", localOffset);

        EditorGUILayout.Space();
        if (GUILayout.Button("Add / Update Lights on Matching Objects", GUILayout.Height(30)))
        {
            RunBatch();
        }
    }

    private void RunBatch()
    {
        Transform[] allTransforms = onlyInsideParent && searchParent != null
            ? searchParent.GetComponentsInChildren<Transform>(true)
            : GameObject.FindObjectsByType<Transform>(FindObjectsSortMode.None);

        int added = 0, updated = 0;

        foreach (Transform t in allTransforms)
        {
            if (t == null) continue;
            if (!t.name.ToLower().Contains(nameContains.ToLower())) continue;

            // Cari child "PointLight" yang mungkin sudah pernah dibuat sebelumnya
            Transform existingLightChild = t.Find("PointLight");
            Light lightComp;

            if (existingLightChild != null)
            {
                lightComp = existingLightChild.GetComponent<Light>();
                if (lightComp == null)
                {
                    lightComp = existingLightChild.gameObject.AddComponent<Light>();
                }
                updated++;
            }
            else
            {
                GameObject lightObj = new GameObject("PointLight");
                lightObj.transform.SetParent(t, false);
                lightObj.transform.localPosition = localOffset;
                lightComp = lightObj.AddComponent<Light>();
                added++;
            }

            lightComp.type = lightType;
            lightComp.intensity = intensity;
            lightComp.range = range;
            lightComp.color = lightColor;
        }

        Debug.Log($"Batch Ceiling Lights selesai. Baru ditambah: {added}, di-update: {updated}.");
    }
}

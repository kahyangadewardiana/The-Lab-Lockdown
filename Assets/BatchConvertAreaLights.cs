// BatchConvertAreaLights.cs
// Taruh di folder: Assets/Editor/
//
// FUNGSI: Cari semua komponen Light bertipe 'Area' (biasanya hasil import
// dari Blender, tidak didukung realtime di URP) di dalam sebuah parent,
// lalu ubah jadi 'Point' (atau 'Spot') dengan intensity/range yang bisa
// diatur sekaligus untuk semuanya. Posisi lampu asli tetap dipertahankan.
//
// CARA PAKAI:
// 1. Buka menu Unity: Tools > Batch Convert Area Lights
// 2. Drag parent koridor (misal 'liminal space') ke field 'Search Parent'
// 3. Atur Intensity & Range sesuai selera (mulai redup dulu)
// 4. Klik 'Convert & Update Matching Lights'
// 5. Ulangi kapan saja untuk sesuaikan intensitas semua lampu itu sekaligus.

using UnityEditor;
using UnityEngine;

public class BatchConvertAreaLights : EditorWindow
{
    private Transform searchParent;
    private LightType fromType = LightType.Rectangle; // 'Area' lights di Unity muncul sbg Rectangle/Disc
    private LightType toType = LightType.Point;
    private float intensity = 0.6f;
    private float range = 4f;
    private Color lightColor = new Color(0.85f, 0.9f, 1f);
    private bool alsoUpdateAlreadyConvertedPoints = true;

    [MenuItem("Tools/Batch Convert Area Lights")]
    public static void ShowWindow()
    {
        GetWindow<BatchConvertAreaLights>("Batch Convert Area Lights");
    }

    private void OnGUI()
    {
        GUILayout.Label("Batch Convert Area Lights -> Point/Spot", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "Area Light tidak didukung realtime di URP. Tool ini otomatis " +
            "ubah semua Area Light di dalam parent tertentu jadi Point/Spot " +
            "Light, sambil mengatur intensitas semuanya sekaligus.",
            MessageType.Info);

        searchParent = (Transform)EditorGUILayout.ObjectField(
            "Search Parent (misal 'liminal space')", searchParent, typeof(Transform), true);

        EditorGUILayout.Space();
        fromType = (LightType)EditorGUILayout.EnumPopup("Convert FROM type", fromType);
        toType = (LightType)EditorGUILayout.EnumPopup("Convert TO type", toType);

        intensity = EditorGUILayout.FloatField("Intensity", intensity);
        range = EditorGUILayout.FloatField("Range", range);
        lightColor = EditorGUILayout.ColorField("Color", lightColor);

        alsoUpdateAlreadyConvertedPoints = EditorGUILayout.Toggle(
            "Update juga yang sudah ber-tipe 'TO' sebelumnya", alsoUpdateAlreadyConvertedPoints);

        EditorGUILayout.Space();
        GUI.enabled = searchParent != null;
        if (GUILayout.Button("Convert & Update Matching Lights", GUILayout.Height(30)))
        {
            RunBatch();
        }
        GUI.enabled = true;
    }

    private void RunBatch()
    {
        Light[] allLights = searchParent.GetComponentsInChildren<Light>(true);

        int converted = 0, updated = 0, skipped = 0;

        foreach (Light light in allLights)
        {
            bool matchesFrom = light.type == fromType;
            bool matchesToAlready = light.type == toType;

            if (matchesFrom)
            {
                light.type = toType;
                light.intensity = intensity;
                light.range = range;
                light.color = lightColor;
                converted++;
            }
            else if (matchesToAlready && alsoUpdateAlreadyConvertedPoints)
            {
                light.intensity = intensity;
                light.range = range;
                light.color = lightColor;
                updated++;
            }
            else
            {
                skipped++;
            }
        }

        Debug.Log($"Batch Convert Area Lights selesai. Dikonversi: {converted}, " +
                  $"di-update (sudah {toType} sebelumnya): {updated}, dilewati: {skipped}.");
    }
}

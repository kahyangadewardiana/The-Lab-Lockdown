// SceneLayoutBuilder.cs
// Taruh file ini di dalam folder: Assets/Editor/
// (buat folder "Editor" jika belum ada — wajib bernama "Editor" agar Unity
//  mengenalinya sebagai editor script dan tidak ikut di-build ke game)
//
// CARA PAKAI:
// 1. Taruh file ini di Assets/Editor/SceneLayoutBuilder.cs
// 2. Taruh SceneLayout.json (yang saya kirimkan terpisah) di:
//    Assets/Resources/SceneLayout.json
//    (folder "Resources" wajib ada, boleh dibuat di mana saja di dalam Assets)
// 3. Buka menu Unity: Tools > Scene Layout Builder
// 4. Di window yang muncul, isi field "Prefab Mappings" — pasangkan setiap
//    "Type Name" (misal: floor, server, ceiling_light, wall, desk, dst.)
//    dengan Prefab milik Anda sendiri (drag & drop dari Project window)
// 5. Klik tombol "Build Scene Layout"
// 6. Semua objek akan muncul di scene aktif, dikelompokkan di bawah
//    GameObject bernama "GeneratedLayout", dengan posisi & rotasi yang
//    sama persis seperti scene Demo.unity aslinya, tapi menggunakan
//    prefab Anda sendiri.
//
// Catatan: Type Name yang tidak Anda isi prefab-nya akan dilewati (skip)
// dan dicatat di Console sebagai peringatan, jadi Anda bisa cek objek
// mana saja yang belum ter-cover.

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class SceneLayoutBuilder : EditorWindow
{
    [Serializable]
    public class LayoutObject
    {
        public string name;
        public List<double> pos; // x, y, z
        public List<double> rot; // euler x, y, z
        public string guid;
    }

    [Serializable]
    public class LayoutObjectList
    {
        public List<LayoutObject> items;
    }

    [Serializable]
    public class PrefabMapping
    {
        public string typeName;
        public GameObject prefab;
    }

    private List<PrefabMapping> mappings = new List<PrefabMapping>();
    private Vector2 scrollPos;
    private string jsonResourceName = "SceneLayout";
    private string parentName = "GeneratedLayout";

    [MenuItem("Tools/Scene Layout Builder")]
    public static void ShowWindow()
    {
        GetWindow<SceneLayoutBuilder>("Scene Layout Builder");
    }

    private void OnEnable()
    {
        if (mappings.Count == 0)
        {
            AutoDetectTypes();
        }
    }

    private static string StripIndexSuffix(string n)
    {
        // "ceiling_light (5)" -> "ceiling_light"
        int idx = n.LastIndexOf(" (");
        if (idx > 0 && n.EndsWith(")"))
            return n.Substring(0, idx).Trim();
        return n.Trim();
    }

    private void AutoDetectTypes()
    {
        var data = LoadLayoutData();
        if (data == null) return;

        var types = data.items
            .Select(o => StripIndexSuffix(o.name))
            .Distinct()
            .OrderBy(t => t);

        mappings = types.Select(t => new PrefabMapping { typeName = t, prefab = null }).ToList();
    }

    private LayoutObjectList LoadLayoutData()
    {
        TextAsset json = Resources.Load<TextAsset>(jsonResourceName);
        if (json == null)
        {
            Debug.LogError($"Tidak menemukan Resources/{jsonResourceName}.json. " +
                            "Pastikan SceneLayout.json ada di dalam folder Assets/Resources/.");
            return null;
        }
        // Wrap array JSON into an object for JsonUtility (which needs a root object)
        string wrapped = "{\"items\":" + json.text + "}";
        return JsonUtility.FromJson<LayoutObjectList>(wrapped);
    }

    private void OnGUI()
    {
        GUILayout.Label("Scene Layout Builder", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "Pasangkan setiap tipe objek dengan prefab milik Anda sendiri, " +
            "lalu klik 'Build Scene Layout'.", MessageType.Info);

        jsonResourceName = EditorGUILayout.TextField("JSON Resource Name", jsonResourceName);
        parentName = EditorGUILayout.TextField("Parent Object Name", parentName);

        if (GUILayout.Button("Reload Type List from JSON"))
        {
            AutoDetectTypes();
        }

        EditorGUILayout.Space();
        GUILayout.Label($"Prefab Mappings ({mappings.Count} types)", EditorStyles.boldLabel);

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(400));
        foreach (var m in mappings)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(m.typeName, GUILayout.Width(160));
            m.prefab = (GameObject)EditorGUILayout.ObjectField(m.prefab, typeof(GameObject), false);
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();

        EditorGUILayout.Space();
        if (GUILayout.Button("Build Scene Layout", GUILayout.Height(30)))
        {
            BuildLayout();
        }
    }

    private void BuildLayout()
    {
        var data = LoadLayoutData();
        if (data == null) return;

        var lookup = mappings
            .Where(m => m.prefab != null)
            .ToDictionary(m => m.typeName, m => m.prefab);

        GameObject parent = GameObject.Find(parentName);
        if (parent == null)
            parent = new GameObject(parentName);

        int placed = 0, skipped = 0;

        foreach (var o in data.items)
        {
            string type = StripIndexSuffix(o.name);
            if (!lookup.TryGetValue(type, out GameObject prefab))
            {
                Debug.LogWarning($"Skip '{o.name}': tidak ada prefab untuk tipe '{type}'.");
                skipped++;
                continue;
            }

            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            instance.name = o.name;
            instance.transform.SetParent(parent.transform, false);
            instance.transform.localPosition = new Vector3(
                (float)o.pos[0], (float)o.pos[1], (float)o.pos[2]);
            instance.transform.localEulerAngles = new Vector3(
                (float)o.rot[0], (float)o.rot[1], (float)o.rot[2]);

            placed++;
        }

        Debug.Log($"Scene Layout Builder selesai. Ditempatkan: {placed}, dilewati: {skipped}.");
        Selection.activeGameObject = parent;
    }
}

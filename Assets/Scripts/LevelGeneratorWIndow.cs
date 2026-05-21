using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class LevelGeneratorWindow : EditorWindow
{
    // =========================
    // EDITOR MODEL
    // =========================

    [System.Serializable]
    public class Wave
    {
        public bool foldout = true;
        public List<Event> events = new();
    }
    
    private void OnEnable()
    {
        waves = new List<Wave>
        {
            // Wave 0 - Intro
            new Wave { events = new List<Event> {
                new Event { type = EventType.SpawnEnemies, enemyType = EnemyType.Basic, count = 5, delay = 0.8f },
            }},

            // Wave 1 - Picking up
            new Wave { events = new List<Event> {
                new Event { type = EventType.SpawnEnemies, enemyType = EnemyType.Basic, count = 8, delay = 0.7f },
            }},

            // Wave 2 - First split
            new Wave { events = new List<Event> {
                new Event { type = EventType.SpawnEnemies, enemyType = EnemyType.Basic, count = 6, delay = 0.6f },
                new Event { type = EventType.Wait, duration = 3f },
                new Event { type = EventType.SpawnEnemies, enemyType = EnemyType.Basic, count = 6, delay = 0.6f },
            }},

            // Wave 3 - Sustained
            new Wave { events = new List<Event> {
                new Event { type = EventType.SpawnEnemies, enemyType = EnemyType.Basic, count = 12, delay = 0.6f },
            }},

            // Wave 4 - Double push
            new Wave { events = new List<Event> {
                new Event { type = EventType.SpawnEnemies, enemyType = EnemyType.Basic, count = 8, delay = 0.5f },
                new Event { type = EventType.Wait, duration = 4f },
                new Event { type = EventType.SpawnEnemies, enemyType = EnemyType.Basic, count = 8, delay = 0.5f },
            }},

            // Wave 5 - Triple burst
            new Wave { events = new List<Event> {
                new Event { type = EventType.SpawnEnemies, enemyType = EnemyType.Basic, count = 5, delay = 0.5f },
                new Event { type = EventType.Wait, duration = 2f },
                new Event { type = EventType.SpawnEnemies, enemyType = EnemyType.Basic, count = 5, delay = 0.5f },
                new Event { type = EventType.Wait, duration = 2f },
                new Event { type = EventType.SpawnEnemies, enemyType = EnemyType.Basic, count = 5, delay = 0.5f },
            }},

            // Wave 6 - Big flood
            new Wave { events = new List<Event> {
                new Event { type = EventType.SpawnEnemies, enemyType = EnemyType.Basic, count = 20, delay = 0.5f },
            }},

            // Wave 7 - Relentless
            new Wave { events = new List<Event> {
                new Event { type = EventType.SpawnEnemies, enemyType = EnemyType.Basic, count = 10, delay = 0.5f },
                new Event { type = EventType.Wait, duration = 3f },
                new Event { type = EventType.SpawnEnemies, enemyType = EnemyType.Basic, count = 10, delay = 0.5f },
                new Event { type = EventType.Wait, duration = 3f },
                new Event { type = EventType.SpawnEnemies, enemyType = EnemyType.Basic, count = 5, delay = 0.5f },
            }},

            // Wave 8 - Overwhelming
            new Wave { events = new List<Event> {
                new Event { type = EventType.SpawnEnemies, enemyType = EnemyType.Basic, count = 30, delay = 0.5f },
            }},

            // Wave 9 - Final 50
            new Wave { events = new List<Event> {
                new Event { type = EventType.SpawnEnemies, enemyType = EnemyType.Basic, count = 20, delay = 0.5f },
                new Event { type = EventType.Wait, duration = 2f },
                new Event { type = EventType.SpawnEnemies, enemyType = EnemyType.Basic, count = 15, delay = 0.5f },
                new Event { type = EventType.Wait, duration = 1f },
                new Event { type = EventType.SpawnEnemies, enemyType = EnemyType.Basic, count = 15, delay = 0.5f },
            }},
        };
    }

    public enum EventType
    {
        SpawnEnemies,
        Wait
    }

    [System.Serializable]
    public class Event
    {
        public EventType type;

        public EnemyType enemyType;
        public int count;
        public float delay;

        public float duration;
    }

    private List<Wave> waves = new();
    private string levelName = "Level_01";

    private Vector2 scroll;

    [MenuItem("Tools/TD Level Generator")]
    public static void Open()
    {
        GetWindow<LevelGeneratorWindow>("TD Level Generator");
    }

    private void OnGUI()
    {
        EditorGUILayout.Space();

        levelName = EditorGUILayout.TextField("Level Name", levelName);

        EditorGUILayout.Space();

        scroll = EditorGUILayout.BeginScrollView(scroll);

        DrawWaves();

        EditorGUILayout.EndScrollView();

        EditorGUILayout.Space();

        if (GUILayout.Button("GENERATE LEVEL", GUILayout.Height(40)))
        {
            GenerateLevel();
        }
    }

    // =========================
    // UI
    // =========================
    private void DrawWaves()
    {
        for (int i = 0; i < waves.Count; i++)
        {
            var wave = waves[i];

            EditorGUILayout.BeginVertical("box");

            // 🔽 Foldout
            wave.foldout = EditorGUILayout.Foldout(
                wave.foldout,
                $"Wave {i:00}",
                true,
                EditorStyles.foldoutHeader
            );

            if (wave.foldout)
            {
                EditorGUILayout.Space(5);

                for (int j = 0; j < wave.events.Count; j++)
                {
                    var evt = wave.events[j];

                    EditorGUILayout.BeginVertical("box");

                    evt.type = (EventType)EditorGUILayout.EnumPopup("Type", evt.type);

                    switch (evt.type)
                    {
                        case EventType.SpawnEnemies:
                            evt.enemyType = (EnemyType)EditorGUILayout.EnumPopup("Enemy Type", evt.enemyType);
                            evt.count = EditorGUILayout.IntField("Count", evt.count);
                            evt.delay = EditorGUILayout.FloatField("Delay", evt.delay);
                            break;

                        case EventType.Wait:
                            evt.duration = EditorGUILayout.FloatField("Duration", evt.duration);
                            break;
                    }

                    if (GUILayout.Button("Remove Event"))
                    {
                        wave.events.RemoveAt(j);
                        break;
                    }

                    EditorGUILayout.EndVertical();
                }

                EditorGUILayout.Space(5);

                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("Add Spawn Event"))
                {
                    wave.events.Add(new Event { type = EventType.SpawnEnemies });
                }

                if (GUILayout.Button("Add Wait Event"))
                {
                    wave.events.Add(new Event { type = EventType.Wait });
                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space(5);

                if (GUILayout.Button("Remove Wave"))
                {
                    waves.RemoveAt(i);
                    break;
                }
            }

            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Add Wave"))
            waves.Add(new Wave());
    }

    // =========================
    // GENERATION
    // =========================
    private void GenerateLevel()
    {
        if (string.IsNullOrWhiteSpace(levelName))
        {
            Debug.LogError("Level name is empty!");
            return;
        }

        string rootPath = $"Assets/Levels/{levelName}";

        // overwrite
        if (AssetDatabase.IsValidFolder(rootPath))
        {
            bool overwrite = EditorUtility.DisplayDialog(
                "Overwrite Level?",
                $"Level '{levelName}' already exists. Overwrite?",
                "Yes",
                "No"
            );

            if (!overwrite)
                return;

            FileUtil.DeleteFileOrDirectory(rootPath);
            FileUtil.DeleteFileOrDirectory(rootPath + ".meta");
            AssetDatabase.Refresh();
        }

        // folders
        CreateFolderIfNotExists("Assets", "Levels");
        AssetDatabase.CreateFolder("Assets/Levels", levelName);

        string wavesPath = $"{rootPath}/Waves";
        string eventsPath = $"{rootPath}/Events";

        AssetDatabase.CreateFolder(rootPath, "Waves");
        AssetDatabase.CreateFolder(rootPath, "Events");

        // LEVEL
        var levelSO = ScriptableObject.CreateInstance<LevelSO>();
        levelSO.waves = new List<WaveSO>();

        string levelPath = $"{rootPath}/{levelName}.asset";
        AssetDatabase.CreateAsset(levelSO, levelPath);

        Debug.Log($"Created Level: {levelPath}");

        // WAVES
        for (int i = 0; i < waves.Count; i++)
        {
            var waveSO = ScriptableObject.CreateInstance<WaveSO>();
            waveSO.events = new List<SpawnEventSO>();

            string waveName = $"Wave_{i:00}";
            string wavePath = $"{wavesPath}/{waveName}.asset";

            AssetDatabase.CreateAsset(waveSO, wavePath);
            levelSO.waves.Add(waveSO);

            string waveEventsPath = $"{eventsPath}/{waveName}";
            AssetDatabase.CreateFolder(eventsPath, waveName);

            int eventIndex = 0;

            CreateAndAddEvent<StartEventSO>(waveSO, waveEventsPath, eventIndex++, "Start");

            for (int j = 0; j < waves[i].events.Count; j++)
            {
                var evtData = waves[i].events[j];
                var evtSO = CreateEventSO(evtData);

                string evtName = $"Event_{eventIndex:00}_{evtData.type}";
                string evtPath = $"{waveEventsPath}/{evtName}.asset";

                AssetDatabase.CreateAsset(evtSO, evtPath);
                waveSO.events.Add(evtSO);

                eventIndex++;
            }

            CreateAndAddEvent<EndEventSO>(waveSO, waveEventsPath, eventIndex++, "End");

            EditorUtility.SetDirty(waveSO);
        }

        EditorUtility.SetDirty(levelSO);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("✅ LEVEL GENERATED");
    }

    // =========================
    // HELPERS
    // =========================
    private void CreateAndAddEvent<T>(WaveSO wave, string path, int index, string label)
        where T : SpawnEventSO
    {
        var evt = ScriptableObject.CreateInstance<T>();

        string evtName = $"Event_{index:00}_{label}";
        string evtPath = $"{path}/{evtName}.asset";

        AssetDatabase.CreateAsset(evt, evtPath);
        EditorUtility.SetDirty(evt); // 👈 add this
        wave.events.Add(evt);
    }

    private SpawnEventSO CreateEventSO(Event data)
    {
        switch (data.type)
        {
            case EventType.SpawnEnemies:
                var spawn = ScriptableObject.CreateInstance<SpawnEnemiesEventSO>();
                spawn.enemyType = data.enemyType;
                spawn.count = data.count;
                spawn.delayBetween = data.delay;
                return spawn;

            case EventType.Wait:
                var wait = ScriptableObject.CreateInstance<WaitEventSO>();
                wait.duration = data.duration;
                return wait;
        }

        return null;
    }

    private void CreateFolderIfNotExists(string parent, string name)
    {
        string path = $"{parent}/{name}";
        if (!AssetDatabase.IsValidFolder(path))
        {
            AssetDatabase.CreateFolder(parent, name);
        }
    }
}
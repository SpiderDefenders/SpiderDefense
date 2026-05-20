using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundLibrary", menuName = "Audio/Sound Library")]
public class SoundLibrarySO : ScriptableObject
{
    [SerializeField] private List<SoundEntry> sounds = new();

    private Dictionary<SoundID, SoundEntry> _lookup;

    public void Init()
    {
        _lookup = sounds.ToDictionary(s => s.id, s => s);
    }

    public bool TryGet(SoundID id, out SoundEntry entry) =>
        _lookup.TryGetValue(id, out entry);
}
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace GhostNirvana {
    [CreateAssetMenu(fileName = "BuffList",
                 menuName = "~/Stats/BuffList", order = 1)]
    public class BuffList : ScriptableObject {
        [SerializeField, ReorderableList] public List<Buff> All;
    }
}

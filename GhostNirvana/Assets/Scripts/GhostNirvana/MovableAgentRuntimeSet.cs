using UnityEngine;
using ScriptableBehaviour;

namespace GhostNirvana {
[CreateAssetMenu(fileName = "MoveableAgents",
                 menuName = "~/Sets/MovableAgent", order = 1)]
public class MovableAgentRuntimeSet : RuntimeSet<MovableAgent> { }
}

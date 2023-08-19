using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Control;

namespace GhostNirvana {

[RequireComponent(typeof(IDoll<StandardMovementInput>))]
public class AISeekPlayer : MonoBehaviour, IPossessor<StandardMovementInput> {

    IDoll<StandardMovementInput> _possessed;
    IDoll<StandardMovementInput> IPossessor<StandardMovementInput>.Possessed {
        get => _possessed;
        set => _possessed = value;
    }

    IPossessor<StandardMovementInput> Possessor => this;

    void Awake() {
        _possessed = GetComponent<IDoll<StandardMovementInput>>();
    }

    void Start() {
        Possessor.Possess(_possessed);
    }

    public StandardMovementInput GetCommand() {
        Vector3 desiredMovement = Vector3.zero;
        if (Miyu.Instance) desiredMovement = (Miyu.Instance.transform.position - transform.position).normalized;
        return new StandardMovementInput {
            desiredMovement = desiredMovement
        };
    }
}

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Control;

namespace GhostNirvana {

public class AISeekPlayer : MonoBehaviour, IPossessor<Ghosty.Input> {

    IDoll<Ghosty.Input> _possessed;
    IDoll<Ghosty.Input> IPossessor<Ghosty.Input>.Possessed {
        get => _possessed;
        set => _possessed = value;
    }

    IPossessor<Ghosty.Input> Possessor => this;

    void Awake() {
        _possessed = GetComponent<IDoll<Ghosty.Input>>();
    }

    void Start() {
        Possessor.Possess(_possessed);
    }

    public Ghosty.Input GetCommand() {
        return new Ghosty.Input {
            desiredMovement = (Miyu.Instance.transform.position - transform.position).normalized
        };
    }
}

}

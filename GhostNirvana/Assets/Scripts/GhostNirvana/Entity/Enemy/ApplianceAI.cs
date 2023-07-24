using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Control;

namespace GhostNirvana {

public class ApplianceAI : MonoBehaviour, IPossessor<Appliance.Input> {

    IDoll<Appliance.Input> _possessed;
    IDoll<Appliance.Input> IPossessor<Appliance.Input>.Possessed {
        get => _possessed;
        set => _possessed = value;
    }

    IPossessor<Appliance.Input> Possessor => this;

    void Awake() {
        _possessed = GetComponent<IDoll<Appliance.Input>>();
    }

    void Start() {
        Possessor.Possess(_possessed);
    }

    public Appliance.Input GetCommand() {
        return new Appliance.Input {
            desiredMovement = (Miyu.Instance.transform.position - transform.position).normalized
        };
    }
}

}

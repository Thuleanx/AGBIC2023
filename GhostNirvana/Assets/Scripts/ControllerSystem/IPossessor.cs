using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Control {

public interface IPossessor<Command> {
    public IDoll<Command> Possessed { get; protected set; }

    public void Possess(IDoll<Command> doll) {
        // unpossess current doll
        if (doll != null && doll.Possessor == this)
            doll.SetPossessed(null);
        
        if (doll.Possessor != null)
            Debug.LogWarning("Possessing an already possessed doll.");
        Possessed = doll;
        doll.SetPossessed(this);
    }

    public Command GetCommand();
}

}

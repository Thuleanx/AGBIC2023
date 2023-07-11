using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Control {

public interface IDoll<Command> {
    public IPossessor<Command> Possessor { get; protected set; }

    public Command GetCommand() => Possessor.GetCommand();
    public bool IsPossessed => Possessor != null;

    internal void SetPossessed(IPossessor<Command> possessor) => Possessor = possessor;
}

}

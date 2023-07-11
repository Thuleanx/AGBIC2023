using System.Collections;
using System;
using Base;

namespace AI {

public abstract class State<Agent, StateID>
        where Agent : Entity
        where StateID : struct, IComparable, IConvertible, IFormattable {

    public virtual void Begin(StateMachine<Agent, StateID> stateMachine, Agent agent) {}
    public virtual void End(StateMachine<Agent, StateID> stateMachine, Agent agent) {}

    public virtual StateID? Update(StateMachine<Agent, StateID> stateMachine, Agent agent) => null;
    public virtual StateID? FixUpdate(StateMachine<Agent, StateID> stateMachine, Agent agent) => null;
    public virtual bool CanEnter(StateMachine<Agent, StateID> stateMachine, Agent agent) => true;

    public virtual IEnumerator Coroutine(StateMachine<Agent, StateID> stateMachine, Agent agent) => null;
}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Base;
using NaughtyAttributes;

namespace AI {

public abstract class StateMachine<Agent, StateID> : MonoBehaviour 
        where Agent : Entity
        where StateID : struct, IComparable, IConvertible, IFormattable {

    Agent agent;
    Coroutine currentCoroutine;
    StateID defaultState;

    Dictionary<StateID, State<Agent, StateID>> States;
    public Hashtable Blackboard { get; private set; }

    bool startExecuted = false;

    [SerializeField, ReadOnly]
    StateID _currentState;
    public StateID State {
        get => _currentState;
        private set {
            if (!value.Equals(_currentState)) {
                if (currentCoroutine != null) StopCoroutine(currentCoroutine);
                States[_currentState]?.End(this, agent);
                _currentState = value;
                States[_currentState]?.Begin(this, agent);

                IEnumerator enumerator = States[_currentState]?.Coroutine(this, agent);
                if (enumerator != null) currentCoroutine = StartCoroutine(enumerator);
            }
        }
    }

    public virtual void ConstructMachine(Agent agent, StateID defaultState) {
        Blackboard = new Hashtable();
        States = new Dictionary<StateID, State<Agent, StateID>>();
        this.agent = agent;
        this.defaultState = defaultState;
        _currentState = defaultState;

        Construct();
    }

    public void AssignState<T>(StateID id) where T : State<Agent, StateID>, new()
        => States[id] = new T();

    public bool CanEnter(StateID id) 
        => States[id] != null && States[id].CanEnter(this, agent);

    public void SetState(StateID newState) => State = newState;

    public abstract void Construct();

    protected virtual void OnEnable() {
        if (startExecuted) Init();
    }

    protected virtual void Start() {
        Init();
        startExecuted = true;
    }

    public void Init() {
        _currentState = defaultState;
        States[_currentState].Begin(this, agent);
        IEnumerator enumerator = States[_currentState]?.Coroutine(this, agent);
        if (enumerator != null) currentCoroutine = StartCoroutine(enumerator);
    }

    public void RunUpdate() 
        => State = States[State]?.Update(this, agent) ?? State;

    public void RunFixUpdate()
        => State = States[State]?.FixUpdate(this, agent) ?? State;
}

}

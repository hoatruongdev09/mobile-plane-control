using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine {
    public State CurrentState { get; set; }
    public State LastState { get; set; }

    public void Start (State startState) {
        CurrentState = startState;
        CurrentState.Enter ();
    }
    public void Start (State startState, object options) {
        CurrentState = startState;
        CurrentState.Enter (options);
    }
    public void ChangeState (State nextState) {
        LastState = CurrentState;
        LastState.Exit ();
        CurrentState = nextState;
        CurrentState.Enter ();
    }
    public void ChangeState (State nextState, object options) {
        LastState = CurrentState;
        LastState.Exit ();
        CurrentState = nextState;
        CurrentState.Enter (options);
    }
    public void ChangeState (State nextState, object enterOptions, object exitOptions) {
        LastState = CurrentState;
        LastState.Exit (exitOptions);
        CurrentState = nextState;
        CurrentState.Enter (enterOptions);
    }
}
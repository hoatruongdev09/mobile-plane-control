using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TornadoState : State {
    protected Tornado Tornado { get; private set; }
    protected StateMachine StateMachine { get; private set; }
    public TornadoState (Tornado tornado, StateMachine stateMachine) {
        Tornado = tornado;
        StateMachine = stateMachine;
    }
    public virtual void Enter () {

    }

    public virtual void Enter (object options) {

    }

    public virtual void Exit () {

    }

    public virtual void Exit (object options) {

    }

    public virtual void Reset () {

    }

    public virtual void Update () {

    }
}
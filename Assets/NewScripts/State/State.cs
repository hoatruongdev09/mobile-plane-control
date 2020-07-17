using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface State {
    void Enter ();
    void Enter (object options);
    void Update ();
    void Exit ();
    void Exit (object options);
    void Reset ();
}
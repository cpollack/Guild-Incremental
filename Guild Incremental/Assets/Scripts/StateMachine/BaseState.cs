using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseState
{
    public BaseState(GameObject gameObject)
    {
        this.gameObject = gameObject;
    }

    protected GameObject gameObject;

    public abstract Type Tick();

    public virtual void OnBeforeStateChange() { }
    public virtual void OnStateChange() { }

    public virtual void Save() { }
    public virtual void Load() { }

    public virtual int GetSubState()
    {
        return 0;
    }
}

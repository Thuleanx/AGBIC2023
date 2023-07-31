using UnityEngine;
using System;
using System.Collections;

namespace Base {

public abstract class Entity : MonoBehaviour {
    bool disposing;

    protected virtual void OnEnable() {
        disposing = false;
    }

    public Coroutine Dispose() {
        if (disposing) return null;
        return StartCoroutine(IDispose());
    }

    protected abstract IEnumerator IDispose();
}

}

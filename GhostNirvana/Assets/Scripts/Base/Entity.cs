using UnityEngine;
using System;
using System.Collections;

namespace Base {

public abstract class Entity : MonoBehaviour {
    public Coroutine Dispose() => StartCoroutine(IDispose());
    protected abstract IEnumerator IDispose();
}

}

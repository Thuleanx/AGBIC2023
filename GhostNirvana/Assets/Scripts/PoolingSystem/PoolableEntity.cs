using System.Collections;
using Base;

namespace Optimization {

public class PoolableEntity : Entity {
    protected override IEnumerator IDispose() {
        ObjectPoolManager.Bubble bubble = GetComponent<ObjectPoolManager.Bubble>();

        if (bubble) {
            bubble.RequestDisposal();
        } else gameObject.SetActive(false);

        yield return null;
    }
}

}

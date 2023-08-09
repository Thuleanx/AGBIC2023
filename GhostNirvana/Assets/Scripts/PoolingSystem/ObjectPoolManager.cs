using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Base;

namespace Optimization {

public partial class ObjectPoolManager : MonoBehaviour {
    public class Bubble : MonoBehaviour {
        public bool InQueue {get; private set; } = true;
        public UnityEvent<Bubble> DisposalEvent = new UnityEvent<Bubble>();

        public void OnSpawn() => InQueue = false;

        public void RequestDisposal() {
            if (InQueue) return;
            DisposalEvent?.Invoke(this);
            InQueue = true;
        }
    }

    public class Pool<T> where T : Component {
        public T Prefab;
        public Queue<T> content;
        public int NumBubbles = 0;

        public void Expand(int amount) {
            NumBubbles += amount;
            while (amount-- > 0) {
                bool prefabIsActive = Prefab.gameObject.activeSelf;

                // Important: this prevents any OnEnables from running
                Prefab.gameObject.SetActive(false);

                GameObject obj = Instantiate(Prefab.gameObject);
                Prefab.gameObject.SetActive(prefabIsActive);

                // Grant immortality
                DontDestroyOnLoad(obj);

                Bubble bubble = obj.GetComponent<Bubble>();
                if (!bubble) bubble = obj.AddComponent<Bubble>();
                bubble.DisposalEvent.AddListener(Collect);

                content.Enqueue( obj.GetComponent<T>() );
            }
        }

        public void Collect(Bubble bubble) {
            Debug.Log("Collecting: " + bubble.gameObject);
            // give up collecting the bubble, someone illegally borrowed it
            if (bubble.transform.parent != null) {
                Destroy(bubble.gameObject);
                return;
            }
            content.Enqueue(bubble.GetComponent<T>());
            bubble.gameObject.SetActive(false);
        }
    }
}

[DisallowMultipleComponent]
public partial class ObjectPoolManager : MonoBehaviour {
    public static ObjectPoolManager Instance;

    const int BaseExpansionRate = 5;
    Hashtable pools;
    Dictionary<Scene, HashSet<Bubble>> sceneToBubbleMapping;

    void Awake() {
        if (Instance) Debug.LogError("Multiple Instances of Object Pool Manager!!!");
        Instance = this;

        pools = new Hashtable();
        sceneToBubbleMapping = new Dictionary<Scene, HashSet<Bubble>>();
    }

    void Start() {
        App.BeforeSceneUnload.AddListener(OnSceneUnloaded);
    }

    void OnDestroy() {
        App.BeforeSceneUnload.RemoveListener(OnSceneUnloaded);
    }

    public T Borrow<T>(Scene scene, T Prefab,
        Vector3? position = null, Quaternion? rotation = null) where T : Component {

        if (!Prefab) return null;

        Entity prefabEntity = Prefab.GetComponent<Entity>() ?? Prefab.GetComponentInParent<Entity>();

        if (!(prefabEntity is PoolableEntity))
            Debug.LogError("Entity spawned with the object pool system must be a PoolableEntity.");

        int prefabID = Prefab.GetInstanceID();

        Pool<T> pool = null;
        if (!pools.ContainsKey(prefabID)) {
            pool = new Pool<T>{
                Prefab = Prefab,
                content = new Queue<T>()
            };
            pools.Add(prefabID, pool);
        } else {
            pool = pools[prefabID] as Pool<T>;
        }

        if (pool.content.Count == 0) 
            pool.Expand(Mathf.Max(pool.NumBubbles, BaseExpansionRate));

        T instantiatedObject = pool.content.Dequeue();

        instantiatedObject.gameObject.transform.SetPositionAndRotation(
            position ?? Vector3.zero,
            rotation ?? Quaternion.identity
        );
        instantiatedObject.gameObject.SetActive(true);

        Bubble bubble = instantiatedObject.GetComponent<Bubble>();
        if (!bubble) Debug.LogError("Bubble not found on pooled object. Maybe a script has destroyed it");
        bubble.OnSpawn();

        if (!sceneToBubbleMapping.ContainsKey(scene))
            sceneToBubbleMapping[scene] = new HashSet<Bubble>();

        sceneToBubbleMapping[scene].Add( bubble );

        Debug.Log("Borrowed " + instantiatedObject + " to scene: " + scene.name);

        return instantiatedObject;
    }

    void OnSceneUnloaded(Scene scene) {
        if (!sceneToBubbleMapping.ContainsKey(scene)) return;

        foreach (Bubble bubble in sceneToBubbleMapping[scene])
            bubble.RequestDisposal();

        sceneToBubbleMapping.Remove(scene);
    }
}

}

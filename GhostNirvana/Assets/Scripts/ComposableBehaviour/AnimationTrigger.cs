using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ComposableBehaviour {

public class AnimationTrigger : MonoBehaviour {
    [SerializeField] List<UnityEvent> animEvents;

    public void AnimationOnly_Trigger(int eventID) {
        if (animEvents == null) return;
        if (eventID >= animEvents.Count) return;
        animEvents[eventID]?.Invoke();
    }
}

}

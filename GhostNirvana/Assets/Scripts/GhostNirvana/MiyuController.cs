using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using NaughtyAttributes;
using Control;

namespace GhostNirvana {

public class MiyuController : MonoBehaviour, IPossessor<Miyu.ControllerInput> {

    public IDoll<Miyu.ControllerInput> _possessed;
    IDoll<Miyu.ControllerInput> IPossessor<Miyu.ControllerInput>.Possessed {
        get => _possessed;
        set => _possessed = value;
    }

    public Miyu.ControllerInput GetCommand() {
        return new Miyu.ControllerInput {
            desiredMovement = MovementToWorldDir(Movement)
        };
    }

    [SerializeField] Miyu miyu;

    IPossessor<Miyu.ControllerInput> MiyuPossessor => this;

    [SerializeField, ReadOnly]
    Vector2 Movement;

    public void OnMovement(InputAction.CallbackContext ctx) => Movement = ctx.ReadValue<Vector2>();

    void Awake() {
        MiyuPossessor.Possess(miyu);
    }

    Vector3 MovementToWorldDir(Vector2 movement) {
        Vector3 inputDir = new Vector3(
            movement.x, 0, movement.y
        ).normalized;

        return Quaternion.Euler(
            0, Camera.main.transform.eulerAngles.y, 0f
        ) * inputDir;
    }
}

}

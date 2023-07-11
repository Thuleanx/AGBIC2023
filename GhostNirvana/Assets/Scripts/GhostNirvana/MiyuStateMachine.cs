using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI;
using Thuleanx.Utils;

namespace GhostNirvana {

    [RequireComponent(typeof(Miyu))]
    public class MiyuStateMachine : StateMachine<Miyu, Miyu.States> {

        public Miyu Miyu { get; private set; }

        void Awake() {
            Miyu = GetComponent<Miyu>();

            ConstructMachine(agent: Miyu, defaultState: Miyu.States.Grounded);
        }

        public override void Construct() {
            AssignState<Miyu.MiyuGrounded>(Miyu.States.Grounded);
        }
    }

public partial class Miyu {
    public class MiyuGrounded : State<Miyu, Miyu.States> {
        public override Miyu.States? Update(StateMachine<Miyu, Miyu.States> stateMachine, Miyu miyu) {
            Vector3 desiredVelocity = miyu.Input.desiredMovement * miyu.movementSpeed;

            miyu.Velocity = Mathx.Damp(Vector3.Lerp, miyu.Velocity, desiredVelocity, 
                (miyu.Velocity.sqrMagnitude > desiredVelocity.sqrMagnitude) ? miyu.deccelerationAlpha : miyu.accelerationAlpha, Time.deltaTime);

            if (miyu.Velocity != Vector3.zero) 
                miyu.TurnToFace(miyu.Velocity, miyu.turnSpeed);

            

            return null;
        }

    }
}

}

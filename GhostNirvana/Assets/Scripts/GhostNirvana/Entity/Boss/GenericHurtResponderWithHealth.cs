using Base;
using UnityEngine;
using UnityEngine.Events;
using CombatSystem;
using Optimization;

namespace GhostNirvana {

public class GenericHurtResponderWithHealth : PoolableEntity, IHurtable, IHurtResponder {
    public Status Status { get; private set; }

    [SerializeField] UnityEvent<IHurtable, int, DamageType, Hit> _OnDamage;
    UnityEvent<IHurtable, int, DamageType, Hit> IHurtable.OnBeforeDamage => _OnDamage;

    public Entity Owner => this;

    public void RespondToHurt(Hit hit) { }

    public bool ValidateHit(Hit hit) => true;

    void IHurtable.OnTakeDamage(int damageAmount, DamageType damageType, Hit hit)
        => Status.TakeDamage(damageAmount);

    protected void Awake() {
        Status = GetComponent<Status>();
        Status.Owner = this;
    }

    protected override void OnEnable() {
        base.OnEnable();
        IHurtResponder.ConnectChildrenHurtboxes(this);
    }

    protected void OnDisable() => IHurtResponder.DisconnectChildrenHurtboxes(this);
}

}

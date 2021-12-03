using UnityEngine;

public interface IInteractible
{
    public abstract void Target(bool targetted);

    public void OnInteract(CharacterMovement player);

    public bool IsTargetted { get; }



}

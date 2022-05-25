using UnityEngine;

public interface IInteractible
{
    public abstract void Target(bool targetted);

    public void OnInteract(LocalCharacterMovement player);

    public bool IsTargetted { get; }



}

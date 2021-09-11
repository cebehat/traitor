using UnityEngine;

public interface IInteractible
{
    public abstract void Target(bool targetted);

    public void OnInteract(Player player);

    public bool IsTargetted { get; }


}

using System.Collections.Generic;
using UnityEngine;

public abstract class NestedBehaviour
{
    public GameObject Context { get; private set; }

    public NestedBehaviour (GameObject context)
    {
        Context = context;
    }

    public void Execute(NestedBehaviourExecutionContext context)
    {
        if (context.Mark(this)) return;

        OnExecute();
    }

    protected abstract void OnExecute();
}

public sealed class NestedBehaviourExecutionContext
{
    HashSet<NestedBehaviour> executedBehaviours = new HashSet<NestedBehaviour>();

    public bool Mark(NestedBehaviour behaviour)
    {
        bool state = executedBehaviours.Contains(behaviour);
        executedBehaviours.Add(behaviour);
        return state;
    }

    public void Clear() => executedBehaviours.Clear();
}
using System.Collections.Generic;

public abstract class NestedBehaviour { }

public sealed class NestedBehaviourExecutionContext
{
    HashSet<NestedBehaviour> executedBehaviours = new HashSet<NestedBehaviour>();

    public bool Mark(NestedBehaviour behaviour)
    {
        bool state = executedBehaviours.Contains(behaviour);
        executedBehaviours.Add(behaviour);
        return state;
    }

    public void Execute<T> (T behaviour, System.Action<T> callback) where T : NestedBehaviour
    {
        if (Mark(behaviour)) return;
        callback?.Invoke(behaviour);
    }

    public void Clear() => executedBehaviours.Clear();
}
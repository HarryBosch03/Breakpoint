using UnityEngine;

[SelectionBase]
[DisallowMultipleComponent]
public abstract class GameController : MonoBehaviour
{
    [SerializeField] GameSettings gameSettings;

    public GameSettings GameSettings => gameSettings;

    public static GameController instance { get; private set; }

    protected virtual void Start()
    {
        instance = this;
    }
}

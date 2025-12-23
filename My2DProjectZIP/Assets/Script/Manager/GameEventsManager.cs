using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventsManager : MonoBehaviour
{
    public static GameEventsManager Instance;

    public event System.Action OnPrimaryAttack;

    void Awake() => Instance = this;

    public void TriggerPrimaryAttack() => OnPrimaryAttack?.Invoke();
}

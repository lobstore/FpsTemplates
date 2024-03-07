using UnityEngine;
[CreateAssetMenu(fileName = "PlayerConfig", menuName = "Configs/PlayerConfig")]
 public class PlayerConfig : ScriptableObject
{
    [field: SerializeField] public float Speed { get; private set; }
    [field: SerializeField] public float StaminaRegen { get; private set; }
    [field: SerializeField] public float JumpCost { get; private set; }
    [field: SerializeField] public float SprintCost { get; private set; }
    [field: SerializeField] public float RechargeCooldown { get; private set; }
    [field: SerializeField] public float MaxStamina { get; private set; }
    [field: SerializeField] public float MaxHealth { get; private set; }
}
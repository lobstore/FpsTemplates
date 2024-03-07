using UnityEngine;
[CreateAssetMenu(fileName = "WeaponConfig", menuName = "Configs/WeaponConfig")]
internal class WeaponConfig:ScriptableObject
{
    [field: SerializeField] public string WeaponName;
    [field: SerializeField] public float ShotDelay { get; private set; }
    [field: SerializeField] public float MaxRange { get; private set; }
    [field: SerializeField] public float MinRecoil { get;private set; }
    [field: SerializeField] public float MaxRecoil { get;private set; }
    [field: SerializeField] public int MaxAmmo { get;private set; }
    [field: SerializeField] public int Damage { get;private set; }
}
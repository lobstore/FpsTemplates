using UnityEngine;
[CreateAssetMenu(fileName = "WeaponConfig", menuName = "Configs/WeaponConfig")]
public class WeaponConfig:ScriptableObject
{
    [field: SerializeField] public GameObject HolePrefab { get;private set; }
    [field: SerializeField] public string WeaponName { get; private set; }
    [field: SerializeField] public float ShotDelay { get; private set; }
    [field: SerializeField] public float ReloadTime { get; private set; }
    [field: SerializeField] public float MaxRange { get; private set; }
    [field: SerializeField] public float MinSpread { get;private set; }
    [field: SerializeField] public float MaxSpread { get;private set; }
    [field: SerializeField] public int MaxAmmoInMagazine { get;private set; }
    [field: SerializeField] public int AmmoAmount { get;private set; }
    [field: SerializeField] public int Damage { get;private set; }
    [field: SerializeField] public AudioClip ShotClip {  get; private set; }
    [field: SerializeField] public AudioClip ReloadClip {  get; private set; }
    [field: SerializeField] public Vector2[] RecoilTrace { get; private set; }
}
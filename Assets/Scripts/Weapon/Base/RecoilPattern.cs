using UnityEngine;

public class RecoilPattern
{
    public float SpreadEasing { get; protected set; } = 1f;
    public float MinSpread { get; private set; }
    public float MaxSpread { get; private set; }
    private float currentSpread;
    public float CurrentSpread
    {
        get { return currentSpread; }
        set
        {
            if (value > MaxSpread)
            {
                currentSpread = MaxSpread;
            }
            else if (value <= MinSpread)
            {
                currentSpread = MinSpread;
            }
            else
            {
                currentSpread = value;
            }
        }
    }

    int index;
    int Index
    {
        get => index;
        set
        {
            if (value > recoilTrace.Length - 1)
            {
                index = 0;
            }
            else if (value < 0)
            {
                index = recoilTrace.Length - 1;
            }
            else
            {
                index = value;
            }
        }
    }
    private Vector2[] recoilTrace;
    private float verticalRecoil;
    private float horizontalRecoil;
    Vector3 nextShotPosition = Vector3.zero;
    public RecoilPattern(WeaponConfig config)
    {
        recoilTrace = config.RecoilTrace;
        MinSpread = config.MinSpread;
        MaxSpread = config.MaxSpread;
        CurrentSpread = MinSpread;

    }
    public void ResetSpread() {
        CurrentSpread = MinSpread;
    }
    public void ResetRecoil()
    {
        nextShotPosition = Vector3.zero;
        Index = 0;
    }
    public Vector3 NextPosition()
    {
        horizontalRecoil = recoilTrace[Index].x / 10;
        verticalRecoil = recoilTrace[Index].y / 10;
        nextShotPosition.y = verticalRecoil + Random.Range(-CurrentSpread, CurrentSpread);
        nextShotPosition.x = horizontalRecoil + Random.Range(-CurrentSpread, CurrentSpread);
        Index++;
        return nextShotPosition;
    }
}
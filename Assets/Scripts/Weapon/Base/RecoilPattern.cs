using UnityEngine;

public class RecoilPattern
{
    int index;
    int Index
    {
        get => index; 
        set
        {
            if (value>recoilTrace.Length-1)
            {
                index = 0;
            }
            else if (value<0)
            {
                index =recoilTrace.Length-1;
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
    }
    public void Reset()
    {
        nextShotPosition = Vector3.zero;
        index = 0;
    }
    public void NextIndex()
    {
        Index++;
    }
    public float NextPositionX()
    {
        horizontalRecoil = recoilTrace[Index].x/10;
        nextShotPosition.x = horizontalRecoil;
        return nextShotPosition.x;
    }
    public float NextPositionY()
    {
        verticalRecoil = recoilTrace[Index].y / 10;
        nextShotPosition.y = verticalRecoil;
        return nextShotPosition.y;
    }
}
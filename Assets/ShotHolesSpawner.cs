using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class ShotHolesSpawner : MonoBehaviour
{
    private ObjectPool<GameObject> _pool;
    private Vector3 _shotDestination;
    public GameObject prefab;
    [SerializeField] float destroyTime;
    // Start is called before the first frame update
    void Start()
    {
        _pool = new(CreateHole, OnGet, OnRelease, OnDestroyHole, true, 10, 10);
    }
    private GameObject CreateHole()
    {
        GameObject temp = Instantiate(prefab);
        temp.transform.SetParent(transform, true);
        return temp;
    }

    public void TakeShot(Vector3 shotPosition)
    {
        _shotDestination = shotPosition;
        _pool.Get();
    }

    private void OnGet(GameObject go)
    {
        go.transform.position = _shotDestination;
        go.SetActive(true);
        StartCoroutine(SetLifetime(go));
    }
    IEnumerator SetLifetime(GameObject go)
    {
        float time = 0;
        while (time < destroyTime)
        {
            time += Time.deltaTime;
            yield return null;
        }
        _pool.Release(go);
    }
    private void OnRelease(GameObject go)
    {
        go.gameObject.SetActive(false);
    }
    private void OnDestroyHole(GameObject go)
    {
        Destroy(go.gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Grenade : MonoBehaviour
{
    [SerializeField] private bool IsContactExplosion;
    [SerializeField] private bool IsCountdownExplosion;
    private bool hasExploded = false;
    [SerializeField] float countdown = 5;
    [SerializeField] GameObject BOOM;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        if (IsCountdownExplosion && !hasExploded)
        {
            countdown -= Time.deltaTime;
            if (countdown <= 0 && !hasExploded)
            {
                Eplode();
            }
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!IsContactExplosion)
            return;
        if (collision.gameObject.layer == 22) {
            Eplode();
        }
    }

    private void Eplode()
    {
        Instantiate(BOOM, gameObject.transform.position, Quaternion.identity);
        hasExploded = true;
        Destroy(gameObject);
    }
}

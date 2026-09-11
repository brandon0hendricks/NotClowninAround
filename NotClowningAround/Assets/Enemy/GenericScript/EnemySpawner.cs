using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    // Script controls the enemy spawner. Spawns the enemy as well as the boundaries for it, based on where its two boundary positions are located.

    public enum enemyList
    {
        PeanutTosser,
        LionTamer,

    }
    public enemyList enemy;
    [SerializeField] private GameObject boundary1;
    [SerializeField] private GameObject boundary2;

    [SerializeField] private GameObject boundaryPrefab;
    [SerializeField] private GameObject peanutTosserPrefab;
    [SerializeField] private GameObject lionTamerPrefab;

    [HideInInspector] public bool isDead = false;
    private bool checkpointedState;

    

    private FollowPlayer followerScript;


    void Start()
    {
        if (!checkpointedState)
        {
            StartCoroutine(SpawnEnemies());
        }
    }


    private IEnumerator SpawnEnemies()
    {
        GameObject bound1 = Instantiate(boundaryPrefab, boundary1.transform.position, Quaternion.identity);
        GameObject bound2 = Instantiate(boundaryPrefab, boundary2.transform.position, Quaternion.identity);
        bound1.transform.parent = transform;
        bound2.transform.parent = transform;
        switch (enemy)
        {
            case enemyList.PeanutTosser:
                GameObject peenutMan = Instantiate(peanutTosserPrefab, transform.position, Quaternion.identity);
                followerScript = peenutMan.GetComponent<FollowPlayer>();
                peenutMan.GetComponent<EnemyParent>().parentSpawner = this;
                peenutMan.transform.parent = transform;
                followerScript.bound1 = bound1;
                followerScript.bound2 = bound2;
                break;
            case enemyList.LionTamer:
                GameObject whippah = Instantiate(lionTamerPrefab, transform.position, Quaternion.identity);
                followerScript = whippah.GetComponent<FollowPlayer>();
                whippah.GetComponent<EnemyParent>().parentSpawner = this;
                whippah.transform.parent = transform;
                followerScript.bound1 = bound1;
                followerScript.bound2 = bound2;
                break;
            default:
                Debug.Log(gameObject.name + " has no enemy selected!");
                break;
        }
        yield return new WaitForEndOfFrame();
    }
}


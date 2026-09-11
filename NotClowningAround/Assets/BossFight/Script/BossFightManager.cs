using Unity.Cinemachine;
using UnityEngine;

public class BossFightManager : MonoBehaviour
{
    // This will control the boss fight stages!
    // Ideally we have an extra 1st stage, but no promises. 


    [SerializeField] private BoxCollider2D enterTrigger;
    [SerializeField] private GameObject wallBlocker;
    [SerializeField] private BossController boss;


    public bool fightOngoing = false;
    public bool fightBegin = false;

    private void Update()
    {
        if (fightOngoing)
        {
            wallBlocker.SetActive(true);
            Destroy(enterTrigger);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            fightOngoing = true;
            boss.anim.SetBool("Float", true);
        }
    }

}

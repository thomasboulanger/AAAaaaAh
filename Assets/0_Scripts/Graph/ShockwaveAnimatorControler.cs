using UnityEngine;

public class ShockwaveAnimatorControler : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject shockWaveGo;
    [SerializeField] private GameObject followTarget;
    [SerializeField] private Vector3 offset;

    public int limbN;

    void Update()
    {
        if (!followTarget) return;
        transform.position = followTarget.transform.position + offset;
    }

    public void PlayFeedback(Component sender, object data1, object data2, object Unused)
    {
        if (followTarget != sender.gameObject) return;
        if (data1 is not bool) return;
        if (data2 is not float) return;

        animator.SetBool("Status", (bool) data1);

        if (!(bool) data1) return;

        GameObject shockWave = Instantiate
        (
            shockWaveGo,
            new Vector3(transform.position.x, transform.position.y, transform.position.z + .1f),
            Quaternion.Euler(new Vector3(0, 0, Random.Range(0, 360)) + transform.rotation.eulerAngles)
        );

        Destroy(shockWave, 1f);
    }
    
    private void OnDrawGizmos()
    {
        if (followTarget == null || Application.isPlaying) return;

        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(followTarget.transform.position + offset, 0.1f);
    }
}
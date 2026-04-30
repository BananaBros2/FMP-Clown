using UnityEngine;

public class CollectableObjectScript : MonoBehaviour
{
    [Tooltip("ID for tracking collectable")]
    [SerializeField] int manuscriptID = 0;

    private void Start()
    {
        // Change visual depending on if it has already been collected
        if (GameManager.Instance.GetManuscriptStatus(manuscriptID)) 
        {
            transform.GetComponent<Animator>().SetBool("Collected", true);
        }
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) // Collect the object if player enters it
        {
            GameManager.Instance.CollectManuscript(manuscriptID);
            Destroy(this.gameObject);

        }
    }

}

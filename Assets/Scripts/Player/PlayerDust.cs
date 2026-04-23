using UnityEngine;

public class PlayerDust : MonoBehaviour
{
    public void AnimationEnded()
    {
        Destroy(this.gameObject);
    }
}

using UnityEngine;

public class LizardAnimationEvents : MonoBehaviour
{
    [SerializeField] private LizardBoss lizardBoss;

    private void Awake()
    {
        if (lizardBoss == null)
        {
            lizardBoss = GetComponentInParent<LizardBoss>();
        }
    }

    public void ThrowRockFromAnimation()
    {
        if (lizardBoss != null)
        {
            lizardBoss.ThrowRockFromAnimation();
        }
    }
}
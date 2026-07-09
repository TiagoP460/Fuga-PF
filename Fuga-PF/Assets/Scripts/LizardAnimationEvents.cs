using UnityEngine;

public class LizardAnimationEvents : MonoBehaviour
{
    [SerializeField] private LizardBoss lizardBoss;

    private void Awake()
    {
        if (lizardBoss == null)
        {
            lizardBoss = GetComponent<LizardBoss>();

            if (lizardBoss == null)
            {
                lizardBoss = GetComponentInParent<LizardBoss>();
            }
        }
    }

    public void ThrowRockFromAnimation()
    {
        Debug.Log("EVENTO DA PEDRA CHAMADO");

        if (lizardBoss != null)
        {
            lizardBoss.ThrowRockFromAnimation();
        }
        else
        {
            Debug.LogWarning("LizardBoss não foi encontrado no LizardAnimationEvents.");
        }
    }
}
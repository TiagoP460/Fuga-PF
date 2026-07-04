using UnityEngine;

public class SecurityDoor : MonoBehaviour
{
    [SerializeField] private SecurityManager securityManager;

    private void Awake()
    {
        if (securityManager == null)
        {
            securityManager = FindAnyObjectByType<SecurityManager>();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Player"))
            return;

        if (securityManager != null && securityManager.IsSecurityDisabled)
        {
            MessageManager.instance.ShowMessage(
                "Porta liberada."
            );

            Destroy(gameObject);
        }
        else
        {
            MessageManager.instance.ShowMessage(
                "Acesso negado. Desative o sistema de segurança."
            );
        }
    }
}
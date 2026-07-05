using UnityEngine;
using UnityEngine.EventSystems;

public class WirePoint : MonoBehaviour, IPointerClickHandler
{
    public enum WireColor
    {
        Vermelho,
        Azul,
        Verde,
        Amarelo
    }

    public enum WireSide
    {
        Esquerda,
        Direita
    }

    [Header("Configuração do Ponto")]
    [SerializeField] private WireColor wireColor;
    [SerializeField] private WireSide wireSide;

    [HideInInspector] public bool connected = false;
    [HideInInspector] public WireMinigameManager manager;

    public WireColor ColorType => wireColor;
    public WireSide Side => wireSide;
    public RectTransform RectTransform => (RectTransform)transform;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (manager != null)
        {
            manager.SelectPoint(this);
        }
    }
}
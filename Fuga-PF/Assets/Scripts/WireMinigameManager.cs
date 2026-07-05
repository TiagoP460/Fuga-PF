using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WireMinigameManager : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private TerminalInteraction terminalInteraction;
    [SerializeField] private RectTransform linesContainer;
    [SerializeField] private Image linePrefab;
    [SerializeField] private WirePoint[] wirePoints;

    [Header("Visual da Linha")]
    [SerializeField] private float lineThickness = 12f;

    private WirePoint firstPoint;
    private int totalConnections = 0;

    private List<Image> createdLines = new List<Image>();

    private void Awake()
    {
        ConfigurePoints();
    }

    private void OnEnable()
    {
        ConfigurePoints();
    }

    private void ConfigurePoints()
    {
        foreach (WirePoint point in wirePoints)
        {
            if (point != null)
            {
                point.manager = this;
            }
        }
    }

    public void ResetMinigame()
    {
        firstPoint = null;
        totalConnections = 0;

        foreach (Image line in createdLines)
        {
            if (line != null)
            {
                Destroy(line.gameObject);
            }
        }

        createdLines.Clear();

        foreach (WirePoint point in wirePoints)
        {
            if (point != null)
            {
                point.connected = false;
                point.manager = this;
            }
        }
    }

    public void SelectPoint(WirePoint point)
    {
        if (point == null)
            return;

        if (point.connected)
            return;

        if (firstPoint == null)
        {
            if (point.Side != WirePoint.WireSide.Esquerda)
                return;

            firstPoint = point;
            Debug.Log("Primeiro ponto escolhido: " + point.ColorType);
            return;
        }

        if (point == firstPoint)
        {
            firstPoint = null;
            Debug.Log("Seleção cancelada.");
            return;
        }

        if (point.Side != WirePoint.WireSide.Direita)
            return;

        if (point.ColorType == firstPoint.ColorType)
        {
            CreateLine(
                firstPoint.RectTransform,
                point.RectTransform,
                GetColor(point.ColorType)
            );

            firstPoint.connected = true;
            point.connected = true;

            totalConnections++;
            firstPoint = null;

            Debug.Log("Conexão correta. Total: " + totalConnections);

            if (totalConnections >= 4)
            {
                CompleteMinigame();
            }
        }
        else
        {
            FailMinigame();
        }
    }

    private void CompleteMinigame()
    {
        Debug.Log("Minigame concluído.");

        if (terminalInteraction != null)
        {
            terminalInteraction.ResolveWireMinigame(true);
        }
    }

    private void FailMinigame()
    {
        Debug.Log("Ligação errada. Game Over.");

        if (terminalInteraction != null)
        {
            terminalInteraction.ResolveWireMinigame(false);
        }
    }

    private void CreateLine(RectTransform start, RectTransform end, Color color)
    {
        if (linePrefab == null)
        {
            Debug.LogWarning("Line Prefab não foi configurado.");
            return;
        }

        if (linesContainer == null)
        {
            Debug.LogWarning("Lines Container não foi configurado.");
            return;
        }

        Image line = Instantiate(linePrefab, linesContainer);
        line.gameObject.SetActive(true);
        line.color = color;
        line.raycastTarget = false;

        RectTransform lineRect = line.rectTransform;

        Vector2 startPos = GetLocalPosition(linesContainer, start);
        Vector2 endPos = GetLocalPosition(linesContainer, end);

        Vector2 direction = endPos - startPos;
        float distance = direction.magnitude;

        lineRect.anchorMin = new Vector2(0.5f, 0.5f);
        lineRect.anchorMax = new Vector2(0.5f, 0.5f);
        lineRect.pivot = new Vector2(0.5f, 0.5f);

        lineRect.anchoredPosition = (startPos + endPos) / 2f;
        lineRect.sizeDelta = new Vector2(distance, lineThickness);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        lineRect.localRotation = Quaternion.Euler(0f, 0f, angle);

        createdLines.Add(line);
    }

    private Vector2 GetLocalPosition(RectTransform container, RectTransform target)
    {
        Vector3 worldPosition = target.TransformPoint(target.rect.center);
        Vector3 localPosition = container.InverseTransformPoint(worldPosition);

        return new Vector2(localPosition.x, localPosition.y);
    }

    private Color GetColor(WirePoint.WireColor color)
    {
        switch (color)
        {
            case WirePoint.WireColor.Vermelho:
                return new Color(1f, 0.1f, 0.1f);

            case WirePoint.WireColor.Azul:
                return new Color(0.1f, 0.55f, 1f);

            case WirePoint.WireColor.Verde:
                return new Color(0.1f, 0.8f, 0.25f);

            case WirePoint.WireColor.Amarelo:
                return new Color(1f, 0.9f, 0.05f);

            default:
                return Color.white;
        }
    }
}
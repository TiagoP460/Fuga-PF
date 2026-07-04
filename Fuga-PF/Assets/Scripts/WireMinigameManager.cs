using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WireMinigameManager : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private SecurityManager securityManager;
    [SerializeField] private TerminalInteraction terminalInteraction;
    [SerializeField] private RectTransform linesContainer;
    [SerializeField] private Image linePrefab;
    [SerializeField] private WirePoint[] wirePoints;

    [Header("Visual")]
    [SerializeField] private float lineThickness = 12f;

    private WirePoint firstPoint;
    private int totalConnections = 0;
    private readonly List<Image> createdLines = new();

    private void Awake()
    {
        if (securityManager == null)
            securityManager = FindAnyObjectByType<SecurityManager>();

        foreach (WirePoint point in wirePoints)
        {
            if (point != null)
            {
                point.manager = this;
                point.connected = false;
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
                Destroy(line.gameObject);
        }

        createdLines.Clear();

        foreach (WirePoint point in wirePoints)
        {
            if (point != null)
                point.connected = false;
        }
    }

    public void SelectPoint(WirePoint point)
    {
        if (point == null || point.connected)
            return;

        // Primeiro clique: só aceita pontos da esquerda
        if (firstPoint == null)
        {
            if (point.Side != WirePoint.WireSide.Esquerda)
                return;

            firstPoint = point;
            return;
        }

        // Clicou no mesmo ponto: cancela a seleção
        if (point == firstPoint)
        {
            firstPoint = null;
            return;
        }

        // Segundo clique: só aceita pontos da direita
        if (point.Side != WirePoint.WireSide.Direita)
            return;

        // Verifica se a cor bate
        if (point.ColorType == firstPoint.ColorType)
        {
            CreateLine(firstPoint.RectTransform, point.RectTransform, GetUnityColor(point.ColorType));

            firstPoint.connected = true;
            point.connected = true;

            totalConnections++;
            firstPoint = null;

            if (totalConnections >= 4)
            {
                Success();
            }
        }
        else
        {
            Fail();
        }
    }

    private void Success()
    {
        if (terminalInteraction != null)
        {
            terminalInteraction.ResolveWireMinigame(true);
        }
        else if (securityManager != null)
        {
            securityManager.DisableSecurity();
        }
    }

    private void Fail()
    {
        if (terminalInteraction != null)
        {
            terminalInteraction.ResolveWireMinigame(false);
        }
        else if (securityManager != null)
        {
            securityManager.TriggerSecurityFail();
        }
    }

    private void CreateLine(RectTransform start, RectTransform end, Color color)
    {
        if (linePrefab == null || linesContainer == null)
            return;

        Image line = Instantiate(linePrefab, linesContainer);
        line.gameObject.SetActive(true);
        line.color = color;

        RectTransform lineRect = line.rectTransform;

        Vector2 startPos = GetLocalPoint(linesContainer, start);
        Vector2 endPos = GetLocalPoint(linesContainer, end);

        Vector2 direction = endPos - startPos;
        float length = direction.magnitude;

        lineRect.anchorMin = new Vector2(0.5f, 0.5f);
        lineRect.anchorMax = new Vector2(0.5f, 0.5f);
        lineRect.pivot = new Vector2(0.5f, 0.5f);

        lineRect.sizeDelta = new Vector2(length, lineThickness);
        lineRect.anchoredPosition = (startPos + endPos) / 2f;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        lineRect.localRotation = Quaternion.Euler(0f, 0f, angle);

        createdLines.Add(line);
    }

    private Vector2 GetLocalPoint(RectTransform container, RectTransform target)
    {
        Vector3 worldPoint = target.TransformPoint(target.rect.center);
        Vector3 localPoint = container.InverseTransformPoint(worldPoint);
        return new Vector2(localPoint.x, localPoint.y);
    }

    private Color GetUnityColor(WirePoint.WireColor color)
    {
        return color switch
        {
            WirePoint.WireColor.Vermelho => new Color(1f, 0.2f, 0.2f),
            WirePoint.WireColor.Azul => new Color(0.15f, 0.6f, 1f),
            WirePoint.WireColor.Verde => new Color(0.2f, 0.8f, 0.3f),
            WirePoint.WireColor.Amarelo => new Color(1f, 0.9f, 0.15f),
            _ => Color.white
        };
    }
}
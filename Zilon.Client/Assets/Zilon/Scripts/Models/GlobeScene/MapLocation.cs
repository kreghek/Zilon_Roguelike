using System;
using System.Collections.Generic;

using UnityEngine;

using Zilon.Core.Client;
using Zilon.Core.World;

/// <summary>
/// Модель узла провинции на глобальной карте.
/// </summary>
public class MapLocation : MonoBehaviour, IGlobeNodeViewModel
{
    public event EventHandler OnSelect;
    public event EventHandler OnHover;
    public SpriteRenderer Icon;

    public GlobeRegionNode Node { get; set; }

    public GlobeRegion ParentRegion { get; set; }

    public bool OtherRegion { get; set; }

    public List<MapLocation> NextNodes { get; }
    public List<MapLocationConnector> Connectors { get; }

    public MapLocation()
    {
        NextNodes = new List<MapLocation>();
        Connectors = new List<MapLocationConnector>();
    }

    public void Start()
    {
        if (Node.Scheme.Sid == "forest")
        {
            var sprite = Resources.Load<Sprite>("Globe/forest");
            Icon.sprite = sprite;
        }
        else if (Node.Scheme.Sid == "city")
        {
            var sprite = Resources.Load<Sprite>("Globe/city");
            Icon.sprite = sprite;

            if (Node.IsHome)
            {
                gameObject.transform.localScale = new Vector3(2, 2, 2);
            }
        }
        else
        {
            var sprite = Resources.Load<Sprite>("Globe/gungeon");
            Icon.sprite = sprite;
        }

        if (OtherRegion)
        {
            Icon.color = Color.cyan;
        }

        if (Node == null)
        {
            throw new ArgumentNullException(nameof(Node));
        }

        Node.ObservedStateChanged += Node_ObservedStateChanged;
        UpdateObservedState();
    }

    private void Node_ObservedStateChanged(object sender, EventArgs e)
    {
        UpdateObservedState();
    }

    public void OnMouseDown()
    {
        OnSelect?.Invoke(this, new EventArgs());
    }

    public void OnMouseOver()
    {
        OnHover?.Invoke(this, new EventArgs());
    }

    public void OnDestroy()
    {
        Node.ObservedStateChanged -= Node_ObservedStateChanged;
    }

    internal void UpdateObservedState()
    {
        if (Node.ObservedState == GlobeNodeObservedState.Visited)
        {
            gameObject.SetActive(true);

            if (!OtherRegion)
            {
                Icon.color = Color.white;
            }
            else
            {
                Icon.color = Color.cyan;
            }

            foreach (var nextNode in NextNodes)
            {
                if (nextNode.Node.ObservedState == GlobeNodeObservedState.Hidden)
                {
                    nextNode.gameObject.SetActive(true);
                    if (!nextNode.OtherRegion)
                    {
                        nextNode.Icon.color = Color.grey;
                    }
                    else
                    {
                        nextNode.Icon.color = Color.Lerp(Color.cyan, Color.gray, 0.5f);
                    }
                }
                else
                {
                    nextNode.gameObject.SetActive(true);

                    if (!nextNode.OtherRegion)
                    {
                        nextNode.Icon.color = Color.white;
                    }
                    else
                    {
                        nextNode.Icon.color = Color.cyan;
                    }
                }

                // Визуализируем коннекторы
                foreach (var connector in Connectors)
                {
                    connector.gameObject.SetActive(false);

                    if (connector.gameObject1 == gameObject || connector.gameObject2 == gameObject)
                    {
                        connector.gameObject.SetActive(true);
                    }
                }
            }
        }
        else
        {
            gameObject.SetActive(false);
            foreach (var nextNode in NextNodes)
            {
                if (nextNode.Node.ObservedState == GlobeNodeObservedState.Visited)
                {
                    gameObject.SetActive(true);

                    if (!OtherRegion)
                    {
                        Icon.color = Color.grey;
                    }
                    else
                    {
                        Icon.color = Color.Lerp(Color.cyan, Color.gray, 0.5f);
                    }

                    // Визуализируем коннекторы
                    foreach (var connector in Connectors)
                    {
                        connector.gameObject.SetActive(false);

                        if (connector.gameObject1 == gameObject || connector.gameObject2 == gameObject)
                        {
                            connector.gameObject.SetActive(true);
                        }
                    }
                }
            }
        }
    }
}
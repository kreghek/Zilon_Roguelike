using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zilon.Core.Schemes;
using Zilon.Core.Services;

/// <summary>
/// Модель представления глобальной карты.
/// </summary>
public class GlobeMapVM : MonoBehaviour
{
    /// <summary>
    /// Префаб узла карты.
    /// </summary>
    public MapLocation LocationPrefab;

    /// <summary>
    /// Префаб пути на карте.
    /// </summary>
    public MapLocationConnector ConnectorPrefab;

    /// <summary>
    /// Префаб группы.
    /// </summary>
    public GroupVM GroupPrefab;

    /// <summary>
    /// Ссылка на канвас на сцене Globe.
    /// </summary>
    public Canvas Canvas;

    /// <summary>
    /// Текущий набор путей представления узлов карты.
    /// </summary>
    public List<MapLocation> Nodes { get; set; }

    /// <summary>
    /// Текущий набор моделей представления путей между узлами карты.
    /// </summary>
    public List<MapLocationConnector> Paths { get; set; }

    /// <summary>
    /// Текущие модели представления групп на карте.
    /// </summary>
    public List<GroupVM> Groups { get; set; }

    /// <summary>
    /// Текущая выбранная пользователем группа.
    /// </summary>
    public GroupVM SelectedGroup { get; set; }

    public void CreateMapEntities(ISchemeService schemeService, string sid)
    {
        if (schemeService == null)
        {
            throw new System.ArgumentNullException(nameof(schemeService));
        }

        if (string.IsNullOrEmpty(sid))
        {
            throw new System.ArgumentException("Символьный идентификатор карты не может быть пустым.", nameof(sid));
        }

        var locationSchemes = schemeService.GetSchemes<LocationScheme>();
        var mapLocationSchemes = locationSchemes.Where(x => x.MapSid == sid).ToArray();

        var pathSchemes = schemeService.GetSchemes<PathScheme>();
        var mapPathSchemes = pathSchemes.Where(x => x.MapSid == sid).ToArray();

        Nodes = new List<MapLocation>();
        foreach (var locationScheme in mapLocationSchemes)
        {
            var location = Instantiate(LocationPrefab, transform);
            location.Sid = locationScheme.Sid;
            location.transform.position = new Vector3(locationScheme.X, locationScheme.Y);
            Nodes.Add(location);
            location.Canvas = Canvas;
            location.OnSelect += Location_OnSelect;
        }

        Paths = new List<MapLocationConnector>();
        foreach (var pathScheme in mapPathSchemes)
        {
            var connector = Instantiate(ConnectorPrefab, transform);

            var location1 = Nodes.SingleOrDefault(x => x.Sid == pathScheme.Sid1);
            var location2 = Nodes.SingleOrDefault(x => x.Sid == pathScheme.Sid2);

            connector.gameObject1 = location1.gameObject;
            connector.gameObject2 = location2.gameObject;
            connector.Scheme = pathScheme;
            Paths.Add(connector);
        }

        Groups = new List<GroupVM>();
        var group = Instantiate(GroupPrefab, transform);
        group.CurrentLocation = Nodes.First();
        group.transform.position = group.CurrentLocation.transform.position;
        group.OnSelect += Group_OnSelect;
        Groups.Add(group);
    }

    private void Location_OnSelect(object sender, System.EventArgs e)
    {
        if (SelectedGroup != null)
        {
            var targetLocation = sender as MapLocation;
            SelectedGroup.CurrentLocation = targetLocation;
            UpdateLocations();
        }
    }

    private void Group_OnSelect(object sender, System.EventArgs e)
    {
        // Сброс текущего выделения группы
        if (SelectedGroup != null)
        {
            SelectedGroup.SetSelectState(false);
        }

        SelectedGroup = sender as GroupVM;
        SelectedGroup.SetSelectState(true);

        UpdateLocations();
    }

    private void UpdateLocations()
    {
        // Затенить все не доступные локации
        var currentLocation = SelectedGroup.CurrentLocation;
        foreach (var location in Nodes)
        {
            UpdateLocationAvailableState(currentLocation, location);
        }
    }

    private void UpdateLocationAvailableState(MapLocation currentLocation, MapLocation location)
    {
        var isAvailable = IsLocationAvailable(currentLocation, location);
        location.SetAvailableState(isAvailable);
    }

    private bool IsLocationAvailable(MapLocation currentLocation, MapLocation location)
    {
        if (currentLocation == location)
        {
            return true;
        }

        var currentPaths = Paths.Where(path => path.Scheme.Sid1 == currentLocation.Sid ||
                path.Scheme.Sid2 == currentLocation.Sid).ToArray();

        foreach (var path in currentPaths)
        {
            if (path.Scheme.Sid1 == location.Sid || path.Scheme.Sid2 == location.Sid)
            {
                return true;
            }
        }

        return false;
    }
}

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zilon.Logic.Schemes;
using Zilon.Logic.Services;

public class Map : MonoBehaviour
{

    public MapLocation LocationPrefab;
    public MapLocationConnector ConnectorPrefab;
    public GroupVM GroupPrefab;
    public Canvas Canvas;

    public List<MapLocation> MapLocations { get; set; }
    public List<MapLocationConnector> Paths { get; set; }
    public List<GroupVM> Groups { get; set; }
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

        var mapScheme = schemeService.GetScheme<MapScheme>(sid);

        var locationSchemes = schemeService.GetSchemes<LocationScheme>();
        var mapLocationSchemes = locationSchemes.Where(x => x.MapSid == sid).ToArray();

        var pathSchemes = schemeService.GetSchemes<PathScheme>();
        var mapPathSchemes = pathSchemes.Where(x => x.MapSid == sid).ToArray();

        var locations = new List<MapLocation>();
        foreach (var locationScheme in mapLocationSchemes)
        {
            var location = Instantiate(LocationPrefab, transform);
            location.Sid = locationScheme.Sid;
            location.transform.position = new Vector3(locationScheme.X, locationScheme.Y);
            locations.Add(location);
            location.Canvas = Canvas;
            location.OnSelect += Location_OnSelect;
        }

        Paths = new List<MapLocationConnector>();
        foreach (var pathScheme in mapPathSchemes)
        {
            var connector = Instantiate(ConnectorPrefab, transform);

            var location1 = locations.SingleOrDefault(x => x.Sid == pathScheme.Sid1);
            var location2 = locations.SingleOrDefault(x => x.Sid == pathScheme.Sid2);

            connector.gameObject1 = location1.gameObject;
            connector.gameObject2 = location2.gameObject;
            connector.Scheme = pathScheme;
            Paths.Add(connector);
        }

        MapLocations = locations;

        Groups = new List<GroupVM>();
        var group = Instantiate(GroupPrefab, transform);
        group.CurrentLocation = locations.First();
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

        // Затенить все не доступные локации
        var currentLocation = SelectedGroup.CurrentLocation;
        foreach (var location in MapLocations)
        {
            location.SetAvailableState(false);
            UpdateLocationAvailableState(currentLocation, location);
        }
    }

    private void UpdateLocationAvailableState(MapLocation currentLocation, MapLocation location)
    {
        if (currentLocation == location)
        {
            location.SetAvailableState(true);
            return;
        }

        foreach (var path in Paths)
        {
            if (path.PathScheme.Sid1 == currentLocation.Sid ||
                path.PathScheme.Sid2 == currentLocation.Sid)
            {
                location.SetAvailableState(true);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}

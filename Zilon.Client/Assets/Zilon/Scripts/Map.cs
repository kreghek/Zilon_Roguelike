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
    public List<GroupVM> Groups { get; set; }
    public GroupVM SelectedGroup { get; set; }

    public void CreateMapEntities(ISchemeService schemeService, string sid)
    {
        if (schemeService == null)
        {
            throw new System.ArgumentNullException(nameof(schemeService));
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

        foreach (var transitionScheme in mapPathSchemes)
        {
            var connector = Instantiate(ConnectorPrefab, transform);

            connector.gameObject1 = locations.SingleOrDefault(x => x.Sid == transitionScheme.Sid1).gameObject;
            connector.gameObject2 = locations.SingleOrDefault(x => x.Sid == transitionScheme.Sid2).gameObject;
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
    }

    // Update is called once per frame
    void Update()
    {

    }
}

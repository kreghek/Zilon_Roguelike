using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Map : MonoBehaviour
{

    public MapLocation LocationPrefab;
    public MapLocationConnector ConnectorPrefab;
    public GroupVM GroupPrefab;
    public Canvas Canvas;

    private LocationScheme[] locationSchemes;
    private LocationTrasitionScheme[] trasitionSchemes;

    public List<MapLocation> MapLocations { get; set; }
    public List<GroupVM> Groups { get; set; }
    public GroupVM SelectedGroup { get; set; }

    public Map()
    {
        locationSchemes = new[] {
            new LocationScheme{
                Sid = "loc1",
                Position = new Vector2(0, 0)
            },
            new LocationScheme{
                Sid = "loc2",
                Position = new Vector2(3, 1)
            },
            new LocationScheme{
                Sid = "loc3",
                Position = new Vector2(-4, 3)
            },
            new LocationScheme{
                Sid = "loc4",
                Position = new Vector2(-5.5f, 2.5f)
            }
        };

        trasitionSchemes = new[] {
            new LocationTrasitionScheme{
                StartLocationSid = "loc1",
                EndLocationSid = "loc2",
            },
            new LocationTrasitionScheme{
                StartLocationSid = "loc1",
                EndLocationSid = "loc3",
            },
            new LocationTrasitionScheme{
                StartLocationSid = "loc4",
                EndLocationSid = "loc3",
            },
        };
    }

    public void CreateMapEntities()
    {
        var locations = new List<MapLocation>();
        foreach (var locationScheme in locationSchemes)
        {
            var location = Instantiate(LocationPrefab, transform);
            location.Sid = locationScheme.Sid;
            location.transform.position = new Vector3(locationScheme.Position.x, locationScheme.Position.y);
            locations.Add(location);
            location.Canvas = Canvas;
            location.OnSelect += Location_OnSelect;
        }

        foreach (var transitionScheme in trasitionSchemes)
        {
            var connector = Instantiate(ConnectorPrefab, transform);

            connector.gameObject1 = locations.SingleOrDefault(x => x.Sid == transitionScheme.StartLocationSid).gameObject;
            connector.gameObject2 = locations.SingleOrDefault(x => x.Sid == transitionScheme.EndLocationSid).gameObject;
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

    private class LocationScheme
    {
        public string Sid { get; set; }
        public Vector2 Position { get; set; }
    }

    private class LocationTrasitionScheme
    {
        public string StartLocationSid { get; set; }
        public string EndLocationSid { get; set; }
    }
}

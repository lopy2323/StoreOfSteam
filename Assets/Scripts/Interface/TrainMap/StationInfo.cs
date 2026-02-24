using System.Linq;
using UnityEngine;

public class StationInfo : MonoBehaviour
{
    public string stationName;
    public TrainTrackPoint stationConnections;

    void Start()
    {
        stationConnections = GetComponent<TrainTrackPoint>();
    }
}

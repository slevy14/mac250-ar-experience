using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ARLocation {
    public class WebMapLoader : MonoBehaviour
    {

        public class DataEntry
        {
            public int id;
            public double lat;
            public double lng;
            public double altitude;
            public string altitudeMode;
            public string name;
            public string meshId;
            public float movementSmoothing;
            public int maxNumberOfLocationUpdates;
            public bool useMovingAverage;
            public bool hideObjectUtilItIsPlaced;

            public AltitudeMode getAltitudeMode()
            {
                if (altitudeMode == "GroundRelative") {
                    return AltitudeMode.GroundRelative;
                } else if (altitudeMode == "DeviceRelative") {
                    return AltitudeMode.DeviceRelative;
                } else if (altitudeMode == "Absolute") {
                    return AltitudeMode.Absolute;
                } else {
                    return AltitudeMode.Ignore;
                }
            }
        }

        /// <summary>
        ///   The `PrefabDatabase` ScriptableObject, containing a dictionary of Prefabs with a string ID.
        /// </summary>
        public PrefabDatabase PrefabDatabase;

        /// <summary>
        ///   The XML data file download from the Web Map Editor (htttps://editor.unity-ar-gps-location.com)
        /// </summary>
        public TextAsset XmlDataFile;

        /// <summary>
        ///   If true, enable DebugMode on the `PlaceAtLocation` generated instances.
        /// </summary>
        public bool DebugMode;

        private List<DataEntry> _dataEntries = new List<DataEntry>();
        private List<GameObject> _stages = new List<GameObject>();
        private List<PlaceAtLocation> _placeAtComponents = new List<PlaceAtLocation>();

        // Start is called before the first frame update
        void Start()
        {


            LoadXmlFile();
            BuildGameObjects();
            
            Dot = GameObject.FindWithTag("locationDot");
            

            startPosition = Dot.transform.position;
            Mover();
            
            
        }

        

        void BuildGameObjects()
        {
            foreach (var entry in _dataEntries)
            {
                var Prefab = PrefabDatabase.GetEntryById(entry.meshId);

                if (!Prefab)
                {
                    Debug.LogWarning($"[ARLocation#WebMapLoader]: Prefab {entry.meshId} not found.");
                    continue;
                }

                var PlacementOptions = new PlaceAtLocation.PlaceAtOptions()
                    {
                        MovementSmoothing = entry.movementSmoothing,
                        MaxNumberOfLocationUpdates = entry.maxNumberOfLocationUpdates,
                        UseMovingAverage = entry.useMovingAverage,
                        HideObjectUntilItIsPlaced = entry.hideObjectUtilItIsPlaced
                    };

                var location = new Location()
                    {
                        Latitude = entry.lat,
                        Longitude = entry.lng,
                        Altitude = entry.altitude,
                        AltitudeMode = entry.getAltitudeMode(),
                        Label = entry.name
                    };

                var instance = PlaceAtLocation.CreatePlacedInstance(Prefab,
                                                                    location,
                                                                    PlacementOptions,
                                                                    DebugMode);

                _stages.Add(instance);
            }
        }

        // Update is called once per frame
        void LoadXmlFile()
        {
            var xmlString = XmlDataFile.text;

            Debug.Log(xmlString);

            XmlDocument xmlDoc = new XmlDocument();

            try {
                xmlDoc.LoadXml(xmlString);
            } catch(XmlException e) {
                Debug.LogError("[ARLocation#WebMapLoader]: Failed to parse XML file: " + e.Message);
            }

            var root = xmlDoc.FirstChild;
            var nodes = root.ChildNodes;
            foreach (XmlNode node in nodes)
            {
                Debug.Log(node.InnerXml);
                Debug.Log(node["id"].InnerText);

                int id = int.Parse(node["id"].InnerText);
                double lat = double.Parse(node["lat"].InnerText, CultureInfo.InvariantCulture);
                double lng = double.Parse(node["lng"].InnerText, CultureInfo.InvariantCulture);
                double altitude = double.Parse(node["altitude"].InnerText, CultureInfo.InvariantCulture);
                string altitudeMode = node["altitudeMode"].InnerText;
                string name = node["name"].InnerText;
                string meshId = node["meshId"].InnerText;
                float movementSmoothing = float.Parse(node["movementSmoothing"].InnerText, CultureInfo.InvariantCulture);
                int maxNumberOfLocationUpdates = int.Parse(node["maxNumberOfLocationUpdates"].InnerText);
                bool useMovingAverage = bool.Parse(node["useMovingAverage"].InnerText);
                bool hideObjectUtilItIsPlaced = bool.Parse(node["hideObjectUtilItIsPlaced"].InnerText);

                DataEntry entry = new DataEntry() {
                    id = id,
                    lat = lat,
                    lng = lng,
                    altitudeMode = altitudeMode,
                    altitude = altitude,
                    name = name,
                    meshId = meshId,
                    movementSmoothing = movementSmoothing,
                    maxNumberOfLocationUpdates = maxNumberOfLocationUpdates,
                    useMovingAverage =useMovingAverage,
                    hideObjectUtilItIsPlaced = hideObjectUtilItIsPlaced };

                _dataEntries.Add(entry);

                Debug.Log($"{id}, {lat}, {lng}, {altitude}, {altitudeMode}, {name}, {meshId}, {movementSmoothing}, {maxNumberOfLocationUpdates}, {useMovingAverage}, {hideObjectUtilItIsPlaced}");
                
                
                //Debug.Log($"{id}, {lat}, {lng}")
            }
        }

        // Update is called once per frame


    private GameObject Dot;

    private bool waitingToUpdate;
    private Vector3 startPosition;
    private float currlongitude;
    private float currlatitude;

    // In the bottom left corner of the map on 1566 Hazelwood Avenue
    private double latStart = 34.124547374284774;
    private double lngStart = -118.21432838197913;

    private double latZoom = 34.12601869098553;
    private double lngZoom = -118.21323756601166;


    // At the the intersection by Moore and Sycamore Glen
    private double latRef1 = 34.12657446034415;
    private double lngRef1 = -118.21055867799848;

    // in front of weingart
    private double latRef2 = 34.12786474921631;
    private double lngRef2 = -118.21068629607586;

    // between the MP and field
    private double latRef3 = 34.127540222252456;
    private double lngRef3 = -118.21251606815756;

    public bool dev = false;

    [Range(1, 3)] public int rf = 1;


    [Range(0, 3)] public double publicZoom = 1;

    [Range(-300, 300)] public int offsetZoomX = 0;
    [Range(-300, 300)] public int offsetZoomY = 0;

    private void Update()
    {
        if (!waitingToUpdate)
        {
            StartCoroutine(FetchLocationData());
        }
    }

    private IEnumerator FetchLocationData()
    {
        waitingToUpdate = true;
        yield return new WaitForSeconds(1);

        // First, check if user has location service enabled
        if (!Input.location.isEnabledByUser)
        {


            print("Location disabled");
            waitingToUpdate = false;
            yield break;
        }

        // Start service before querying location
        Input.location.Start();
        print("Fetching Location..");
        // Wait until service initializes
        int maxWait = 60;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            print("Location inizializing");


            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // Service didn't initialize in 60 seconds
        if (maxWait < 1)
        {
            print("Location Timed out");
        

            waitingToUpdate = false;
            yield break;
        }

        // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            print("Unable to determine device location");


            waitingToUpdate = false;
            yield break;
        }
        else
        {
            currlatitude = Input.location.lastData.latitude;
            currlongitude = Input.location.lastData.longitude;
            print("fetched data!  (" + currlatitude + ", " + currlongitude + ")");
            Mover();
            //latLonTest.text = "(" + currlatitude + ", " + currlongitude + ")";
        }
    } // updates currLatitude, currLongitude whenever called (updates every second)


    void Mover()
    {
        double zoomFactor = 1.6;
        double latZoomFactor = 2;
        double lngZoomFactor = 1.4;

        double latNew = currlatitude;
        double lngNew = currlongitude;

        if (dev)
        {
            if (rf == 1)
            {
                latNew = latRef1;
                lngNew = lngRef1;
            }
            else if (rf == 2)
            {
                latNew = latRef2;
                lngNew = lngRef2;
            }
            else if (rf == 3)
            {
                latNew = latRef3;
                lngNew = lngRef3;
            }
        }

        double fineLatDiff = latNew - latStart;
        double fineLngDiff = lngNew - lngStart;

        double latDiff = (fineLatDiff * 100000) * latZoomFactor * publicZoom + offsetZoomY;
        double lngDiff = (fineLngDiff * 100000) * lngZoomFactor * publicZoom + offsetZoomX;


        latDiff = latDiff + offsetZoomY;
        lngDiff = lngDiff + offsetZoomX;
        print(latDiff + ", " + lngDiff);
        if (dev)
        {
            print( Math.Round(currlatitude, 4) + " " + Math.Round(currlongitude, 4) + "\n" + fineLatDiff +
                              " " + fineLngDiff + "\n" + latDiff + ", " + lngDiff);
        }

        Dot.transform.position = startPosition + new Vector3((float) lngDiff, (float) latDiff, 0);
        waitingToUpdate = false;
    }
        
        
    }
}

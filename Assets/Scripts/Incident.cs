using UnityEngine;
using System.Collections.Generic;

namespace Projeto1IA
{
    public class Incident
    {
        public IncidentType Type { get; private set; }
        public string OriginLocationName { get; private set; }
        public float StartTime { get; private set; }

        public HashSet<string> AffectedLocations { get; private set; } = new HashSet<string>();

        public HashSet<string> ImpassableLocations { get; private set; } = new HashSet<string>();

        public bool IsResolved { get; private set; }

        public Incident(IncidentType type, string originLocationName)
        {
            Type = type;
            OriginLocationName = originLocationName;
            StartTime = Time.time;
            AffectedLocations.Add(originLocationName);
        }

        public void SpreadTo(string locationName)
        {
            AffectedLocations.Add(locationName);
        }

        public void MakeImpassable(string locationName)
        {
            ImpassableLocations.Add(locationName);
            AffectedLocations.Add(locationName);
        }

        public void Resolve()
        {
            IsResolved = true;
            AffectedLocations.Clear();
            ImpassableLocations.Clear();
        }

        public bool IsLocationAffected(string locationName) => AffectedLocations.Contains(locationName);
        public bool IsLocationImpassable(string locationName) => ImpassableLocations.Contains(locationName);
        public float ElapsedTime => Time.time - StartTime;
    }
}

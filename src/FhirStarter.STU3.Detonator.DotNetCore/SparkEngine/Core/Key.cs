﻿using FhirStarter.STU3.Detonator.DotNetCore.SparkEngine.Extensions;

namespace FhirStarter.STU3.Detonator.DotNetCore.SparkEngine.Core
{
    public interface IKey
    {
        string Base { get; set; }
        string TypeName { get; set; }
        string ResourceId { get; set; }
        string VersionId { get; set; }
    }

    public class Key : IKey
    {
        public string Base { get; set; }
        public string TypeName { get; set; }
        public string ResourceId { get; set; }
        public string VersionId { get; set; }

        public Key(string _base, string type, string resourceid, string versionid)
        {
            Base = _base?.TrimEnd('/');
            TypeName = type;
            ResourceId = resourceid;
            VersionId = versionid;
        }

        public override string ToString()
        {
            return this.ToUriString();
        }

        public static Key Create(string type)
        {
            return new Key(null, type, null, null);
        }

        public static Key Create(string type, string resourceid)
        {
            return new Key(null, type, resourceid, null);
        }

        public static Key Create(string type, string resourceid, string versionid)
        {
            return new Key(null, type, resourceid, versionid);
        }
    }
}
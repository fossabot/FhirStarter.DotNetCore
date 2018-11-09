﻿/* 
 * Copyright (c) 2014, Furore (info@furore.com) and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the BSD 3-Clause license
 * available at https://raw.github.com/furore-fhir/spark/master/LICENSE
 */

using System;
using System.Collections.Generic;
using System.Text;
using FhirStarter.Bonfire.DotNetCore.SparkEngine.Extensions;
using Hl7.Fhir.Model;

namespace FhirStarter.Bonfire.DotNetCore.SparkEngine.Core
{
    public enum EntryState
    {
        Internal,
        Undefined,
        External
    }

    public class Entry
    {
        public IKey Key
        {
            get
            {
                if (Resource != null)
                {
                    return Resource.ExtractKey();
                }
                return _key;
            }
            set
            {
                if (Resource != null)
                {
                    value.ApplyTo(Resource);
                }
                else
                {
                    _key = value;
                }
            }
        }

        public Resource Resource { get; set; }
        public Bundle.HTTPVerb Method { get; set; }
        // API: HttpVerb should not be in Bundle.
        public DateTimeOffset? When
        {
            get
            {
                if (Resource?.Meta != null)
                {
                    return Resource.Meta.LastUpdated;
                }
                return _when;
            }
            set
            {
                if (Resource != null)
                {
                    if (Resource.Meta == null) Resource.Meta = new Meta();
                    Resource.Meta.LastUpdated = value;
                }
                else
                {
                    _when = value;
                }
            }
        }

        public EntryState State { get; set; }

        private IKey _key;
        private DateTimeOffset? _when;

        protected Entry(Bundle.HTTPVerb method, IKey key, DateTimeOffset? when, Resource resource)
        {
            if (resource != null)
            {
                key.ApplyTo(resource);
            }
            else
            {
                Key = key;
            }
            Resource = resource;
            Method = method;
            When = when ?? DateTimeOffset.Now;
            State = EntryState.Undefined;
        }


        public override string ToString()
        {
            return $"{Method} {Key}";
        }
    }
}

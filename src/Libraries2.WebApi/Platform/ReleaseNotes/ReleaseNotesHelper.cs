using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using Newtonsoft.Json.Linq;
using Xlent.Lever.Libraries2.Core.Assert;

namespace Xlent.Lever.Libraries2.WebApi.Platform.ReleaseNotes
{
    /// <summary>
    /// Provides release notes for the service.
    /// </summary>
    public class ReleaseNotesHelper
    {
        /// <summary>
        /// Reads ReleaseNotes.json from App_Data and parses it.
        /// </summary>
        public static IEnumerable<ReleaseNotes> ReadReleaseNotes()
        {
            const string filePath = "~/App_Data/ReleaseNotes.json";
            var file = HostingEnvironment.MapPath(filePath);
            FulcrumAssert.IsNotNull(file, null, $"Expected file '{filePath}' to exist");
            var text = File.ReadAllText(file ?? throw new ArgumentNullException());
            var entries = JArray.Parse(text);
            return entries.Select(x => x.ToObject<ReleaseNotes>());
        }
    }
}

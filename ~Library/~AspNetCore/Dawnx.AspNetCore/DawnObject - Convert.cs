﻿using Newtonsoft.Json;

namespace Dawnx
{
    public static partial class DawnObject
    {
        /// <summary>
        /// Serializes the specified object to a JSON string.
        /// </summary>
        /// <param name="this"></param>
        /// <returns></returns>
        public static string Json(this object @this) => JsonConvert.SerializeObject(@this);

    }
}

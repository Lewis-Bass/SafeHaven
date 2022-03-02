using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibraries.Models
{
    public class GenericRestResponse
    {
        /// <summary>
        /// Array of responses from the rest service
        /// </summary>
        public string[] ResponseString { get; set; }

        /// <summary>
        /// Array of exception messages from the rest service
        /// </summary>
        public string[] ErrorList { get; set; }

        /// <summary>
        /// License involved with the response
        /// </summary>
        public string License { get; set; }

        /// <summary>
        /// Device involved with the rest service
        /// </summary>
        public string Device { get; set; }
    }
}

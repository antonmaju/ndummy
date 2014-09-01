using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NDummy
{
    /// <summary>
    /// Holds information of the current creation context
    /// </summary>
    public class ObjectCreationContext
    {
        /// <summary>
        /// Gets or sets the temporary properties.
        /// </summary>
        /// <value>
        /// The temporary data.
        /// </value>
        public IDictionary<string, object> TempProperties { get; set; }

        /// <summary>
        /// Gets or sets the current object.
        /// </summary>
        /// <value>
        /// The current object.
        /// </value>
        public object CurrentObject { get; set; }

        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        /// <value>
        /// The index.
        /// </value>
        public int Index { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NewRelic.Platform.Sdk.Processors
{
    /// <summary>
    /// A general purpose interface for processing metric values
    /// </summary>
    public interface IProcessor
    {
        /// <summary>
        /// Process a value for metric reporting
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        float? Process(float? val);
    }
}

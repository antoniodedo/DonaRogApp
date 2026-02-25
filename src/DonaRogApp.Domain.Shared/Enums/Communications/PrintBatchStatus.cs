using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonaRogApp.Enums.Communications
{
    /// <summary>
    /// Print batch workflow status
    /// </summary>
    public enum PrintBatchStatus
    {
        /// <summary>
        /// Draft - batch created but not ready for generation
        /// </summary>
        Draft = 1,

        /// <summary>
        /// Ready - batch validated and ready for PDF generation
        /// </summary>
        Ready = 2,

        /// <summary>
        /// Generating - PDF generation in progress (for large batches)
        /// </summary>
        Generating = 3,

        /// <summary>
        /// Generated - PDF created and ready for download
        /// </summary>
        Generated = 4,

        /// <summary>
        /// Downloaded - PDF downloaded by operator
        /// </summary>
        Downloaded = 5,

        /// <summary>
        /// Printed - physical printing completed
        /// </summary>
        Printed = 6,

        /// <summary>
        /// Cancelled - batch cancelled before printing
        /// </summary>
        Cancelled = 99
    }
}

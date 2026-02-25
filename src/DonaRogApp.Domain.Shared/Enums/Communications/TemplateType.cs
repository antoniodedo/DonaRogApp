using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonaRogApp.Enums.Communications
{
    /// <summary>
    /// Type of letter template format
    /// </summary>
    public enum TemplateType
    {
        /// <summary>
        /// HTML template with WYSIWYG editor
        /// Content stored in database, rendered to PDF via DinkToPdf
        /// </summary>
        Html = 1,

        /// <summary>
        /// Microsoft Word document (.docx) with merge fields
        /// Uploaded file converted to HTML via Mammoth, then rendered to PDF
        /// </summary>
        Docx = 2
    }
}

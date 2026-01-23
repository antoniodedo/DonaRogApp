using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonaRogApp.Enums.Donors
{
    /// <summary>
    /// Type of donor subject.
    /// Determines which fields are required and available.
    /// </summary>
    public enum SubjectType
    {
        /// <summary>
        /// Individual person (Persona Fisica)
        /// </summary>
        Individual = 1,

        /// <summary>
        /// Organization/Company (Persona Giuridica)
        /// </summary>
        Organization = 2
    }
}

using System.Collections.Generic;
using LMS.Shared.DTOs.Activity;
using LMS.Shared.DTOs.Document;

namespace LMS.Shared.DTOs.Module
{
    /// <summary>
    /// Extended module information including related entities.
    /// Used for retrieving detailed module view with activities and documents.
    /// Inherits basic module properties from ModuleDto.
    /// </summary>
    public record ModuleDetailDto : ModuleDto
    {
        /// <summary>
        /// List of activities belonging to this module.
        /// Populated from the ACTIVITY table where ModuleId matches this module.
        /// </summary>
        public List<ActivityDto> Activities { get; set; } = new();

        /// <summary>
        /// List of documents associated with this module.
        /// Populated from the DOCUMENT table where ModuleId matches this module.
        /// </summary>
        public List<DocumentDto> Documents { get; set; } = new();
    }
}
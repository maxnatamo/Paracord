using System.Diagnostics.CodeAnalysis;

namespace Paracord.Core.Controller
{
    /// <summary>
    /// Defines the different types of segments, that can occur in a route selector.
    /// </summary>
    public enum ControllerRouteSegmentType
    {
        /// <summary>
        /// Defines the segment as being constant.
        /// </summary>
        Constant,

        /// <summary>
        /// Defines the segment as being variable and can be substituted by any matching value.
        /// </summary>
        Variable,
    }

    /// <summary>
    /// Defines a single segment in a route selector.
    /// </summary>
    public class ControllerRouteSegment
    {
        /// <summary>
        /// The name of the segment.
        /// If <see cref="ControllerRouteSegment.Type" /> is <see cref="ControllerRouteSegmentType.Constant" />, then it indicates the constant value.
        /// If <see cref="ControllerRouteSegment.Type" /> is <see cref="ControllerRouteSegmentType.Variable" />, then it indicates the name of the segment.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The type of segment.
        /// </summary>
        public ControllerRouteSegmentType Type { get; set; } = ControllerRouteSegmentType.Constant;

        /// <summary>
        /// The optional default value for the segment. Only applicable for variable segments.
        /// </summary>
        public string? Default { get; set; } = null;
    }
}
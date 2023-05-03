using System;
using System.Collections.Generic;
using System.Text;

namespace Tramy.Common.Locations
{
    /// <summary>
    /// Type of polygons
    /// </summary>
    public enum PolygonType
    {
        /// <summary>
        /// No type
        /// </summary>
        None, 
        
        /// <summary>
        /// Polygon is a room
        /// </summary>
        Room, 
        
        /// <summary>
        /// Polygon is a wall
        /// </summary>
        Wall, 
        
        /// <summary>
        /// Polygon is a window
        /// </summary>
        Window
    }
}

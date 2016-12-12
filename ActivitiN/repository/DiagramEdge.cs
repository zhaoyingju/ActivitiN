using System.Collections.Generic;

namespace org.activiti.engine.repository
{


    /// <summary>
    /// ´æ´¢Í¼±ßÔµµÄÂ·µã¡£
    /// </summary>
    public class DiagramEdge : DiagramElement
    {
        private IList<DiagramEdgeWaypoint> waypoints;

        public DiagramEdge()
        {
        }

        public DiagramEdge(string id, IList<DiagramEdgeWaypoint> waypoints) : base(id)
        {
            this.waypoints = waypoints;
        }

        public override bool Node
        {
            get
            {
                return false;
            }
        }

        public override bool Edge
        {
            get
            {
                return true;
            }
        }

        public virtual IList<DiagramEdgeWaypoint> Waypoints
        {
            get
            {
                return waypoints;
            }
            set
            {
                this.waypoints = value;
            }
        }
    }
}
using System;
using System.Collections.Generic;

namespace org.activiti.engine.impl.pvm.process
{

    [Serializable]
    public class LaneSet
    {

        protected internal string id;
        protected internal IList<Lane> lanes;
        protected internal string name;

        public virtual string Id
        {
            set
            {
                this.id = value;
            }
            get
            {
                return id;
            }
        }

        public virtual string Name
        {
            get
            {
                return name;
            }
            set
            {
                this.name = value;
            }
        }

        public virtual IList<Lane> Lanes
        {
            get
            {
                if (lanes == null)
                {
                    lanes = new List<Lane>();
                }
                return lanes;
            }
        }

        public virtual void addLane(Lane laneToAdd)
        {
            Lanes.Add(laneToAdd);
        }

        public virtual Lane getLaneForId(string id)
        {
            if (lanes != null && lanes.Count > 0)
            {
                foreach (Lane lane in lanes)
                {
                    if (id.Equals(lane.Id))
                    {
                        return lane;
                    }
                }
            }
            return null;
        }
    }
}
using System;

namespace org.activiti.engine.impl.pvm.process
{

    [Serializable]
    public class ParticipantProcess : HasDIBounds
    {
        protected internal string id;
        protected internal string name;
        protected internal int x = -1;
        protected internal int y = -1;
        protected internal int width = -1;
        protected internal int height = -1;

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


        public virtual int X
        {
            get
            {
                return x;
            }
            set
            {
                this.x = value;
            }
        }


        public virtual int Y
        {
            get
            {
                return y;
            }
            set
            {
                this.y = value;
            }
        }


        public virtual int Width
        {
            get
            {
                return width;
            }
            set
            {
                this.width = value;
            }
        }


        public virtual int Height
        {
            get
            {
                return height;
            }
            set
            {
                this.height = value;
            }
        }

    }
}
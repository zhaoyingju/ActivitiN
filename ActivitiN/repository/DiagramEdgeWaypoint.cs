using System;

namespace org.activiti.engine.repository
{


    /// <summary>
    ///�洢ͼ��Ե�ĺ����λ�á�
    /// </summary>
    [Serializable]
    public class DiagramEdgeWaypoint
    {
        private double? x = null;
        private double? y = null;

        public double? X
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

        public double? Y
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
    }
}
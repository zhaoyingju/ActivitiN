using System;

namespace org.activiti.engine.identity
{
    [Serializable]
    public class Picture
    {

        protected internal sbyte[] bytes;
        protected internal string mimeType;

        public Picture(sbyte[] bytes, string mimeType)
        {
            this.bytes = bytes;
            this.mimeType = mimeType;
        }

        public virtual sbyte[] Bytes
        {
            get
            {
                return bytes;
            }
        }

        public virtual InputStream InputStream
        {
            get
            {
                return new ByteArrayInputStream(bytes);
            }
        }

        public virtual string MimeType
        {
            get
            {
                return mimeType;
            }
        }
    }

}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace InkDesktop
{
    [DataContract]
    public class SlideShowSettings
    {
        [DataMember]
        public ProductSlideShowSettings[] Items;
    }

    [DataContract]
    public class ProductSlideShowSettings
    {
        [DataMember]
        public string Vendor;

        [DataMember]
        public string ProductModel;

        [DataMember]
        public ushort Vid;

        [DataMember]
        public ushort Pid;

        [DataMember]
        public string[] ImageList;

        [DataMember]
        public int Interval; 
    }
}

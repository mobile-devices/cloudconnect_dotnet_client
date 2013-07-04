using System;
using System.Collections.Generic;
using System.Text;
using MD.CloudConnect.Data;

namespace MD.CloudConnect.Api
{
    public class Asset : AbstractApi<AssetData>
    {
        public override string Function
        {
            get { return "assets"; }
        }
    }
}

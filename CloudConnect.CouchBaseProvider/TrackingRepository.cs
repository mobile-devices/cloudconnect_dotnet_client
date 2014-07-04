using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudConnect.CouchBaseProvider
{
    public class TrackingRepository : RepositoryBase<Track>, WebDemo.AbstractModel.ITrackingRepository
    {
        private static int GenerateKey(DateTime date)
        {
            return (int)(date.Year * 10000 + date.Month * 100 + date.Day);
        }

        public IEnumerable<WebDemo.AbstractModel.Track> GetData(WebDemo.AbstractModel.Device device, DateTime date)
        {
            var view = GetView("all_by_asset_and_datekey");
            
            int begindatekey = GenerateKey(date);
            int enddatekey = begindatekey + 1;

            view.StartKey(String.Format("['{0}', {1}]", device.Imei, begindatekey));
            view.StartKey(String.Format("['{0}', {1}]", device.Imei, enddatekey));

            return view;
        }

        public void Create(WebDemo.AbstractModel.Track item)
        {
            base.Create((Track)item);
        }

        public void Delete(string id)
        {
            base.Delete(id);
        }

        public void Update(WebDemo.AbstractModel.Track item)
        {
            base.Update((Track)item);
        }

        public void Update(IList<WebDemo.AbstractModel.Track> items)
        {
            throw new NotImplementedException();
        }

        public IList<WebDemo.AbstractModel.Track> GetAll(int limit = 0)
        {
            return base.GetAll(limit) as IList<WebDemo.AbstractModel.Track>;
        }

        public WebDemo.AbstractModel.Track Get(string id)
        {
            return this.Get(id);
        }

        public WebDemo.AbstractModel.Track Build()
        {
            return new Track();
        }


        public void Create(IList<WebDemo.AbstractModel.Track> items)
        {
            foreach (Track track in items)
                base.Create(track);
        }
    }
}

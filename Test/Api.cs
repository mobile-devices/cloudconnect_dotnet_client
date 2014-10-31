using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Test
{
    [TestClass]
    public class Api
    {
        private MD.CloudConnect.CloudConnect cc = null;

        [TestInitialize]
        public void Init()
        {
            cc = new MD.CloudConnect.CloudConnect(Properties.Settings.Default.env, Properties.Settings.Default.account, Properties.Settings.Default.token);
        }
        [TestCategory("Global Api")]
        [TestMethod]
        public void ValideCloudUrl()
        {
            Assert.AreEqual(cc.WebRequestManager.CloudUrl, Properties.Settings.Default.cloudurl);
        }

        [TestCategory("Assets")]
        [TestMethod]
        public void GetAssets()
        {
            List<MD.CloudConnect.Data.AssetData> assets = cc.Asset.Get();
            Assert.AreNotEqual(assets.Count, 0);
        }
        [TestCategory("Assets")]
        [TestMethod]
        public void ReadImeiFirtAsset()
        {
            MD.CloudConnect.Data.AssetData asset = cc.Asset.Get()[0];
            Assert.AreNotEqual(asset.Imei, String.Empty);
        }

        [TestCategory("Tracks")]
        [TestMethod]
        public void GetTracks()
        {
            List<MD.CloudConnect.Data.TrackingData> tracks = cc.Track.Get();
            Assert.AreNotEqual(tracks.Count, 0);
        }

        [TestCategory("Messages")]
        [TestMethod]
        public void GetMessages()
        {
            List<MD.CloudConnect.Data.MessageData> messages = cc.Message.Get();
            Assert.AreNotEqual(messages.Count, 0);
        }

        [TestCategory("Fields")]
        [TestMethod]
        public void GetFields()
        {
            List<MD.CloudConnect.Data.FieldData> fields = cc.Field.Get();
            Assert.AreNotEqual(fields.Count, 0);
        }

        [TestCategory("Channels")]
        [TestMethod]
        public void GetChannels()
        {
            List<MD.CloudConnect.Data.ChannelData> channels = cc.Channel.Get();
            Assert.AreNotEqual(channels.Count, 0);
        }
    }
}

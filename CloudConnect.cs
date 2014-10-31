using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MD.CloudConnect.Api;

namespace MD.CloudConnect
{
    public class CloudConnect
    {
        private MDWebRequest _webRequest = null;
        public MDWebRequest WebRequestManager
        {
            get { return _webRequest; }
        }

        private Asset _asset = null;
        public Asset Asset
        {
            get
            {
                return _asset;
            }
        }

        private Track _track = null;
        public Track Track
        {
            get
            {
                return _track;
            }
        }

        private Message _message = null;
        public Message Message
        {
            get
            {
                return _message;
            }
        }

        private Field _field = null;
        public Field Field
        {
            get
            {
                return _field;
            }
        }

        private Channel _channel = null;
        public Channel Channel
        {
            get
            {
                return _channel;
            }
        }

        public CloudConnect(string environment, string account, string token)
        {
            _webRequest = new MDWebRequest(account, environment, token);
            _asset = new Asset() { WebRequest = _webRequest, Cloud = this };
            _track = new Track() { WebRequest = _webRequest, Cloud = this };
            _message = new Message() { WebRequest = _webRequest, Cloud = this };
            _field = new Field() { WebRequest = _webRequest, Cloud = this };
            _channel = new Channel() { WebRequest = _webRequest, Cloud = this };
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MD.CloudConnect.Api;

namespace MD.CloudConnect
{
    public class CloudConnect
    {
        private MDWebRequest _webRequest = null;

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
        public CloudConnect(string environment, string account, string token)
        {
            _webRequest = new MDWebRequest() { Account = account, Environment = environment, Token = token };
            _asset = new Asset() { WebRequest = _webRequest };
            _track = new Track() { WebRequest = _webRequest };
        }
    }
}

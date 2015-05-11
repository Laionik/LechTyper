using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
﻿using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace LechTyper.OAuth
{
    public class OAuthMessageHandler : DelegatingHandler
    {
        private static string _consumerKey = "EpfmvWrsmtncANw5Rfsn4xMQ3";
        private static string _consumerSecret = "0gdIryMSHwVYMvyx8Jj1K0ncx71Apjep0JHOpSuBVfRw1owSMQ";
        private static string _token = "860293476-YhCa0rqGEM5i9d2HiOln6NqVhdOgKFT3T2Kg1h2v";
        private static string _tokenSecret = "3TgMumaP6EBXH5VqhndHGhfSsu2XrAHccarkppfiK9WRj";

        private OAuthBase _oAuthBase = new OAuthBase();

        public OAuthMessageHandler(HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string normalizedUri;
            string normalizedParameters;
            string authHeader;

            string signature = _oAuthBase.GenerateSignature(
                request.RequestUri,
                _consumerKey,
                _consumerSecret,
                _token,
                _tokenSecret,
                request.Method.Method,
                _oAuthBase.GenerateTimeStamp(),
                _oAuthBase.GenerateNonce(),
                out normalizedUri,
                out normalizedParameters,
                out authHeader);

            request.Headers.Authorization = new AuthenticationHeaderValue("OAuth", authHeader);
            return base.SendAsync(request, cancellationToken);
        }
    }
}
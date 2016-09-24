﻿using System;
using System.Net;
using System.Threading.Tasks;

namespace Storm
{
    public static class WebRequestExt
    {
        public static WebResponse GetResponseExt(this WebRequest request)
        {
            if (request == null) { throw new ArgumentNullException(nameof(request)); }

            WebResponse webResp = null;

            try
            {
                webResp = request.GetResponse();
            }
            catch (WebException ex)
            {
                Utils.LogException(ex, request.RequestUri.AbsoluteUri);

                webResp = ex?.Response;
            }

            return webResp;
        }

        public static async Task<WebResponse> GetResponseAsyncExt(this WebRequest request)
        {
            if (request == null) { throw new ArgumentNullException(nameof(request)); }

            WebResponse webResp = null;

            try
            {
                webResp = await request.GetResponseAsync().ConfigureAwait(false);
            }
            catch (WebException ex)
            {
                Utils.LogException(ex, request.RequestUri.AbsoluteUri);

                webResp = ex?.Response;
            }

            return webResp;
        }
    }
}
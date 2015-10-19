using System;
using System.Collections.Generic;
using System.IO;
//using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Web.Http;
using Windows.Web.Http.Filters;



namespace jnrainbbs
{
    class httputils
    {
        public Windows.Web.Http.Headers.HttpResponseHeaderCollection responseHeaders { get; set; }
        public CancellationTokenSource cts = new CancellationTokenSource();

        public async Task<string> PostStringAsync(string link, string param)
        {

            try
            {

                System.Diagnostics.Debug.WriteLine(param);

                Uri uri = new Uri(link);

                HttpClient httpClient = new HttpClient();


                HttpStringContent httpStringContent = new HttpStringContent(param, Windows.Storage.Streams.UnicodeEncoding.Utf8,"application/x-www-form-urlencoded"); //,Windows.Storage.Streams.UnicodeEncoding.Utf8
                
                HttpResponseMessage response = await httpClient.PostAsync(uri,

                                                       httpStringContent).AsTask(cts.Token);
                responseHeaders = response.Headers;
                System.Diagnostics.Debug.WriteLine(responseHeaders);

                string responseBody = await response.Content.ReadAsStringAsync().AsTask(cts.Token);
                return responseBody;
            }
            catch(Exception e)
            {
                return "Error：" + e.Message;
            }

        }

        public async Task<string> GetAsync(string link)
        {
            try
            {
                Uri uri = new Uri(link);

                HttpClient httpClient = new HttpClient();

                //防止相同链接不进行刷新
                httpClient.DefaultRequestHeaders.IfModifiedSince = new DateTimeOffset(DateTime.Now);

                // 获取网络的返回的字符串数据
                string result = await httpClient.GetStringAsync(uri);
                return result;
            }
            catch (Exception e)
            {
                return "Error："+e.Message;
            }
        }

        public async Task<string> PostStringAsync2(string link, List<KeyValuePair<string, string>> values)
        {

            var httpClient = new System.Net.Http.HttpClient(new System.Net.Http.HttpClientHandler());
            System.Net.Http.HttpResponseMessage response = await httpClient.PostAsync(new Uri(link), new System.Net.Http.FormUrlEncodedContent(values));
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine(response.Headers);
            return responseString;
        }


        public async Task<string> ImageToStream(byte[] image)
        {
            try
            {


                System.Diagnostics.Debug.WriteLine(image.Length);
                using (var client = new System.Net.Http.HttpClient())
                {
                    using (var content =
                        new System.Net.Http.MultipartFormDataContent())
                    {
                        content.Add(new System.Net.Http.StreamContent(new MemoryStream(image)), "file", "upload.jpg");

                        using (
                           var message =
                               await client.PostAsync("http://bbs.jiangnan.edu.cn/attachments/upload.php", content))
                        {
                            message.EnsureSuccessStatusCode();
                            string finalresults = await message.Content.ReadAsStringAsync();
                            System.Diagnostics.Debug.WriteLine(finalresults);
                            return finalresults;


                        }
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return e.Message;
            }
        }

        public async Task<string> UploadImage2(byte[] image, string url)
        {
            Stream stream = new System.IO.MemoryStream(image);
            HttpStreamContent streamContent = new HttpStreamContent(stream.AsInputStream());

            Uri resourceAddress = null;
            Uri.TryCreate(url.Trim(), UriKind.Absolute, out resourceAddress);
            Windows.Web.Http.HttpRequestMessage request = new Windows.Web.Http.HttpRequestMessage(Windows.Web.Http.HttpMethod.Post, resourceAddress);
            request.Content = streamContent;

            var httpClient = new Windows.Web.Http.HttpClient();
            var cts = new CancellationTokenSource();
            Windows.Web.Http.HttpResponseMessage response = await httpClient.SendRequestAsync(request).AsTask(cts.Token);
            response.EnsureSuccessStatusCode();
            string result =await response.Content.ReadAsStringAsync();
            return result;
        }

        public async Task<string> upload(string uri, string fileuri)
        {


            HttpClient httpClient = new HttpClient();

            StorageFile file1 = await StorageFile.GetFileFromApplicationUriAsync(new Uri(fileuri, UriKind.Absolute));
            //StorageFile file1 = await StorageFile.;
            using (IRandomAccessStreamWithContentType stream1 = await file1.OpenReadAsync())
            {
                HttpStreamContent streamContent1 = new HttpStreamContent(stream1);
                streamContent1.Headers.ContentType = new Windows.Web.Http.Headers.HttpMediaTypeHeaderValue("image/jpeg");
                
                //Stream stream = new System.IO.MemoryStream(image);
                //long length = stream.Length;
                //HttpStreamContent streamContent = new HttpStreamContent(stream.AsInputStream());
                //HttpStreamContent streamContent = new HttpStreamContent(image.AsInputStream());

                //HttpStringContent stringContent = new HttpStringContent("file=", Windows.Storage.Streams.UnicodeEncoding.Utf8, "multipart/form-data");
                // streamContent.Headers.ContentType =new Windows.Web.Http.Headers.HttpMediaTypeHeaderValue("image/jpeg");
                //streamContent.Headers.ContentLength = (ulong)image.Length;
                ulong length2;
                var ll=streamContent1.TryComputeLength(out length2);
                if (ll == true)
                {
                    streamContent1.Headers.ContentLength = length2;
                }
                 System.Diagnostics.Debug.WriteLine(streamContent1.Headers.ContentLength);
                HttpMultipartFormDataContent hmfdc = new HttpMultipartFormDataContent();
                //HttpMultipartContent hmc = new HttpMultipartContent();
                //hmfdc.Headers.ContentType = new Windows.Web.Http.Headers.HttpMediaTypeHeaderValue("image/jpeg");
                hmfdc.Add(streamContent1, "file", "file1.jpg");
                //hmfdc.Add(streamContent, "file", filename);
                //hmc.Add(streamContent);
                //hmfdc.Add(stringContent);
                HttpResponseMessage response = await httpClient.PostAsync(new Uri(uri),

                                                       hmfdc).AsTask(cts.Token);

                string responseBody = await response.Content.ReadAsStringAsync().AsTask(cts.Token);
                System.Diagnostics.Debug.WriteLine(responseBody);
                return responseBody;
            }
        }
    }


}

using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Text;
using System.Diagnostics;
using Windows.Foundation;
using System.Windows.Media.Imaging;
using System.Threading.Tasks;

namespace weixin
{
    public class PostClient
    {
        #region Members

        /// <summary>
        /// Post data query string.
        /// </summary>
        StringBuilder _postData = new StringBuilder();
        byte[] bytes;

        #endregion

        #region Events

        /// <summary>
        /// Event handler for DownloadStringCompleted event.
        /// </summary>
        /// <param name="sender">Object firing the event.</param>
        /// <param name="e">Argument holding the data downloaded.</param>
        public delegate void DownloadStringCompletedHandler(object sender, DownloadStringCompletedEventArgs e);

        /// <summary>
        /// Occurs when an asynchronous resource-download operation is completed.
        /// </summary>
        public event DownloadStringCompletedHandler DownloadStringCompleted;

        #endregion

        #region Constructors


        /// <summary>
        /// Creates a new instance of PostClient with the specified parameters.
        /// </summary>
        /// <param name="data">POST parameters in string format. Valid string format is something like "id=120&amp;name=John".</param>
        public PostClient(string parameters)
        {
            _postData.Append(parameters);
            bytes = Encoding.UTF8.GetBytes(parameters); 
        }


        /// <summary>
        /// Creates a new instance of PostClient with the specified parameters.
        /// </summary>
        /// <param name="data">POST parameters in Stream format.</param>
        public PostClient(Byte[] parameters)
        {
            bytes = parameters;
        }


        public Byte[] Bytes
        {
            get
            {
                return bytes;
            }
        }

        /// <summary>
        /// Creates a new instance of PostClient with the specified parameters.
        /// </summary>
        /// <param name="parameters">POST parameters as a list of string. Valid list elements are "id=120" and "name=John".</param>
        public PostClient(IList<string> parameters)
        {
            foreach (var element in parameters)
            {
                _postData.Append(string.Format("{0}&", element));
            }

        }

        /// <summary>
        /// Creates a new instance of PostClient with the specified parameters.
        /// </summary>
        /// <param name="parameters">POST parameters as a dictionary with string keys and values. Valid elements could have keys "id" and "name" with values "120" and "John" respectively.</param>
        public PostClient(IDictionary<string, string> parameters)
        {
            foreach (var pair in parameters)
            {
                _postData.Append(string.Format("{0}={1}&", pair.Key, pair.Value));
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Downloads the resource at the specified Uri as a string.
        /// </summary>
        /// <param name="address">The location of the resource to be downloaded.</param>
        public void DownloadStringAsync(Uri address)
        {
            HttpWebRequest request;

            try
            {
                request = (HttpWebRequest)WebRequest.Create(address);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";

               IAsyncResult asyncResult=(IAsyncResult) request.BeginGetRequestStream(new AsyncCallback(RequestReady), request);
            }
            catch
            {
                if (DownloadStringCompleted != null)
                {
                    System.Windows.Deployment.Current.Dispatcher.BeginInvoke(delegate()
                    {
                        DownloadStringCompleted(this, new DownloadStringCompletedEventArgs(new Exception("Error creating HTTP web request.")));
                    });
                }
            }
        }

        #endregion

        #region Protected methods

        protected void RequestReady(IAsyncResult asyncResult)
        {
            HttpWebRequest request =asyncResult.AsyncState as HttpWebRequest;

            using (Stream stream =request.EndGetRequestStream(asyncResult) )
            {
                stream.Position = 0;
                stream.Write(bytes, 0, bytes.Length);
                stream.Close();


                //注释using (StreamWriter writer = new StreamWriter(stream))
                //{
                //    string _post = _postData.ToString();
                //    _post = _post.Substring(0, _post.Length - 1);
                //    writer.Write(_post.ToString());
                //    Debug.WriteLine(_post);
                //    writer.Flush();
                //}


            }

            request.BeginGetResponse(new AsyncCallback(ResponseReady), request);
        }


        protected void ResponseReady(IAsyncResult asyncResult)
        {
            HttpWebRequest request = asyncResult.AsyncState as HttpWebRequest;
            HttpWebResponse response = null;

            try
            {
                response = (HttpWebResponse)request.EndGetResponse(asyncResult);

                Byte[] buffer = new Byte[bytes.Length];
                using (Stream responseStream = response.GetResponseStream())
                {
                    responseStream.Position = 0;
                    responseStream.Read(buffer, 0, buffer.Length);
                    //responseStream.Read(bytes, 0, bytes.Length);
                    responseStream.Close();


                    //注释    using (StreamReader reader = new StreamReader(responseStream))
                    //    {
                    //        result = reader.ReadToEnd();
                    //    }
                }


                if (DownloadStringCompleted != null)
                {
                    System.Windows.Deployment.Current.Dispatcher.BeginInvoke(delegate()
                    {
                        DownloadStringCompleted(this, new DownloadStringCompletedEventArgs(buffer));
                    });
                }
            }
            catch
            {
                if (DownloadStringCompleted != null)
                {
                    System.Windows.Deployment.Current.Dispatcher.BeginInvoke(delegate()
                    {
                        DownloadStringCompleted(this, new DownloadStringCompletedEventArgs(new Exception("Error getting HTTP web response.")));
                    });
                }
            }
        }

        #endregion
    }

    public class DownloadStringCompletedEventArgs : EventArgs
    {
        #region Properties

        /// <summary>
        /// Gets the data that is downloaded by a DownloadStringAsync method.
        /// </summary>
        public string stringResult { get; private set; }
        public Byte[] bytesResult { get; private set; }

        /// <summary>
        /// Gets a value that indicates which error occurred during an asynchronous operation.
        /// </summary>
        public Exception Error { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of DownloadStringCompletedEventArgs with the specified result data.
        /// </summary>
        /// <param name="result">The data that is downloaded by a DownloadStringAsync method.</param>
        public DownloadStringCompletedEventArgs(string result)
        {
            stringResult = result;
        }


        /// <summary>
        /// Creates a new instance of DownloadStringCompletedEventArgs with the specified result data.
        /// </summary>
        /// <param name="result">The data that is downloaded by a DownloadStringAsync method.</param>
        public DownloadStringCompletedEventArgs(Byte[] Buffer)
        {
            bytesResult = Buffer;
        }

        /// <summary>
        /// Creates a new instance of DownloadStringCompletedEventArgs with the specified exception.
        /// </summary>
        /// <param name="ex">The exception generated by the asynchronous operation.</param>
        public DownloadStringCompletedEventArgs(Exception ex)
        {
            Error = ex;
        }
        #endregion
    }
 }


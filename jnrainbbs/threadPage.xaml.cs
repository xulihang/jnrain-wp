using jnrainbbs.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “基本页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍

namespace jnrainbbs
{
    /// <summary>
    /// 可独立使用或用于导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class threadPage : Page
    {
        public int pageNum { get; set; }
        private string myUrl { get; set; }
        public myParam2 myparam { get; set; }
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public threadPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        /// <summary>
        /// 获取与此 <see cref="Page"/> 关联的 <see cref="NavigationHelper"/>。
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// 获取此 <see cref="Page"/> 的视图模型。
        /// 可将其更改为强类型视图模型。
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// 使用在导航过程中传递的内容填充页。  在从以前的会话
        /// 重新创建页时，也会提供任何已保存状态。
        /// </summary>
        /// <param name="sender">
        /// 事件的来源; 通常为 <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">事件数据，其中既提供在最初请求此页时传递给
        /// <see cref="Frame.Navigate(Type, Object)"/> 的导航参数，又提供
        /// 此页在以前会话期间保留的状态的
        /// 字典。 首次访问页面时，该状态将为 null。</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            myparam = (myParam2)e.NavigationParameter;
            pageNum = 1;
            loadThread();
        }

        /// <summary>
        /// 保留与此页关联的状态，以防挂起应用程序或
        /// 从导航缓存中放弃此页。值必须符合
        /// <see cref="SuspensionManager.SessionState"/> 的序列化要求。
        /// </summary>
        /// <param name="sender">事件的来源；通常为 <see cref="NavigationHelper"/></param>
        ///<param name="e">提供要使用可序列化状态填充的空字典
        ///的事件数据。</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper 注册

        /// <summary>
        /// 此部分中提供的方法只是用于使
        /// NavigationHelper 可响应页面的导航方法。
        /// <para>
        /// 应将页面特有的逻辑放入用于
        /// <see cref="NavigationHelper.LoadState"/>
        /// 和 <see cref="NavigationHelper.SaveState"/> 的事件处理程序中。
        /// 除了在会话期间保留的页面状态之外
        /// LoadState 方法中还提供导航参数。
        /// </para>
        /// </summary>
        /// <param name="e">提供导航方法数据和
        /// 无法取消导航请求的事件处理程序。</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion


        private async void backwardBtn_Click(object sender, RoutedEventArgs e)
        {
            if (pageNum == 1)
            {
                MessageDialog md = new MessageDialog("已经是第一页了");
                await md.ShowAsync();
            }
            else
            {
                pageNum = pageNum - 1;
                loadThread();
            }
        }

        private void forwardBtn_Click(object sender, RoutedEventArgs e)
        {
            pageNum = pageNum + 1;
            loadThread();
        }

        private void settingBtn_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(settingPage));
        }

        private async void loadThread()
        {
            string response;
            httputils myhttputils = new httputils();
            response = await myhttputils.GetAsync("http://bbs.jiangnan.edu.cn/rainstyle/thread_json.php?boardName=" + myparam.threadboard + "&ID=" + myparam.threadid +"&page="+pageNum);

            //string filename = myparam.threadboard + "-" + myparam.threadid + "-page1" + ".html";
            System.Diagnostics.Debug.WriteLine(response);
            //System.Diagnostics.Debug.WriteLine(filename);

            if (response.Contains("Error："))
            {
                MessageDialog md = new MessageDialog("连接错误");
                await md.ShowAsync();
            }
            else if (response.Contains("{\"status\":0") == false)
            {
                MessageDialog md = new MessageDialog("网站数据错误");
                await md.ShowAsync();
            }
            else
            {

                JsonObject contentJson = JsonObject.Parse(response);

                JsonArray contentArray = contentJson["posts"].GetArray();
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

                string myHtml;
                myHtml = "<!DOCTYPE html>\n";
                myHtml = myHtml + "<html lang=\"en\" xmlns=\"http://www.w3.org/1999/xhtml\">\n";
                myHtml = myHtml + "<head>\n";
                myHtml = myHtml + "<meta charset=\"utf-8\" />\n";
                myHtml = myHtml + "<title>" + myparam.threadname + "</title>\n";
                myHtml = myHtml + "<link rel=\"stylesheet\" type=\"text/css\" href=\"ms-appx-web:///Assets/mystyle.css\" />";
                if (localSettings.Values.ContainsKey("IfOverrideLink") == true)
                {
                    Object value = localSettings.Values["IfOverrideLink"];
                    if (value.ToString() == "true")
                    {
                        myHtml = myHtml + "<script src=\"ms-appx-web:///Assets/jquery-1.11.3.min.js\" type=\"text/javascript\" language=\"javascript\"></script>";
                        myHtml = myHtml + "<script src=\"ms-appx-web:///Assets/myjs.js\" type=\"text/javascript\" language=\"javascript\"></script>";
                    }
                }

                myHtml = myHtml + "</head>\n";
                myHtml = myHtml + "<body>\n";
                myHtml = myHtml + "<h1>" + myparam.threadname + "</h1>";
                foreach (var single in contentArray)
                {
                    myHtml = myHtml + "<div>\n";
                    string singleThreadHtml = single.GetObject()["content"].GetString();
                    myHtml = myHtml + singleThreadHtml;
                    myHtml = myHtml + "</div>\n";
                    myHtml = myHtml + "<hr />\n";

                }
                myHtml = myHtml + "</body>\n";
                myHtml = myHtml + "</html>\n";
                myHtml = myHtml.Replace("js/xheditor-1.1.9/xheditor_emot/default/", "ms-appx-web:///Assets/em/");
                myHtml = myHtml.Replace("emot/jn/", "ms-appx-web:///Assets/em/");
                myHtml = myHtml.Replace("downip.php", "http://bbs.jiangnan.edu.cn/rainstyle/bbscon.php");


                if (localSettings.Values.ContainsKey("IfLoadImg") == true)
                {
                    Object value2 = localSettings.Values["IfLoadImg"];
                    if (value2.ToString() == "true")
                    {
                        if (localSettings.Values.ContainsKey("imgQuality") == true)
                        {
                            Object value3 = localSettings.Values["imgQuality"];
                            if (value3.ToString() == "origin")
                            {
                                myHtml = myHtml.Replace("/attachments/", "http://bbs.jiangnan.edu.cn/attachments/");
                            }
                            else if (value3.ToString() == "mid")
                            {
                                myHtml = myHtml.Replace("/attachments/", "https://jnrainbbs-bookjnrain.rhcloud.com/attachments-mid/");
                            }
                            else
                            {
                                myHtml = myHtml.Replace("/attachments/", "https://jnrainbbs-bookjnrain.rhcloud.com/attachments-small/");
                            }
                        }
                        else
                        {
                            myHtml = myHtml.Replace("/attachments/", "http://bbs.jiangnan.edu.cn/attachments/");
                        }                        
                    }
                }
                System.Diagnostics.Debug.WriteLine(myHtml);

                /*StorageFolder d = await ApplicationData.Current.LocalFolder.CreateFolderAsync("folder", CreationCollisionOption.OpenIfExists);
                StorageFile htmlFile = await d.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(htmlFile, myHtml);
                System.Diagnostics.Debug.WriteLine(ApplicationData.Current.LocalFolder.Path);
                Uri filepath = new Uri("ms-appdata:///local/folder/"+filename);
                //WebView1.Navigate(filepath);*/
                WebView1.NavigateToString(myHtml);


            }
        }

        private void newpostBtn_Click(object sender, RoutedEventArgs e)
        {
            newpostPage.convalue = "";
            Frame.Navigate(typeof(newpostPage), myparam);
        }

        private void WebView1_ScriptNotify(object sender, NotifyEventArgs e)
        {
            string myurl = e.Value;
            System.Diagnostics.Debug.WriteLine(myurl);
            //example:http://bbs.jiangnan.edu.cn/wForum/disparticle.php?boardName=A.JnrainClub&ID=19956&pos=1
            
            if (myurl.Contains("disparticle"))
            {
                 myUrl = myurl;
                 showMsg();
            }

        }

        private async void showMsg()
        {

            var messageDialog = new MessageDialog("跳转到所点击的链接吗？");

            // Add commands and set their callbacks; both buttons use the same callback function instead of inline event handlers
            messageDialog.Commands.Add(new UICommand(
                "好的",
                new UICommandInvokedHandler(this.CommandInvokedHandler)));
            messageDialog.Commands.Add(new UICommand(
                "不用",
                new UICommandInvokedHandler(this.CommandInvokedHandler)));

            // Set the command that will be invoked by default
            messageDialog.DefaultCommandIndex = 0;

            // Set the command to be invoked when escape is pressed
            messageDialog.CancelCommandIndex = 1;

            // Show the message dialog
            await messageDialog.ShowAsync();

        }

        private void CommandInvokedHandler(IUICommand command)
        {
            string myurl = myUrl;
            switch (command.Label)
            {
                case "好的":
                    int ub, lb;
                    ub = myurl.IndexOf("boardName=") + 10;
                    lb = myurl.IndexOf("&ID=");
                    System.Diagnostics.Debug.WriteLine(ub);
                    System.Diagnostics.Debug.WriteLine(lb);
                    myparam.threadboard = myurl.Substring(ub, lb - ub);
                    System.Diagnostics.Debug.WriteLine(myparam.threadboard);

                    ub = myurl.IndexOf("&ID=") + 4;
                    lb = myurl.IndexOf("&pos=");
                    System.Diagnostics.Debug.WriteLine(ub);
                    System.Diagnostics.Debug.WriteLine(lb);
                    myparam.threadid = myurl.Substring(ub, lb - ub);
                    myparam.threadname = "";

                    System.Diagnostics.Debug.WriteLine(myparam.threadid);
                    pageNum = 1;
                    loadThread();
                    break;
                case "不用":
                    break;
                default:
                    break;
            }
        }


    }


}

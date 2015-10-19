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
    public sealed partial class MainPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public MainPage()
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
            List<string> boardList = new List<string>();
            boardList.Add("站务系统");
            boardList.Add("江南大学");
            boardList.Add("文化艺术");
            boardList.Add("电脑技术");
            boardList.Add("学术科学");
            boardList.Add("菁菁校园");
            boardList.Add("知性感性");
            boardList.Add("休闲娱乐");
            boardList.Add("社团群体");
            boardList.Add("校务邮箱");
            boardList.Add("服务专区");
            int i = 0;
            foreach (string boardname in boardList)
            {
                ListViewItem single = new ListViewItem();
                TextBlock singleText= new TextBlock();
                //single.Content = boardname;
                singleText.Text=boardname;
                singleText.FontSize = 32;
                if (i == 10)
                {
                    singleText.Tag = "A";
                }
                else
                { 
                    singleText.Tag = i;
                }
                single.Content = singleText;
                ListView1.Items.Add(single);
                i++;
            }
            loadtopten();

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

        private void ItemView2_ItemClick(object sender, ItemClickEventArgs e)
        {
            TextBlock clickedItem = (TextBlock)e.ClickedItem;

            Frame.Navigate(typeof(threadPage), clickedItem.Tag);

        }

        private void ItemView1_ItemClick(object sender, ItemClickEventArgs e)
        {
            TextBlock clickedItem = (TextBlock)e.ClickedItem;
            System.Diagnostics.Debug.WriteLine(clickedItem.Tag);
            myParam myparam = new myParam();
            myparam.boardid = clickedItem.Tag.ToString();
            myparam.boardname = clickedItem.Text;
            if (clickedItem.Text == "社团群体")
            {
                mannualParam mannualparam = new mannualParam();
                mannualparam.boardnameList = new List<string> { "听听广播" };
                mannualparam.boardidList = new List<string> { "broadcast" };
                mannualparam.boardname = clickedItem.Text;
                Frame.Navigate(typeof(boardPage), mannualparam);
            }
            else
            {
                Frame.Navigate(typeof(boardPage), myparam);
            }
            //System.Diagnostics.Debug.WriteLine(e.ClickedItem);

            

        }

        private async void loadtopten()
        {
            ListView2.Items.Clear();
            string response;
            httputils myhttputils = new httputils();
            response=await myhttputils.GetAsync("http://bbs.jiangnan.edu.cn/rainstyle/topten_json.php");
            if (response.Contains("Error："))
            {
                MessageDialog md = new MessageDialog("连接错误");
                await md.ShowAsync();
            }
            else
            {
                JsonObject myJson = JsonObject.Parse(response);
                JsonArray innerArray = myJson["posts"].GetArray();
                foreach (var postName in innerArray)
                {
                    string threadTitle = postName.GetObject()["title"].GetString();
                    string threadId = postName.GetObject()["id"].GetString();
                    string threadBoard = postName.GetObject()["board"].GetString();
                    ListViewItem single = new ListViewItem();
                    TextBlock singleText = new TextBlock();
                    //single.Content = boardname;                
                    myParam2 myparam = new myParam2();
                    myparam.threadboard = threadBoard;
                    myparam.threadid = threadId;
                    myparam.threadname = threadTitle;
                    singleText.Text = threadTitle;
                    singleText.Tag = myparam;
                    singleText.FontSize = 24;
                    singleText.TextWrapping = TextWrapping.Wrap;
                    single.Content = singleText;
                    ListView2.Items.Add(single);
                }
            }
        }

        private void settingBtn_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(settingPage));
        }

        private void refreshBtn_Click(object sender, RoutedEventArgs e)
        {
            loadtopten();
        }

        private void loginBtn_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(loginPage));
        }


    }

    public class myParam
    {
        public string boardid { get; set; }
        public string boardname { get; set; }
    }
    public class myParam2
    {
        public string threadid { get; set; }
        public string threadboard { get; set; }
        public string threadname { get; set; }
    }
}

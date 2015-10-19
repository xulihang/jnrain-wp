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
    public sealed partial class boardPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public boardPage()
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
        private  void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            if (e.NavigationParameter.GetType() == typeof(mannualParam))
            {
                mannualParam mannualparam = (mannualParam)e.NavigationParameter;
                loadManually(mannualparam);
            }
            else
            {
                myParam myparam = (myParam)e.NavigationParameter;
                loadBoardList(myparam);
            }

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

        private void ListView1_ItemClick(object sender, ItemClickEventArgs e)
        {
            myParam myparam=new myParam();
            TextBlock clickedItem = (TextBlock)e.ClickedItem;
            System.Diagnostics.Debug.WriteLine(clickedItem.Tag);

            myparam.boardid=clickedItem.Tag.ToString();
            myparam.boardname=clickedItem.Text;
            System.Diagnostics.Debug.WriteLine(myparam.boardid);

            mannualParam mannualparam = new mannualParam();
            mannualparam.boardnameList = new List<string>();
            mannualparam.boardidList = new List<string>();
            mannualparam.boardname = clickedItem.Text;

            switch (clickedItem.Tag.ToString())
            {
                case "A.JOB":

                    mannualparam.boardnameList.Add("官方勤工助学");
		            mannualparam.boardnameList.Add("家教信息");
		            mannualparam.boardnameList.Add("就业信息");
		            mannualparam.boardnameList.Add("其他兼职");
                    mannualparam.boardidList.Add("A.Work");
		            mannualparam.boardidList.Add("HomeTutor");
		            mannualparam.boardidList.Add("Jobs");
		            mannualparam.boardidList.Add("PartTime");
                    Frame.Navigate(typeof(boardPage),mannualparam);
                    break;

                case "A.SecondHand":
                    mannualparam.boardnameList.Add("美容服饰");
                    mannualparam.boardnameList.Add("图书市场");
                    mannualparam.boardnameList.Add("电脑数码");
                    mannualparam.boardnameList.Add("租房信息");
                    mannualparam.boardnameList.Add("交通工具");
                    mannualparam.boardnameList.Add("日用百货");
                    mannualparam.boardidList.Add("Beautylifestyle");
                    mannualparam.boardidList.Add("BookMarket");
                    mannualparam.boardidList.Add("DigiPCTrade");
                    mannualparam.boardidList.Add("RentingHouse");
                    mannualparam.boardidList.Add("Transport");
                    mannualparam.boardidList.Add("Z.riyongbaihuo");
                    Frame.Navigate(typeof(boardPage),mannualparam);
                    break;
                case "Agent":
                    mannualparam.boardnameList.Add("电脑数码代理");
                    mannualparam.boardnameList.Add("综合其他代理");
                    mannualparam.boardidList.Add("Digitalcomputers");
                    mannualparam.boardidList.Add("Z.OtherTrades");
                    Frame.Navigate(typeof(boardPage),mannualparam);
                    break;
                case "graduatestudent":
                    mannualparam.boardnameList.Add("考研资料交流");
                    mannualparam.boardnameList.Add("考研一族");
                    mannualparam.boardidList.Add("GCTbooktrade");
                    mannualparam.boardidList.Add("postgraduate");
                    Frame.Navigate(typeof(boardPage),mannualparam);
                    break;

                case "Sportsgroup":
                    mannualparam.boardnameList.Add("足球世界");
                    mannualparam.boardnameList.Add("NBA");
                    mannualparam.boardnameList.Add("体育运动");
                    mannualparam.boardidList.Add("football");
                    mannualparam.boardidList.Add("NBA");
                    mannualparam.boardidList.Add("Sports");
                    Frame.Navigate(typeof(boardPage),mannualparam);
                    break;
                default:
                    Frame.Navigate(typeof(ThreadListPage), myparam);
                    break;
            }

        }

        private void loadManually(mannualParam mannualparam)
        {
            secName.Text = mannualparam.boardname;
            int i = 0;
            foreach (string boardname in mannualparam.boardnameList)
            {
                ListViewItem single = new ListViewItem();
                TextBlock singleText = new TextBlock();
                
                singleText.Text = boardname;
                singleText.FontSize = 32;
                singleText.Tag = mannualparam.boardidList[i];
                single.Content = singleText;
                ListView1.Items.Add(single);
                i++;
            }

        }

        private async void loadBoardList(myParam myparam)
        {

            secName.Text = myparam.boardname;
            string response;
            httputils myhttputils = new httputils();
            response = await myhttputils.GetAsync("http://bbs.jiangnan.edu.cn/rainstyle/boards_json.php?sec=" + myparam.boardid);
            System.Diagnostics.Debug.WriteLine(response);
            if (response.Contains("Error："))
            {
                MessageDialog md = new MessageDialog("连接错误");
                await md.ShowAsync();
            }
            else
            {
                JsonObject boardlistJson = JsonObject.Parse(response);
                JsonArray boardlistArray = boardlistJson["boards"].GetArray();
                foreach (var singleboard in boardlistArray)
                {
                    string boardname = singleboard.GetObject()["name"].GetString();
                    string boardid = singleboard.GetObject()["id"].GetString();
                    ListViewItem single = new ListViewItem();
                    TextBlock singleText = new TextBlock();
                    singleText.Text = boardname;
                    singleText.Tag = boardid;
                    singleText.FontSize = 32;
                    single.Content = singleText;
                    ListView1.Items.Add(single);
                }
            }
        }
        
    }
    public class mannualParam
    {
        public string boardname { get; set; }
        public List<string>  boardnameList { get; set; }
        public List<string>  boardidList { get; set; }
    }
}

using jnrainbbs.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
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
//using Windows.Web.Http;

// “基本页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍

namespace jnrainbbs
{
    /// <summary>
    /// 可独立使用或用于导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class loginPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public loginPage()
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
        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            //accountTextBox.Text = "xulihang";
            //passwdTextBox.Password = " ";
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (localSettings.Values.ContainsKey("logined") == true)
            {
                if (localSettings.Values["logined"].ToString() == "true")
                {
                    MessageDialog md = new MessageDialog("您已登录，但可能会话已过期,可重新登录");
                    await md.ShowAsync();
                }
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

        private void TextBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        private void accountTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (accountTextBox.Text != "")
            {
                accountHintTextBox.Text = "";
            }
            else
            {
                accountHintTextBox.Text = "学号:";
            }
        }


        private void passwdTextBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (passwdTextBox.Password != "")
            {
                passwdHintTextBox.Text = "";
            }
            else
            {
                passwdHintTextBox.Text = "密码:";
            }
        }


        private async void loginButton_Click(object sender, RoutedEventArgs e)
        {
            loadStarted();
            httputils myhttputils = new httputils();
            string response = await myhttputils.PostStringAsync("http://bbs.jiangnan.edu.cn/rainstyle/apilogin.php", "uid=" + accountTextBox.Text + "&psw=" + passwdTextBox.Password);
            System.Diagnostics.Debug.WriteLine(response);
            loadCompleted();
            if (response.Contains("Error"))
            {
                MessageDialog md = new MessageDialog("登录失败");
                await md.ShowAsync();
            }
            else if((response.Contains("status")==false))
            {
                MessageDialog md = new MessageDialog("502 Bad Gateway");
                await md.ShowAsync();
            }
            else
            {
                JsonObject myJson = JsonObject.Parse(response);
                System.Diagnostics.Debug.WriteLine(response);
                string status = myJson.GetObject()["status"].GetNumber().ToString();
                if (status == "0")
                {
                    //Windows.Web.Http.Headers.HttpResponseHeaderCollection responseHeaders = myhttputils.responseHeaders;
                    //默认登录信息保存一天，可以自行计算
                    var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

                    localSettings.Values["logined"] = "true";

                    MessageDialog md = new MessageDialog("登录成功");
                    await md.ShowAsync();
                    Frame.GoBack();
                }
                else
                {
                    MessageDialog md = new MessageDialog("登录失败");
                    await md.ShowAsync();
                }
            }
        }

        private async void clearButton_Click(object sender, RoutedEventArgs e)
        {
            loadStarted();
            httputils myhttputils = new httputils();
            string message = await myhttputils.GetAsync("http://bbs.jiangnan.edu.cn/rainstyle/apilogout.php");
            string message2= await myhttputils.GetAsync("https://jnrainbbs-bookjnrain.rhcloud.com/clear/" + accountTextBox.Text + "/" + passwdTextBox.Password);
            loadCompleted();
            if (message == "{\"status\":0}")
            {
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

                localSettings.Values["logined"] = "false";
                System.Diagnostics.Debug.WriteLine("注销成功");
            }
            if (message2 == "清除成功")
            {
                MessageDialog md = new MessageDialog("可以了");
                await md.ShowAsync();
            }
            else
            {
                MessageDialog md = new MessageDialog("连接错误或密码不正确");
                await md.ShowAsync();
            }
        }

        private async void logoutButton_Click(object sender, RoutedEventArgs e)
        {
            loadStarted();
            httputils myhttputils = new httputils();
            string message = await myhttputils.GetAsync("http://bbs.jiangnan.edu.cn/rainstyle/apilogout.php");
            loadCompleted();
            if (message == "{\"status\":0}")
            {
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

                localSettings.Values["logined"] = "false";
                MessageDialog md = new MessageDialog("注销成功");
                await md.ShowAsync();
            }
            else
            {
                MessageDialog md = new MessageDialog("连接错误");
                await md.ShowAsync();
            }
        }

        private void loadStarted()
        {
            progressring1.IsActive = true;
            LayoutRoot.IsTapEnabled = false;
            clearButton.IsEnabled = false;
            loginButton.IsEnabled = false;
            logoutButton.IsEnabled = false;
        }

        private void loadCompleted()
        {
            LayoutRoot.IsTapEnabled = true;
            progressring1.IsActive = false;
            clearButton.IsEnabled = true;
            loginButton.IsEnabled = true;
            logoutButton.IsEnabled=true;
        }
    }
}

using jnrainbbs.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// “基本页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍

namespace jnrainbbs
{
    /// <summary>
    /// 可独立使用或用于导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class uploadPage : Page
    {
        public string filename { get; set; }
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public uploadPage()
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

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {

            FileOpenPicker imageOpenPicker = new FileOpenPicker();

            imageOpenPicker.FileTypeFilter.Add(".jpg");
            imageOpenPicker.FileTypeFilter.Add(".png");

            imageOpenPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;

            imageOpenPicker.ContinuationData["Operate"] = "OpenImage";

            imageOpenPicker.PickSingleFileAndContinue();

        }


        private FileOpenPickerContinuationEventArgs filePickerEventArgs;

        public FileOpenPickerContinuationEventArgs FilePickerEventArgs
        {
            get { return filePickerEventArgs; }
            set
            {
                filePickerEventArgs = value;
                ContinuFileOpenPicker(filePickerEventArgs);
            }
        }

        private async void ContinuFileOpenPicker(FileOpenPickerContinuationEventArgs args)
        {

          
            System.Diagnostics.Debug.WriteLine(args.Files[0].Path);
            filename = args.Files[0].Path;
            if (args.ContinuationData["Operate"] as string == "OpenImage" && args.Files != null && args.Files.Count > 0)
            {
                StorageFile file = args.Files[0];
                Image1.Tag = file;
                BitmapImage image = new BitmapImage();
                await image.SetSourceAsync(await file.OpenAsync(FileAccessMode.Read));
                //Stream filestream = await args.Files[0].OpenStreamForReadAsync();
                WriteableBitmap wb = new WriteableBitmap(image.PixelWidth, image.PixelHeight);
                await wb.SetSourceAsync(await file.OpenAsync(FileAccessMode.Read));
                Image1.Source = wb;
                Button1.Tag = "1"; //可以压缩了
                
                //WriteableBitmap wb = new WriteableBitmap(image.PixelWidth, image.PixelHeight);
                //await wb.SetSourceAsync(await file.OpenAsync(FileAccessMode.Read));
                
                //httputils myhttputils = new httputils();
                //await myhttputils.upload(wb.ToByteArray(), "http://bbs.jiangnan.edu.cn/attachments/upload.php");

            }
        }

        private void abortButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private async void uploadButton_Click(object sender, RoutedEventArgs e)
        {
            if (uploadButton.Tag.ToString() == "0")
            {
                MessageDialog md = new MessageDialog("请先选择图片并压缩");
                await md.ShowAsync();
            }
            else
            {
                uploadStarted();
                httputils myhttputils = new httputils();                
                var wb = (WriteableBitmap)Image1.Source;

                //Stream fs = new System.IO.MemoryStream(wb.ToByteArray());

                //SaveToLocalFolderAsync(fs, "scaled.jpg");
                
                StorageFolder x = await  ApplicationData.Current.LocalFolder.CreateFolderAsync("images",CreationCollisionOption.OpenIfExists);      
                StorageFile file = await x.CreateFileAsync("scaled.jpg", CreationCollisionOption.ReplaceExisting);
                using (var s = await file.OpenStreamForWriteAsync())
                {
                    await wb.ToStreamAsJpeg(s.AsRandomAccessStream());
                    //s.Write(wb.ToByteArray(), 0, wb.ToByteArray().Length);
                    //s.Flush();
                }

                string response;
                //try
                //{
                response = await myhttputils.upload("http://bbs.jiangnan.edu.cn/attachments/upload.php", "ms-appdata:///local/images/scaled.jpg");
                //response = await myhttputils.upload("http://bbs.jiangnan.edu.cn/attachments/upload.php", "ms-appx:///Assets/xx.jpg");
                //}
                //catch(Exception exp){
                //    System.Diagnostics.Debug.WriteLine(exp.Message);
                //}

                if (response.Contains("上传成功"))
                {
                    MessageDialog md = new MessageDialog("上传成功");
                    await md.ShowAsync();
                    int ub, lb;
                    ub = response.IndexOf("[pic:");
                    if (response.Contains("png"))
                    {
                        lb = response.IndexOf(".png]")+5;
                    }
                    else if (response.Contains("PNG"))
                    {
                        lb = response.IndexOf(".PNG]") + 5;
                    }
                    else
                    {
                        lb = response.IndexOf(".jpg]")+5;
                    }
                    
                    response = response.Substring(ub, lb - ub);
                    newpostPage.convalue = response;

                    foreach (var page in Frame.BackStack)
                    {
                        System.Diagnostics.Debug.WriteLine(page.ToString());
                        System.Diagnostics.Debug.WriteLine(page.SourcePageType);
                        if (page.SourcePageType.ToString() == "jnrainbbs.uploadPage")
                        {

                            Frame.BackStack.Remove(page);

                        }
                    }

                    Frame.GoBack();
                }
                else
                {
                    MessageDialog md = new MessageDialog("上传失败");
                    await md.ShowAsync();
                }
                uploadCompleted();

            }
        }

        private void ToggleMenuFlyoutItem1_Click(object sender, RoutedEventArgs e)
        {
            processImage(0);
        }

        private void ToggleMenuFlyoutItem2_Click(object sender, RoutedEventArgs e)
        {
            processImage(1);
        }

        private void ToggleMenuFlyoutItem3_Click(object sender, RoutedEventArgs e)
        {
            processImage(2);
        }

        private void uploadStarted()
        {
            progressring1.IsActive = true;
            Button1.IsEnabled = false;
            uploadButton.IsEnabled = false;
            abortButton.IsEnabled=false;
            chooseImgButton.IsEnabled = false;

        }

        private void uploadCompleted()
        {
            progressring1.IsActive = false;
            Button1.IsEnabled = true;
            uploadButton.IsEnabled = true;
            abortButton.IsEnabled = true;
            chooseImgButton.IsEnabled = true;
        }

        private async void processImage(int size)
        {
            if (Button1.Tag.ToString() == "0")
            {
                MessageDialog md = new MessageDialog("请先选择图片");
                await md.ShowAsync();
            }
            else
            {
                try
                {
                    StorageFile file = (StorageFile)Image1.Tag;

                    //WriteableBitmap image = (WriteableBitmap)Image1.Source;

                    BitmapImage image = new BitmapImage();
                    await image.SetSourceAsync(await file.OpenAsync(FileAccessMode.Read));

                    double proportion = Convert.ToDouble(image.PixelWidth) / Convert.ToDouble(image.PixelHeight);

                    int width = 360;
                    if (size == 0)
                    {
                        width = image.PixelWidth;
                    }
                    else if (size==1)
                    {
                        if (image.PixelWidth > 1024)
                        {
                            width = 1024;
                        }
                        else
                        {
                            width = image.PixelWidth/2;
                        }
                        
                    }
                    else
                    {
                        width = 320;
                    }

                    int height = Convert.ToInt32(width / proportion);


                    WriteableBitmap wb = new WriteableBitmap(image.PixelWidth, image.PixelHeight);
                    await wb.SetSourceAsync(await file.OpenAsync(FileAccessMode.Read));

                    System.Diagnostics.Debug.WriteLine("wb:" + wb.PixelHeight);

                    WriteableBitmap newwb = wb.Resize(width, height, WriteableBitmapExtensions.Interpolation.NearestNeighbor);

                    Image1.Source = newwb;
                    uploadButton.Tag = "1";//可以上传了

                    // Image1.Source = wb;
                }
                catch (Exception exp)
                {
                    System.Diagnostics.Debug.WriteLine(exp.Message);
                }
            }
        }

        public async void SaveToLocalFolderAsync(Stream file, string fileName)
        {
            try
            {
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                StorageFile storageFile = await localFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                using (Stream outputStream = await storageFile.OpenStreamForWriteAsync())
                {
                    await file.CopyToAsync(outputStream);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }

    }
}

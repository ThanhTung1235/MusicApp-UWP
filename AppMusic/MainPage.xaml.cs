using AppMusic.Entity;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using AppMusic.Services;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace AppMusic
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private static Member currentLogin;
        private string CurrentTag = " ";
        public static string tokenString = null;
                
                

        public MainPage()
        {
            currentLogin = new Member();
            this.InitializeComponent();
            this.My_Frame.Navigate(typeof(Views.Home));
        }
        private void btn_bar_Click(object sender, RoutedEventArgs e)
        {
            this.My_SplitView.IsPaneOpen = !this.My_SplitView.IsPaneOpen;
        }

        public async void Logout()
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            if (await folder.TryGetItemAsync("token.txt") != null)
            {
                StorageFile file = await folder.GetFileAsync("token.txt");
                await file.DeleteAsync();
                Debug.WriteLine("you logouted !!!");
            }
        }
        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            RadioButton radio = sender as RadioButton;
            if (CurrentTag == radio.Tag.ToString())
            {
                return;
            }
            switch (radio.Tag.ToString())
            {
                case "Home":
                    CurrentTag = "Home";
                    this.My_Frame.Navigate(typeof(Views.Home));
                    break;
                case "My_account":
                    CurrentTag = "My_account";
                    this.My_Frame.Navigate(typeof(Views.Get_info_user));
                    break;
                //case "Register":
                //    CurrentTag = "Register";
                //    this.My_Frame.Navigate(typeof(Views.Sign_Up));
                //    break;
                //case "Login":
                //    CurrentTag = "Login";
                //    this.My_Frame.Navigate(typeof(Views.Sign_In));
                //    break;
                case "HotSong":
                    CurrentTag = "HotSong";
                    this.My_Frame.Navigate(typeof(Views.List_music));
                    break;
                case "Logout":
                    CurrentTag = "Logout";
                    Logout();
                    var rootFrame = Window.Current.Content as Frame;
                    rootFrame.Navigate(typeof(Views.Sign_In));
                    break;
            }
        }

    }
}






﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Search;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using AppMusic.Entity;
using AppMusic.Services;
using Newtonsoft.Json;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace AppMusic.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class List_music : Page
    {
        private ObservableCollection<Song> listSong;
        private ObservableCollection<SongLocal> listSongLocal;
        public static string tokenKey = null;
        private int _currentIndex;

        internal ObservableCollection<Song> ListSong
        {
            get => listSong;
            set => listSong = value;
        }

        internal ObservableCollection<SongLocal> ListSongLocal
        {
            get => listSongLocal;
            set => listSongLocal = value;
        }

        private Song currentSong;
        private bool _isPlaying = false;

        public List_music()
        {
            this.InitializeComponent();
            this.currentSong = new Song();
            this.ListSong = new ObservableCollection<Song>();
            this.ListSongLocal = new ObservableCollection<SongLocal>();
            this.GetSongOnline();
            this.GetSongsInLibary();
        }

        public static async Task<string> ReadToken()
        {
            if (tokenKey == null)
            {
                StorageFolder folder = ApplicationData.Current.LocalFolder;
                StorageFile file = await folder.GetFileAsync("token.txt");
                string content = await FileIO.ReadTextAsync(file);
                TokenResponse member_token = JsonConvert.DeserializeObject<TokenResponse>(content);
                Debug.WriteLine("token la: " + member_token.Token);
                tokenKey = member_token.Token;
            }

            return tokenKey;
        }

        public async void GetSongOnline()
        {
            await ReadToken();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(APIHandle.GET_SONG);
            //String encoded = ReadToken().ToString();
            request.Headers.Add("Authorization", "Basic " + tokenKey);
            request.Method = "GET";
            Debug.WriteLine(tokenKey);
            HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
            StreamReader resStreamReader = new StreamReader(response.GetResponseStream());
            string result = resStreamReader.ReadToEnd();
            ObservableCollection<Song> listSongs = JsonConvert.DeserializeObject<ObservableCollection<Song>>(result);
            foreach (var songs in listSongs)
            {

                ListSong.Add(songs);
            }


            //Debug.WriteLine(result);

        }

        private async void btn_add(object sender, RoutedEventArgs e)
        {
            HttpClient client = new HttpClient();
            this.currentSong.name = this.txt_name.Text;
            this.currentSong.description = this.txt_description.Text;
            this.currentSong.singer = this.txt_singer.Text;
            this.currentSong.author = this.txt_author.Text;
            this.currentSong.thumbnail = this.txt_thumbnail.Text;
            this.currentSong.link = this.txt_link.Text;


            var jsonSong = JsonConvert.SerializeObject(this.currentSong);
            StringContent content = new StringContent(jsonSong, Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders.Add("Authorization", "Basic " + tokenKey);
            var response = client.PostAsync(APIHandle.REGISTER_SONG, content);
            var contents = await response.Result.Content.ReadAsStringAsync();
            if (response.Result.StatusCode == HttpStatusCode.Created)
            {
                Debug.WriteLine("Success");
            }
            else
            {
                ErrorResponse errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(contents);
                if (errorResponse.error.Count > 0)
                {
                    foreach (var key in errorResponse.error.Keys)
                    {
                        if (this.FindName(key) is TextBlock textBlock)//kiem tra xem no co phai la mot dang textBlock
                        {
                            textBlock.Text = "* " + errorResponse.error[key];
                        }
                    }
                }
            }

            this.txt_name.Text = String.Empty;
            this.txt_description.Text = String.Empty;
            this.txt_singer.Text = String.Empty;
            this.txt_author.Text = String.Empty;
            this.txt_thumbnail.Text = String.Empty;
            this.txt_link.Text = String.Empty;
        }

        private void currentSongs(object sender, TappedRoutedEventArgs e)
        {
            //Song choosed with tapped
            StackPanel panel = sender as StackPanel;
            CommandBar_control.Visibility = Visibility.Visible;
            Song chooseSong = panel.Tag as Song;
            Debug.WriteLine(chooseSong.link);
            _currentIndex = this.MyListSong.SelectedIndex;
            Uri mp3Link = new Uri(chooseSong.link);
            this.mediaElement.Source = mp3Link;
            this.name_song.Text = this.ListSong[_currentIndex].name + " - " + this.ListSong[_currentIndex].singer;
            Do_play();
        }
        private void LoadSongFromLocal(SongLocal songLocal)
        {
            Debug.WriteLine(songLocal.Type);
            this.mediaElement.SetSource(songLocal.Stream, songLocal.Type);
            this.name_song.Text = songLocal.Name + "-" + songLocal.Singer;

        }
        private async void GetSongLocal(IReadOnlyList<StorageFile> files)
        {
            if (files.Count > 0)
            {
                foreach (var file in files)
                {
                    MusicProperties musicProperties = await file.Properties.GetMusicPropertiesAsync();
                    const uint resquestedSize = 190;
                    const ThumbnailMode thumbnailMode = ThumbnailMode.MusicView;
                    const ThumbnailOptions thumbnailOptions = ThumbnailOptions.UseCurrentScale;
                    var thumbnail_song = await file.GetThumbnailAsync(thumbnailMode, resquestedSize, thumbnailOptions);
                    await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                        CoreDispatcherPriority.Normal, async () =>
                        {
                            BitmapImage bitmapImage = new BitmapImage();
                            bitmapImage.SetSource(thumbnail_song);
                            listSongLocal.Add(new SongLocal()
                            {
                                Name = musicProperties.Title,
                                Singer = musicProperties.Artist,
                                Author = musicProperties.Artist,
                                Album = musicProperties.Album,
                                Time = musicProperties.Duration.ToString().Split()[0],
                                Thumbnai = bitmapImage,
                                Stream = await file.OpenAsync(FileAccessMode.Read),
                                Type = file.ContentType
                            });

                        });

                }
            }
        }
        private async void GetSongsInLibary()
        {
            QueryOptions queryOption = new QueryOptions
                (CommonFileQuery.OrderByTitle, new string[] { ".mp3"});

            queryOption.FolderDepth = FolderDepth.Deep;

            Queue<IStorageFolder> folders = new Queue<IStorageFolder>();

            var files = await KnownFolders.MusicLibrary.CreateFileQueryWithOptions
                (queryOption).GetFilesAsync();

            foreach (var file in files)
            {
                Debug.WriteLine(file.DisplayName);
            }
            GetSongLocal(files);
            
        }

       
        //
        private void Choosed_song(object sender, TappedRoutedEventArgs e)
        {
            
            StackPanel stackPanel = sender as StackPanel;
            SongLocal songLocal_selected = stackPanel.Tag as SongLocal;
            _currentIndex = ListSongInLibary.SelectedIndex;
            this.CommandBar_control.Visibility = Visibility.Visible;
            this.name_song.Text = this.ListSong[_currentIndex].name + " - " + this.ListSong[_currentIndex].singer;
            LoadSongFromLocal(songLocal_selected);
            Do_play();

        }













        private void Do_play()
        {
            _isPlaying = true;
            this.status_song.Text = "Now Playing :";
            this.mediaElement.Play();
            PlayButton.Icon = new SymbolIcon(Symbol.Pause);


        }

        private void Do_pause()
        {
            _isPlaying = false;
            this.status_song.Text = "pause Playing :";
            this.mediaElement.Pause();
            PlayButton.Icon = new SymbolIcon(Symbol.Play);
        }



        private void Player_Click(object sender, RoutedEventArgs e)
        {
            if (_isPlaying)
            {
                Do_pause();
            }
            else
            {
                Do_play();
            }
        }

        private void btn_Previous(object sender, RoutedEventArgs e)
        {
            mediaElement.Stop();
            if (_currentIndex >= 0)
            {
                _currentIndex -= 1;
            }
            else
            {
                _currentIndex = listSong.Count - 1;
            }

            Uri mp3Link = new Uri(ListSong[_currentIndex].link);
            this.name_song.Text = this.ListSong[_currentIndex].name + " - " + this.ListSong[_currentIndex].singer;
            this.mediaElement.Source = mp3Link;
            Debug.WriteLine(mp3Link);
            Do_play();

        }

        private void btn_Next(object sender, RoutedEventArgs e)
        {
            mediaElement.Stop();
            if (_currentIndex < ListSong.Count - 1)
            {
                _currentIndex += 1;
            }
            else
            {
                _currentIndex = 0;
            }

            Uri mp3Link = new Uri(ListSong[_currentIndex].link);
            this.name_song.Text = this.ListSong[_currentIndex].name + " - " + this.ListSong[_currentIndex].singer;
            Debug.WriteLine(mp3Link);
            this.mediaElement.Source = mp3Link;
            Do_play();
        }

       
    }
}





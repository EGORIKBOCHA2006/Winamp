using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Winamp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        public List<string> FilesOfRecipes
        {
            get
            {
                List<string> files = new List<string>();
                foreach (var file in Directory.EnumerateFiles("music", $"*.mp3"))
                {
                    files.Add(System.IO.Path.GetFileName(file));
                }
                return files;
            }
        }
       
        public void FillList()
        {
            ListOfRecipes.ItemsSource = FilesOfRecipes;
        }



        DispatcherTimer timer;
        string url;
        bool statMedia = true;
        int indexSong = -1;
        public MainWindow()
        {
            InitializeComponent();
            new DispatcherTimer();
            FillList();
            url = "";
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(0.1);
            timer.Tick += Timer_Tick;
            tbSelected.Text=url.ToString();
            btnPlay.IsEnabled = false;
            
            btnUpload.IsEnabled = false;
            btnBack.IsEnabled = false;
            btnStop.IsEnabled = false;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            sliderPosition.Value = media.Position.TotalSeconds;
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            
            statMedia=!statMedia;
            if (statMedia)
            {
                media.Play();
                timer.Start();
                btnPlay.Content =FindResource("pauseImg");
            }
            else
            {
                media.Pause();
                timer.Start();
                btnPlay.Content = FindResource("playImg");

            }
            
        }


        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            media.Stop();
            timer.Stop();
            statMedia = false;
            btnPlay.Content = FindResource("playImg");
        }

        private void btnUpload_Click(object sender, RoutedEventArgs e)
        {
            
            if (ListOfRecipes.SelectedIndex == ListOfRecipes.Items.Count - 1)
            {
                btnNext.IsEnabled = false;
                btnBack.IsEnabled = true;
            }
            btnStop.IsEnabled = true;
            indexSong=ListOfRecipes.SelectedIndex;
            btnPlay.IsEnabled = true;
            url = ListOfRecipes.SelectedItem.ToString();
            media.Stop();
            media.Source = new Uri($"music\\{url}", UriKind.RelativeOrAbsolute);
            
            media.Play();
            btnPlay.Content = FindResource("pauseImg");
            btnUpload.IsEnabled = false;
            
        }
        private void sliderPosition_ValueChanged(object sender, RoutedEventArgs e)
        {
            media.Pause();
            media.Position = TimeSpan.FromSeconds(sliderPosition.Value);
            media.Play();


        }



        private void ListOfRecipes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            tbSelected.Text = ListOfRecipes.SelectedItem.ToString();
            if (tbSelected.Text!=url)
                btnUpload.IsEnabled = true;
            else
                btnUpload.IsEnabled = false;
            
            
            
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Audio files (*.mp3)|*.mp3";
            if (ofd.ShowDialog()==true)
            {
                FilesOfRecipes.Add(ofd.SafeFileName);

                try
                {
                    File.Copy(ofd.FileName, $"music\\{ofd.SafeFileName}");
                }
                catch(System.IO.IOException)
                {
                    MessageBox.Show("Композиция уже добавлена","Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning );
                }
                FillList();
            }
            
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            media.Volume=slVolume.Value/100;
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            
            btnStop.IsEnabled = true;
            if (indexSong+1 != 0)
                btnBack.IsEnabled = true;
            ListOfRecipes.SelectedIndex = ++indexSong;
            tbSelected.Text = ListOfRecipes.SelectedItem.ToString();
            btnPlay.IsEnabled = true;
            url = ListOfRecipes.SelectedItem.ToString();
            media.Stop();
            media.Source = new Uri($"music\\{url}", UriKind.RelativeOrAbsolute);
            tbMediaName.Text = url;
            media.Play();
            btnPlay.Content = FindResource("pauseImg");
            btnUpload.IsEnabled = false;


            if (indexSong == ListOfRecipes.Items.Count - 1)

                btnNext.IsEnabled = false;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
           
            btnNext.IsEnabled = true;
            ListOfRecipes.SelectedIndex = --indexSong;
            tbSelected.Text = ListOfRecipes.SelectedItem.ToString();
            btnPlay.IsEnabled = true;
            url = ListOfRecipes.SelectedItem.ToString();
            media.Stop();
            media.Source = new Uri($"music\\{url}", UriKind.RelativeOrAbsolute);
            tbMediaName.Text = url;
            media.Play();
            btnPlay.Content = FindResource("pauseImg");
            btnUpload.IsEnabled = false;
            if (indexSong == 0)
                btnBack.IsEnabled = false;






        }

        private void media_MediaOpened(object sender, RoutedEventArgs e)
        {
            sliderPosition.Value = 0;
            sliderPosition.Maximum = media.NaturalDuration.TimeSpan.TotalSeconds;
            tbMediaName.Text = url+" ("+ media.NaturalDuration.TimeSpan.Minutes+":"+ (media.NaturalDuration.TimeSpan.Seconds%60).ToString().PadLeft(2,'0')+")";
            timer.Start();
        }

        private void media_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (indexSong!=ListOfRecipes.Items.Count-1)
            {
                indexSong++;
                ListOfRecipes.SelectedIndex = indexSong;
                this.btnUpload_Click(sender, e);
                
            }
        }

        private void sliderPosition_LostMouseCapture(object sender, MouseEventArgs e)
        {
            
            media.Position = TimeSpan.FromSeconds(sliderPosition.Value);
            if (statMedia)
            {
                timer.Start();
                media.Play();
            }
        }

        private void sliderPosition_GotMouseCapture(object sender, MouseEventArgs e)
        {
            timer.Stop();
        }
    }
}

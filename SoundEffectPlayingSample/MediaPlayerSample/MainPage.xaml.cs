using System.Windows.Input;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Resources;
using System.Windows.Threading;

namespace MediaPlayerSample
{
    /// <summary>
    /// Simple demo of being able to play multiple sounds
    /// *Note this will just play sounds with no management as it's unlikey the user will interact enough to break the 32 simultaneous channel limit
    /// If the above is likely then consider having some limiting control
    /// 1 tap = 1 sound play until it finishes = 1 channel
    /// </summary>
    public partial class MainPage : PhoneApplicationPage
    {
        DispatcherTimer XnaDispatchTimer;
        int activeSound;
        //Raw collection of sounds with no lookup - Play by index
        List<SoundEffect> soundlist = new List<SoundEffect>();

        //Dictionary of sounds with a name lookup
        Dictionary<string, SoundEffect> soundEffects = new Dictionary<string, SoundEffect>();

        private Dictionary<Rect,string> touchPointsCollection = new Dictionary<Rect,string>();

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            XnaDispatchTimer = new DispatcherTimer();
            XnaDispatchTimer.Interval = TimeSpan.FromMilliseconds(50);

            //Very important, FrameworkDispatcher called regually throughout the life of this page
            XnaDispatchTimer.Tick += delegate
            {
                try
                {
                    FrameworkDispatcher.Update();
                }
                catch { }
            };

            this.Loaded += new RoutedEventHandler(MainPage_Loaded);


            
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            //Start the sound engine
            XnaDispatchTimer.Start();
            touchPointsCollection = new Dictionary<Rect, string>();

            //Load the Audio resources, if used on multiple pages consider doing in App.XAML.cs and have a static ref to the references
            StreamResourceInfo info = Application.GetResourceStream(new Uri("Audio/ApplauseShort.wav", UriKind.Relative));
            var effect = SoundEffect.FromStream(info.Stream);
            soundlist.Add(effect);
            CreateSound("Applause", effect);

            info = Application.GetResourceStream(new Uri("Audio/Explosion.wav", UriKind.Relative));
            var effect2 = SoundEffect.FromStream(info.Stream);
            soundlist.Add(effect2);
            CreateSound("Explosion",effect2);

            info = Application.GetResourceStream(new Uri("Audio/Flip_win.wav", UriKind.Relative));
            var effect3 = SoundEffect.FromStream(info.Stream);
            soundlist.Add(effect3);
            CreateSound("Flip", effect3);

            info = Application.GetResourceStream(new Uri("Audio/Tapped.wav", UriKind.Relative));
            var effect4 = SoundEffect.FromStream(info.Stream);
            soundlist.Add(effect4);
            CreateSound("Tapped", effect4);

            activeSound = 0;

            //Enable Touch reporting only AFTER all content loaded!
            Touch.FrameReported += new TouchFrameEventHandler(Touch_FrameReported);
        }
  
        private void CreateSound(string soundName, SoundEffect effect)
        {
            soundEffects.Add(soundName, effect);
            FrameworkElement Element = (FrameworkElement)FindName(soundName);
            var ElementTransform = Element.TransformToVisual(LayoutRoot);
            touchPointsCollection.Add(ElementTransform.TransformBounds(new Rect(0, 0, Element.ActualWidth, Element.ActualHeight)), soundName);
        }

        void Touch_FrameReported(object sender, TouchFrameEventArgs e)
        {
            //TouchPoint primaryTouchPoint = e.GetPrimaryTouchPoint(null);


            TouchPointCollection touchPoints = e.GetTouchPoints(null);


            foreach (TouchPoint tp in touchPoints)
            {
                if (tp.Action == TouchAction.Down)
                {
                    foreach (var touchPoint in touchPointsCollection)
                    {
                        if (touchPoint.Key.Contains(tp.Position))
                        {
                            PlaySound(touchPoint.Value);
                        }
                    }
                }

            }
        }

        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            //Important to stop the audio timer of you leave a page playing audio, if audio used throughout entire app, move to App.XAML.cs
            XnaDispatchTimer.Stop();
            base.OnNavigatingFrom(e);
        }

        #region Manual buttons to loop through available sounds
        private void btnPreviousSong_Click(object sender, RoutedEventArgs e)
        {
            soundlist[activeSound].Play();
            if (activeSound > 0)
            {
                activeSound -= 1;
            }
        }

        private void btnNextSong_Click(object sender, RoutedEventArgs e)
        {
            soundlist[activeSound].Play();
            if (activeSound < soundlist.Count - 1)
            {
                activeSound += 1;
            }
        }
        #endregion

        #region Events for tapping on individual sounds

        //These could just all be one event used by all references on MainPage.XAML
        private void ApplauseControl_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            PlaySound(((UserControl)sender).Tag.ToString());
        }

        private void ExplosionControl_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            PlaySound(((UserControl)sender).Tag.ToString());
        }

        private void FlipControl_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            PlaySound(((UserControl)sender).Tag.ToString());
        }

        private void TappedControl_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            PlaySound(((UserControl)sender).Tag.ToString());
        }
        #endregion


        void PlaySound(string soundName)
        {
            if (soundEffects.ContainsKey(soundName))
            {
                soundEffects[soundName].Play();
            }
        }

    }
}
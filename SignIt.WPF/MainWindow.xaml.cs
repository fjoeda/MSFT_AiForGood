using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

using System.IO.IsolatedStorage;

using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using System.Globalization;

namespace SignIt.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        GesturesTest gestures;
        string oldArg = "";
        public MainWindow()
        {
            InitializeComponent();
            //videoCapElement.VideoCaptureSource = "Intel(R) RealSense(TM) Camera SR300 Virtual Driver";
            videoCapElement_Copy.VideoCaptureSource = "Intel(R) RealSense(TM) Camera SR300 Virtual Driver";
            gestures = new GesturesTest();
            gestures.GestureChanged += (arg) => Dispatcher.InvokeAsync(() =>
            {
                if(!arg.Equals(oldArg))
                    txt_SignInterpr.Text += ParseGestureId(arg) + " ";
                oldArg = arg;
            });

            Loaded += async (s, arg) => await gestures.Init();
            Closed += (s, arg) => gestures?.Dispose();
        }

        public void RecognizeSpeech()
        {
            // Creates an instance of a speech config with specified subscription key and service region.
            // Replace with your own subscription key and service region (e.g., "westus").
            var config = SpeechConfig.FromSubscription("c78539a1f1754b37a9d72875a3d19c06", "southeastasia");

            // Creates a speech recognizer.
            using (var recognizer = new SpeechRecognizer(config))
            {
                Console.WriteLine("Say something...");

                // Starts speech recognition, and returns after a single utterance is recognized. The end of a
                // single utterance is determined by listening for silence at the end or until a maximum of 15
                // seconds of audio is processed.  The task returns the recognition text as result. 
                // Note: Since RecognizeOnceAsync() returns only a single utterance, it is suitable only for single
                // shot recognition like command or query. 
                // For long-running multi-utterance recognition, use StartContinuousRecognitionAsync() instead.
                var result = recognizer.RecognizeOnceAsync().Result;

                // Checks result.
                if (result.Reason == ResultReason.RecognizedSpeech)
                {
                    txt_STTText.Text += result.Text + Environment.NewLine;
                }
                else if (result.Reason == ResultReason.NoMatch)
                {
                    Console.WriteLine($"NOMATCH: Speech could not be recognized.");
                }
                else if (result.Reason == ResultReason.Canceled)
                {
                    var cancellation = CancellationDetails.FromResult(result);
                    Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                    if (cancellation.Reason == CancellationReason.Error)
                    {
                        Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                        Console.WriteLine($"CANCELED: ErrorDetails={cancellation.ErrorDetails}");
                        Console.WriteLine($"CANCELED: Did you update the subscription info?");
                    }
                }
            }
        }

        private void stopButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            startButton.IsEnabled = false;
            startButton.Content = "Recognizing...";
            RecognizeSpeech();
            startButton.IsEnabled = true;
            startButton.Content = "Start";
        }

        private string ParseGestureId(string text)
        {
            switch (text)
            {
                case "Huruf_I":
                    return "i";
                case "Piece":
                    return "2";
                case "baik":
                    return "baik";
                case "LikeGesture":
                    return "mantab";
                default:
                    return "";
            }
        }

        private void mainTab_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Tab1.IsSelected)
            {
                videoCapElement.VideoCaptureSource = " ";
                //videoCapElement.Stop();
                videoCapElement_Copy.VideoCaptureSource = "Intel(R) RealSense(TM) Camera SR300 Virtual Driver";
            }
            else if (Tab2.IsSelected)
            {
                videoCapElement_Copy.VideoCaptureSource = "";
                //videoCapElement_Copy.Stop();
                videoCapElement.VideoCaptureSource = "Intel(R) RealSense(TM) Camera SR300 Virtual Driver";
            }
        }
    }


}


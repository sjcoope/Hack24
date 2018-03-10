using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Popups;
using Hack24.EmotionalAwareness.App.Config;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Hack24.EmotionalAwareness.App
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly IFaceServiceClient faceServiceClient;

        public MainPage()
        {
            var text = File.ReadAllText("./Assets/FaceServiceAPIKey.txt");
            faceServiceClient = new FaceServiceClient(text, "https://westeurope.api.cognitive.microsoft.com/face/v1.0");
            this.InitializeComponent();
            ShowPreview();
        }

        private async void ShowPreview()
        {
            MediaCapture capture = new MediaCapture();
            await capture.InitializeAsync();
            PART_Capture.Source = capture;
            await capture.StartPreviewAsync();
        }

        private async void Capture_Click(object sender, RoutedEventArgs e)
        {
            var stream = new InMemoryRandomAccessStream();
            await PART_Capture.Source.CapturePhotoToStreamAsync(ImageEncodingProperties.CreatePng(), stream);

            var stm = stream.AsStream();
            stm.Seek(0, SeekOrigin.Begin);
            var br = new BinaryReader(stm);
            var bytes = br.ReadBytes((int)stm.Length);

            var decoder = await BitmapDecoder.CreateAsync(stream);
            var image = await decoder.GetSoftwareBitmapAsync(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
            var source = new SoftwareBitmapSource();

            var width = image.PixelWidth;
            var height = image.PixelHeight;
            var scaleX = PART_Canvas.Width / image.PixelWidth;
            var scaleY = PART_Canvas.Height / image.PixelHeight;
            stream.Seek(0);
            image = await decoder.GetSoftwareBitmapAsync(
                BitmapPixelFormat.Bgra8,
                BitmapAlphaMode.Premultiplied,
                new BitmapTransform { ScaledHeight = (uint)((double)height * scaleY), ScaledWidth = (uint)((double)width * scaleX) },
                ExifOrientationMode.IgnoreExifOrientation,
                ColorManagementMode.DoNotColorManage);

            await source.SetBitmapAsync(image);

            PART_Canvas.Children.Clear();
            PART_Canvas.Children.Add(new Image { Source = source });
            PART_Canvas.Visibility = Visibility.Visible;

            var faces = await UploadAndDetectFaces(stream.AsStream());
            ProcessFaces(faces);
        }

        private void OutputFaceInfo(Face[] faces)
        {

        }

        private void HighlightFaces(Face[] faces)
        {

        }

        private void ProcessFaces(Face[] faces)
        {
            OutputFaceInfo(faces);
            HighlightFaces(faces);
        }

        private async Task<Face[]> UploadAndDetectFaces(Stream imageStream)
        {
            // The list of Face attributes to return.
            IEnumerable<FaceAttributeType> faceAttributes =
                new FaceAttributeType[] { FaceAttributeType.Gender, FaceAttributeType.Age, FaceAttributeType.Smile, FaceAttributeType.Emotion, FaceAttributeType.Glasses, FaceAttributeType.Hair };

            // Call the Face API.
            try
            {
                using (imageStream)
                {
                    Face[] faces = await faceServiceClient.DetectAsync(imageStream, returnFaceId: true, returnFaceLandmarks: false, returnFaceAttributes: faceAttributes);
                    return faces;
                }
            }
            // Catch and display Face API errors.
            catch (FaceAPIException f)
            {
                var dialog = new MessageDialog(f.ErrorMessage, f.ErrorCode);
                await dialog.ShowAsync();
                return new Face[0];
            }
            // Catch and display all other errors.
            catch (Exception e)
            {
                var dialog = new MessageDialog(e.Message, "Error");
                await dialog.ShowAsync();
                return new Face[0];
            }
        }
    }
}

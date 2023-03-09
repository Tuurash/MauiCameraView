using Camera.MAUI;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using ImageFormat = Camera.MAUI.ImageFormat;

namespace CameraView;

public partial class MainPage : ContentPage
{



    public MainPage()
    {
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);
        NavigationPage.SetBackButtonTitle(this, null);

        CameraViewFrame.CamerasLoaded += Cameras_Loaded;
    }

    private async void Cameras_Loaded(object? sender, EventArgs e) => await CheckPermission();

    public bool CameraEnabled = false;

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CheckPermission();
    }

    private async Task CheckPermission()
    {
        PermissionStatus result = await Permissions.CheckStatusAsync<Permissions.Camera>();

        if (result == PermissionStatus.Granted)
        {
            await StartCameraAsync();
            return;
        }

        result = await Permissions.RequestAsync<Permissions.Camera>();

        if (result == PermissionStatus.Granted)
        {
            await StartCameraAsync();
            return;
        }
        else
        {
            var toast = Toast.Make("Camera Permission Declined.", ToastDuration.Short, 14);
            await toast.Show();
        }
    }

    private async Task StartCameraAsync()
    {
        await Task.Delay(100);
        try
        {
            var Cameras = CameraViewFrame.Cameras.ToList();
            if (Cameras.Any())
            {
                CameraViewFrame.Camera = Cameras.FirstOrDefault();

                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await CameraViewFrame.StartCameraAsync();

                    if (await CameraViewFrame.StartCameraAsync() == CameraResult.Success)
                    {
                        CameraEnabled = true;
                    }
                    else
                    {
                        CameraEnabled = false;
                        var toast = Toast.Make("Something Went Wrong!", ToastDuration.Short, 14);
                        await toast.Show();
                    }
                });
            }
        }
        catch { CameraEnabled = false; }
    }

    private async void Capture_Clicked(object sender, EventArgs e)
    {
        ImageSource? CapturedImage = null;
        try
        {
            if (CameraEnabled)
            {
                LoaderPreview.IsRunning = true; LoaderPreview.IsVisible = true; LoaderText.Text = "Preparing Preview ..";

                await Task.Delay(1000);

                LoaderText.Text = "Ready";

                await Task.Delay(10);

                await MainThread.InvokeOnMainThreadAsync(() =>
                                 CapturedImage = CameraViewFrame.GetSnapShot(ImageFormat.PNG));

                await Task.Delay(100);

                if (CapturedImage != null)
                {
                    await MainThread.InvokeOnMainThreadAsync(
                                     () => ImgCapturedImage.Source = CapturedImage);
                }
                else
                {
                    var toast = Toast.Make("Something Went Wrong!", ToastDuration.Short, 14);
                    await toast.Show();
                }
            }
            else
                await CheckPermission();
        }
        finally { LoaderPreview.IsRunning = false; LoaderPreview.IsVisible = false; LoaderText.Text = ""; }
    }

    public static byte[] ReadFully(Stream input)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            input.CopyTo(ms);
            return ms.ToArray();
        }
    }
}
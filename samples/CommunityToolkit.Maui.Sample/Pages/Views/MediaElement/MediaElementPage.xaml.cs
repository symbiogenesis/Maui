﻿using System.ComponentModel;
using CommunityToolkit.Maui.Core.Primitives;
using CommunityToolkit.Maui.Sample.ViewModels.Views;
using CommunityToolkit.Maui.Views;
using Microsoft.Extensions.Logging;

namespace CommunityToolkit.Maui.Sample.Pages.Views;

public partial class MediaElementPage : BasePage<MediaElementViewModel>
{
	readonly ILogger logger;

	public MediaElementPage(MediaElementViewModel viewModel, ILogger<MediaElementPage> logger) : base(viewModel)
	{
		InitializeComponent();

		this.logger = logger;
		mediaElement.PropertyChanged += MediaElement_PropertyChanged;
	}

	void MediaElement_PropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		if (e.PropertyName == nameof(mediaElement.Duration))
		{
			logger.LogInformation("Duration: {newDuration}", mediaElement.Duration);
			positionSlider.Maximum = mediaElement.Duration.TotalSeconds;
		}
	}

	void OnMediaOpened(object? sender, EventArgs e) => logger.LogInformation("Media opened.");

	void OnStateChanged(object? sender, MediaStateChangedEventArgs e) =>
		logger.LogInformation("Media State Changed. Old State: {PreviousState}, New State: {NewState}", e.PreviousState, e.NewState);

	void OnMediaFailed(object? sender, MediaFailedEventArgs e) => logger.LogInformation("Media failed. Error: {ErrorMessage}", e.ErrorMessage);

	void OnMediaEnded(object? sender, EventArgs e) => logger.LogInformation("Media ended.");

	void OnPositionChanged(object? sender, MediaPositionChangedEventArgs e)
	{
		logger.LogInformation("Position changed to {position}", e.Position);
		positionSlider.Value = e.Position.TotalSeconds;
	}

	void OnSeekCompleted(object? sender, EventArgs e) => logger.LogInformation("Seek completed.");

	void OnResetClicked(object? sender, EventArgs e)
	{
		mediaElement.Source = null;
	}

	void OnMp4OnlineSourceClicked(object? sender, EventArgs e)
	{
		mediaElement.Source = MediaSource.FromUri("https://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4");
	}

	void OnHlsSourceClicked(object? sender, EventArgs e)
	{
		mediaElement.Source = MediaSource.FromUri("https://wowza.peer5.com/live/smil:bbb_abr.smil/playlist.m3u8");
	}

	void OnResourceSourceClicked(object? sender, EventArgs e)
	{
		if (DeviceInfo.Platform == DevicePlatform.MacCatalyst
			|| DeviceInfo.Platform == DevicePlatform.iOS)
		{
			mediaElement.Source = MediaSource.FromResource("AppleVideo.mp4");
		}
		else if (DeviceInfo.Platform == DevicePlatform.Android)
		{
			mediaElement.Source = MediaSource.FromResource("AndroidVideo.mp4");
		}
		else if (DeviceInfo.Platform == DevicePlatform.WinUI)
		{
			mediaElement.Source = MediaSource.FromResource("WindowsVideo.mp4");
		}
	}

	void OnSpeedMinusClicked(object? sender, EventArgs e)
	{
		if (mediaElement.Speed >= 1)
		{
			mediaElement.Speed -= 1;
		}
	}

	void OnSpeedPlusClicked(object? sender, EventArgs e)
	{
		if (mediaElement.Speed < 10)
		{
			mediaElement.Speed += 1;
		}
	}

	void OnVolumeMinusClicked(object? sender, EventArgs e)
	{
		if (mediaElement.Volume >= 0)
		{
			if (mediaElement.Volume < .1)
			{
				mediaElement.Volume = 0;

				return;
			}

			mediaElement.Volume -= .1;
		}
	}

	void OnVolumePlusClicked(object? sender, EventArgs e)
	{
		if (mediaElement.Volume < 1)
		{
			if (mediaElement.Volume > .9)
			{
				mediaElement.Volume = 1;

				return;
			}

			mediaElement.Volume += .1;
		}
	}

	void OnPlayClicked(object? sender, EventArgs e)
	{
		mediaElement.Play();
	}

	void OnPauseClicked(object? sender, EventArgs e)
	{
		mediaElement.Pause();
	}

	void OnStopClicked(object? sender, EventArgs e)
	{
		mediaElement.Stop();
	}

	void BasePage_Unloaded(object? sender, EventArgs e)
	{
		// Stop and cleanup MediaElement when we navigate away
		mediaElement.Handler?.DisconnectHandler();
	}

	void Slider_DragCompleted(object? sender, EventArgs e)
	{
		ArgumentNullException.ThrowIfNull(sender);

		var newValue = ((Slider)sender).Value;
		mediaElement.SeekTo(TimeSpan.FromSeconds(newValue));
		mediaElement.Play();
	}

	void Slider_DragStarted(object sender, EventArgs e)
	{
		mediaElement.Pause();
    }

	void Button_Clicked(object? sender, EventArgs e)
	{
		if (string.IsNullOrWhiteSpace(customSourceEntry.Text))
		{
			DisplayAlert("Error Loading URL Source", "No value was found to load as a media source. " +
				"When you do enter a value, make sure it's a valid URL. No additional validation is done.",
				"OK");

			return;
		}

		mediaElement.Source = MediaSource.FromUri(customSourceEntry.Text);
	}
}
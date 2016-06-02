﻿using System;
using System.Collections.Generic;
using System.Drawing;

namespace EveOPreview.UI
{
	/// <summary>
	/// Main view interface
	/// Presenter uses it to access GUI properties
	/// </summary>
	public interface IMainView : IView
	{
		bool MinimizeToTray { get; set; }

		double ThumbnailsOpacity { get; set; }

		bool EnableClientsLocationTracking { get; set; }
		bool HideActiveClientThumbnail { get; set; }
		bool ShowThumbnailsAlwaysOnTop { get; set; }
		bool HideThumbnailsOnLostFocus { get; set; }
		bool EnablePerClientThumbnailsLayouts { get; set; }

		bool SyncThumbnailsSize { get; set; }
		int ThumbnailsWidth { get; set; }
		int ThumbnailsHeight { get; set; }

		bool EnableThumbnailZoom { get; set; }
		int ThumbnailZoomFactor { get; set; }
		ViewZoomAnchor ThumbnailZoomAnchor { get; set; }

		bool ShowThumbnailOverlays { get; set; }
		bool ShowThumbnailFrames { get; set; }

		void SetForumUrl(string url);

		void Minimize();

		void AddThumbnails(IList<IThumbnailDescriptionView> thumbnails);
		void UpdateThumbnails(IList<IThumbnailDescriptionView> thumbnails);
		void RemoveThumbnails(IList<IThumbnailDescriptionView> thumbnails);
		void UpdateThumbnailsSizeView(Size size);
		void UpdateZoomSettingsView();

		event Action ApplicationExitRequested;
		event Action FormActivated;
		event Action FormMinimized;
		event Action<ViewCloseRequest> FormCloseRequested;
		event Action ApplicationSettingsChanged;
		event Action ThumbnailsSizeChanged;
		event Action<IntPtr> ThumbnailStateChanged;
		event Action ForumUrlLinkActivated;
	}
}
// Quarrel © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.Status.Models;
using Discord.API.Status.Rest;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Quarrel.Services.Analytics;
using Quarrel.Services.Analytics.Enums;
using Quarrel.ViewModels.SubPages.DiscordStatus.Enums;
using Quarrel.ViewModels.SubPages.DiscordStatus.Models;
using Refit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Quarrel.ViewModels.SubPages.DiscordStatus
{
    /// <summary>
    /// A ViewModel for the DiscordStatus subpage.
    /// </summary>
    public partial class DiscordStatusViewModel : ObservableObject
    {
        private readonly IAnalyticsService _analyticsService;

        private Index? _status;
        private IStatusService? _statusService;

        [AlsoNotifyChangeFor(nameof(IsLoaded))]
        [AlsoNotifyChangeFor(nameof(IsLoading))]
        [AlsoNotifyChangeFor(nameof(FailedToLoad))]
        [ObservableProperty]
        private DiscordStatusState _state;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiscordStatusViewModel"/> class.
        /// </summary>
        public DiscordStatusViewModel(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
            _state = DiscordStatusState.Loading;
            SetupAndLoad();
        }

        /// <summary>
        /// Fired when the status is loaded.
        /// </summary>
        public event EventHandler? StatusLoaded;

        /// <summary>
        /// Fired when the chart data is loaded.
        /// </summary>
        public event EventHandler? ChartDataLoaded;

        /// <summary>
        /// Gets or sets a value indicating whether or not the status has been loaded.
        /// </summary>
        public bool IsLoaded => _state == DiscordStatusState.Loaded;

        /// <summary>
        /// Gets or sets a value indicating whether or not the status is being loaded.
        /// </summary>
        public bool IsLoading => _state == DiscordStatusState.Loading;

        /// <summary>
        /// Gets or sets a value indicating whether or not the status was able to load.
        /// </summary>
        public bool FailedToLoad => _state == DiscordStatusState.FailedToLoad;

        /// <summary>
        /// Gets or sets outage index information.
        /// </summary>
        public Index? Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        /// <summary>
        /// Gets the lowest value on chart.
        /// </summary>
        public double Min { get; private set; } = 0;

        /// <summary>
        /// Gets the highest value on chart.
        /// </summary>
        public double Max { get; private set; } = 0;

        /// <summary>
        /// Gets ping times to graph.
        /// </summary>
        public List<double> Data { get; private set; } = new List<double>();

        /// <summary>
        /// Gets precise ping times.
        /// </summary>
        public Dictionary<int, Datum> DataValues { get; private set; } = new Dictionary<int, Datum>();

        /// <summary>
        /// Gets or sets list of Incidents.
        /// </summary>
        public ObservableCollection<BindableIncident> Incidents { get; set; } = new ObservableCollection<BindableIncident>();

        /// <summary>
        /// Gets or sets status notifications.
        /// </summary>
        public ObservableCollection<BindableComponent> Components { get; set; } = new ObservableCollection<BindableComponent>();

        private async void SetupAndLoad()
        {
            State = DiscordStatusState.Loading;

            var factory = new DiscordStatusRestFactory();
            _statusService = factory.GetStatusService();

            try
            {
                Status = await _statusService.GetStatus();
            }
            catch (ApiException ex)
            {
                _analyticsService.Log(LoggedEvent.DiscordStatusRequestFailed, ("Endpoint", "GetStatus"), ("Exception", ex.Message));
            }

            // Has Data
            if (Status != null)
            {
                // Loads Status
                if (Status.Status != null)
                {
                    StatusLoaded?.Invoke(this, null);
                }

                // Loads Incidents
                if (Status.Incidents != null)
                {
                    foreach (var incident in Status.Incidents)
                    {
                        if (incident.Status != "resolved")
                        {
                            var updates = new List<BindableComponent>();
                            for (int i = 0; i < incident.IncidentUpdates.Length; i++)
                            {
                                updates.Add(
                                    new BindableComponent(
                                        name: incident.IncidentUpdates[i].Status,
                                        status: incident.IncidentUpdates[i].UpdatedAt.ToString("t"),
                                        description: incident.IncidentUpdates[i].Body));
                            }

                            var component = new BindableIncident(
                                name: incident.Name, 
                                status: incident.Status, 
                                items: updates);

                            if (!string.IsNullOrWhiteSpace(component.Name))
                            {
                                Incidents.Add(component);
                            }
                        }
                    }
                }

                // Loads Components
                if (Status.Components != null)
                {
                    foreach (var component in Status.Components)
                    {
                        string? description = null;
                        if (component.Description != null)
                        {
                            description = component.Description.Replace('_', ' ');
                        }

                        var sc = new BindableComponent(
                            name: component.Name,
                            status: component.Status,
                            description: description);

                        Components.Add(sc);
                    }
                }

                ShowMetrics("day");
                State = DiscordStatusState.Loaded;
            }

            if (Status is null)
            {
                State = DiscordStatusState.FailedToLoad;
            }
        }

        /// <summary>
        /// Show metrics for a date range.
        /// </summary>
        /// <param name="duration">day, week or month.</param>
        public async void ShowMetrics(string duration)
        {
            AllMetrics? metrics = null;

            try
            {
                Guard.IsNotNull(_statusService);
                metrics = await _statusService.GetMetrics(duration);
            }
            catch (Exception ex)
            {
                _analyticsService.Log(LoggedEvent.DiscordStatusRequestFailed, ("Endpoint", "GetMetrics"), ("Exception", ex.Message));
            }

            if (metrics != null && metrics.Metrics != null && metrics.Metrics.Length > 0)
            {
                var metric = metrics.Metrics[0];
                Data.Clear();
                DataValues.Clear();
                Max = 0;
                Min = 0;
                for (var i = 0; i < metric.Data.Length; i++)
                {
                    DataValues.Add(i, metric.Data[i]);
                    Data.Add(metric.Data[i].Value);
                    if (metric.Data[i].Value > Max)
                    {
                        Max = metric.Data[i].Value;
                    }

                    if (metric.Data[i].Value < Min || Min == 0)
                    {
                        Min = metric.Data[i].Value;
                    }
                }

                ChartDataLoaded?.Invoke(this, null);
            }
        }
    }
}

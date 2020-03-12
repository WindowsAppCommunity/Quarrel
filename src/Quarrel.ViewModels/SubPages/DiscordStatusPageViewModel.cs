// Copyright (c) Quarrel. All rights reserved.

using DiscordStatusAPI.API;
using DiscordStatusAPI.API.Status;
using DiscordStatusAPI.Models;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Quarrel.ViewModels.SubPages
{
    /// <summary>
    /// Discord Status page data.
    /// </summary>
    public class DiscordStatusPageViewModel : ViewModelBase
    {
        private bool _failedToLoad = false;
        private bool _loaded = false;
        private bool _loading = false;
        private Index _status;
        private IStatusService _statusService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiscordStatusPageViewModel"/> class.
        /// </summary>
        public DiscordStatusPageViewModel()
        {
            SetupAndLoad();
        }

        /// <summary>
        /// Fired when the status is loaded.
        /// </summary>
        public event EventHandler StatusLoaded;

        /// <summary>
        /// Fired when the chart data is loaded.
        /// </summary>
        public event EventHandler ChartDataLoaded;

        /// <summary>
        /// Gets or sets a value indicating whether or not the status was able to load.
        /// </summary>
        public bool FailedToLoad
        {
            get => _failedToLoad;
            set => Set(ref _failedToLoad, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the status has been loaded.
        /// </summary>
        public bool Loaded
        {
            get => _loaded;
            set => Set(ref _loaded, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the status is being loaded.
        /// </summary>
        public bool Loading
        {
            get => _loading;
            set => Set(ref _loading, value);
        }

        /// <summary>
        /// Gets or sets outage index information.
        /// </summary>
        public Index Status
        {
            get => _status;
            set => Set(ref _status, value);
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
        public ObservableCollection<ComplexComponent> Incidents { get; set; } = new ObservableCollection<ComplexComponent>();

        /// <summary>
        /// Gets or sets status notifications.
        /// </summary>
        public ObservableCollection<SimpleComponent> Components { get; set; } = new ObservableCollection<SimpleComponent>();

        /// <summary>
        /// Show metrics for a date range.
        /// </summary>
        /// <param name="duration">day, week or month.</param>
        public async void ShowMetrics(string duration)
        {
            var metrics = await _statusService.GetMetrics(duration);
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

        private async void SetupAndLoad()
        {
            Loading = true;

            RestFactory factory = new RestFactory();
            _statusService = factory.GetStatusService();

            try
            {
                Status = await _statusService.GetStatus();
            }
            catch
            {
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
                            List<SimpleComponent> updates = new List<SimpleComponent>();
                            for (int i = 0; i < incident.IncidentUpdates.Length; i++)
                            {
                                updates.Add(new SimpleComponent()
                                {
                                    Status = incident.IncidentUpdates[i].Status,
                                    Description = incident.IncidentUpdates[i].Body,
                                    Name = incident.IncidentUpdates[i].UpdatedAt.ToString("t"),
                                });
                            }

                            ComplexComponent component = new ComplexComponent()
                            {
                                Name = incident.Name,
                                Status = incident.Status,
                                Items = updates,
                            };

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
                        SimpleComponent sc = new SimpleComponent()
                        {
                            Name = component.Name,
                            Status = component.Status.Replace("_", " "),
                            Description = component.Description,
                        };
                        Components.Add(sc);
                    }
                }

                ShowMetrics("day");
                Loaded = true;
            }

            FailedToLoad = Status == null;
            Loading = false;
        }
    }

    public class ComplexComponent
    {
        public string Description { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string Time { get; set; }
        public List<SimpleComponent> Items { get; set; }
    }

    public class SimpleComponent
    {
        public string Description { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
    }
}

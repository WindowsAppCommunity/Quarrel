using DiscordStatusAPI.API;
using DiscordStatusAPI.API.Status;
using DiscordStatusAPI.Models;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Quarrel.ViewModels.SubPages
{

    #region Classes

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

    #endregion

    public class DiscordStatusPageViewModel : ViewModelBase
    {
        #region Constructors

        public DiscordStatusPageViewModel()
        {
            Setup();
        }

        #endregion

        #region Events

        public event EventHandler StatusLoaded;

        public event EventHandler ChartDataLoaded;

        #endregion

        #region Methods

        private async void Setup()
        {
            Loading = true;

            RestFactory factory = new RestFactory();
            StatusService = factory.GetStatusService();

            try
            {
                Status = await StatusService.GetStatus();
            }
            catch { }

            FailedToLoad = Status == null;

            if (Status == null)
            {
                return;
            }

            if (Status.Status != null)
            {
                StatusLoaded?.Invoke(this, null);
            }

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
                                Name = incident.IncidentUpdates[i].UpdatedAt.ToString("t")
                            });
                        }
                        ComplexComponent component = new ComplexComponent()
                        {
                            Name = incident.Name,
                            Status = incident.Status,
                            Items = updates
                        };

                        if (!string.IsNullOrWhiteSpace(component.Name))
                            Incidents.Add(component);
                    }
                }
            }

            if (Status.Components != null)
            {
                foreach (var component in Status.Components)
                {
                    SimpleComponent sc = new SimpleComponent()
                    {
                        Name = component.Name,
                        Status = component.Status.Replace("_", " "),
                        Description = component.Description
                    };
                    Components.Add(sc);
                }
            }

            ShowMetrics("day");

            Loaded = true;
            Loading = false;
        }

        public async void ShowMetrics(string duration)
        {
            var metrics = await StatusService.GetMetrics(duration);
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
        #endregion

        #region Properties

        /// <summary>
        /// Indicates if the status was able to load
        /// </summary>
        public bool FailedToLoad
        {
            get => _FailedToLoad;
            set => Set(ref _FailedToLoad, value);
        }
        private bool _FailedToLoad;

        /// <summary>
        /// Indicates if the status has been loaded
        /// </summary>
        public bool Loaded
        {
            get => _Loaded;
            set => Set(ref _Loaded, value);
        }
        private bool _Loaded = false;

        /// <summary>
        /// Indicates if the status is being loaded
        /// </summary>
        public bool Loading
        {
            get => _Loading;
            set => Set(ref _Loading, value);
        }
        private bool _Loading = false;

        /// <summary>
        /// Outage index information
        /// </summary>
        public Index Status
        {
            get => _Status;
            set => Set(ref _Status, value);
        }
        private Index _Status;

        /// <summary>
        /// Access to Discord Status API
        /// </summary>
        private IStatusService StatusService;

        /// <summary>
        /// Lowest value on chart
        /// </summary>
        public double Min = 0;

        /// <summary>
        /// Highest value on chart
        /// </summary>
        public double Max = 0;

        /// <summary>
        /// Ping times to graph
        /// </summary>
        public readonly List<double> Data = new List<double>();

        /// <summary>
        /// Precise ping times
        /// </summary>
        public Dictionary<int, Datum> DataValues = new Dictionary<int, Datum>();

        /// <summary>
        /// List of Incidents 
        /// </summary>
        public ObservableCollection<ComplexComponent> Incidents { get; set; } = new ObservableCollection<ComplexComponent>();

        /// <summary>
        /// Status notifications
        /// </summary>
        public ObservableCollection<SimpleComponent> Components { get; set; } = new ObservableCollection<SimpleComponent>();
        #endregion
    }
}

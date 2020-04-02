// Copyright (c) Quarrel. All rights reserved.

using Windows.UI.Xaml;

namespace Quarrel.StateTriggers
{
    /// <summary>
    /// A State Trigger bound to a bool value.
    /// </summary>
    public class BooleanStateTrigger : StateTriggerBase
    {
        /// <summary>
        /// The value of the trigger binding.
        /// </summary>
        public static readonly DependencyProperty DataValueProperty =
            DependencyProperty.RegisterAttached(
                "DataValue",
                typeof(bool),
                typeof(BooleanStateTrigger),
                new PropertyMetadata(false, DataValueChanged));

        /// <summary>
        /// The active state.
        /// </summary>
        /// <remarks>
        /// Using a DependencyProperty as the backing store for TriggerValue.  This enables animation, styling, binding, etc...
        /// </remarks>
        public static readonly DependencyProperty TriggerValueProperty =
            DependencyProperty.RegisterAttached(
                "TriggerValue",
                typeof(bool),
                typeof(BooleanStateTrigger),
                new PropertyMetadata(false, TriggerValueChanged));

        /// <summary>
        /// Gets the data value.
        /// </summary>
        /// <param name="obj">Context.</param>
        /// <returns>The data value.</returns>
        public static bool GetDataValue(DependencyObject obj)
        {
            return (bool)obj.GetValue(DataValueProperty);
        }

        /// <summary>
        /// Sets the data value.
        /// </summary>
        /// <param name="obj">Context.</param>
        /// <param name="value">The new value.</param>
        public static void SetDataValue(DependencyObject obj, bool value)
        {
            obj.SetValue(DataValueProperty, value);
        }

        /// <summary>
        /// Gets the trigger state.
        /// </summary>
        /// <param name="obj">Context.</param>
        /// <returns>The trigger state.</returns>
        public static bool GetTriggerValue(DependencyObject obj)
        {
            return (bool)obj.GetValue(TriggerValueProperty);
        }

        /// <summary>
        /// Sets the trigger state.
        /// </summary>
        /// <param name="obj">Context.</param>
        /// <param name="value">The new trigger state.</param>
        public static void SetTriggerValue(DependencyObject obj, bool value)
        {
            obj.SetValue(TriggerValueProperty, value);
        }

        private static void TriggerValueChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            bool dataValue = (bool)target.GetValue(BooleanStateTrigger.DataValueProperty);
            TriggerStateCheck(target, dataValue, (bool)e.NewValue);
        }

        private static void DataValueChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            bool triggerValue = (bool)target.GetValue(BooleanStateTrigger.TriggerValueProperty);
            TriggerStateCheck(target, (bool)e.NewValue, triggerValue);
        }

        private static void TriggerStateCheck(DependencyObject target, bool dataValue, bool triggerValue)
        {
            BooleanStateTrigger trigger = target as BooleanStateTrigger;
            if (trigger == null)
            {
                return;
            }

            trigger.SetActive(triggerValue == dataValue);
        }
    }
}
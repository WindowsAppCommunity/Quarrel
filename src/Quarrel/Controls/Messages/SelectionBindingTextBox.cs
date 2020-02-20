// Copyright (c) Quarrel. All rights reserved.

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Controls.Messages
{
    /// <summary>
    /// TextBox with a twoway binding on the curser position.
    /// </summary>
    public class SelectionBindingTextBox : TextBox
    {
        /// <summary>
        /// A property representing the position of the curser.
        /// </summary>
        public static readonly DependencyProperty BindableSelectionStartProperty =
            DependencyProperty.Register(
            "BindableSelectionStart",
            typeof(int),
            typeof(SelectionBindingTextBox),
            new PropertyMetadata(0, OnBindableSelectionStartChanged));

        /// <summary>
        /// A property representing the selected values by the curser.
        /// </summary>
        public static readonly DependencyProperty BindableSelectionLengthProperty =
            DependencyProperty.Register(
            "BindableSelectionLength",
            typeof(int),
            typeof(SelectionBindingTextBox),
            new PropertyMetadata(0, OnBindableSelectionLengthChanged));

        /// <summary>
        /// A property representing the Text in the TextBox.
        /// </summary>
        public static readonly DependencyProperty BindableTextProperty =
            DependencyProperty.Register(
            "BindableText",
            typeof(string),
            typeof(SelectionBindingTextBox),
            new PropertyMetadata(0, OnBindableTextChanged));

        /// <summary>
        /// Determines if a change is coming from the user or programmatically.
        /// </summary>
        private bool _programmaticChange;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectionBindingTextBox"/> class.
        /// </summary>
        public SelectionBindingTextBox() : base()
        {
            this.SelectionChanged += this.OnSelectionChanged;
            this.TextChanged += OnTextChanged;
        }

        /// <summary>
        /// Gets or sets the text into the draft.
        /// </summary>
        public string BindableText
        {
            get
            {
                return (string)this.GetValue(BindableTextProperty);
            }

            set
            {
                this.SetValue(BindableTextProperty, value);
                Text = value;
            }
        }

        /// <summary>
        /// Gets or sets the position of the curser.
        /// </summary>
        public int BindableSelectionStart
        {
            get
            {
                return (int)this.GetValue(BindableSelectionStartProperty);
            }

            set
            {
                this.SetValue(BindableSelectionStartProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets selected values from curser.
        /// </summary>
        public int BindableSelectionLength
        {
            get
            {
                return (int)this.GetValue(BindableSelectionLengthProperty);
            }

            set
            {
                this.SetValue(BindableSelectionLengthProperty, value);
            }
        }

        /// <summary>
        /// Updates BindableText.
        /// </summary>
        private static void OnBindableTextChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var textBox = dependencyObject as SelectionBindingTextBox;
            textBox.BindableText = (string)args.NewValue;
        }

        /// <summary>
        /// Updates BindableSelectionStart.
        /// </summary>
        private static void OnBindableSelectionStartChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var textBox = dependencyObject as SelectionBindingTextBox;

            if (textBox._programmaticChange)
            {
                int newValue = (int)args.NewValue;
                textBox.SelectionStart = newValue;
            }
            else
            {
                textBox._programmaticChange = true;
            }
        }

        /// <summary>
        /// Updates BindableSelectionLength.
        /// </summary>
        private static void OnBindableSelectionLengthChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var textBox = dependencyObject as SelectionBindingTextBox;

            if (textBox._programmaticChange)
            {
                int newValue = (int)args.NewValue;
                textBox.SelectionLength = newValue;
            }
            else
            {
                textBox._programmaticChange = true;
            }
        }

        /// <summary>
        /// Updates bindable text when TextBox text changes.
        /// </summary>
        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.BindableText != this.Text)
            {
                this.BindableText = this.Text;
            }
        }

        /// <summary>
        /// Updates bindable selection positions when TextBox selection changes.
        /// </summary>
        private void OnSelectionChanged(object sender, RoutedEventArgs e)
        {
            if (this.BindableSelectionStart != this.SelectionStart)
            {
                this._programmaticChange = false;
                this.BindableSelectionStart = this.SelectionStart;
            }

            if (this.BindableSelectionLength != this.SelectionLength)
            {
                this._programmaticChange = false;
                this.BindableSelectionLength = this.SelectionLength;
            }
        }
    }
}

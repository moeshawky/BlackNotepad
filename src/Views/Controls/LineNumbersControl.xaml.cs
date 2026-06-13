using System;
using System.Windows;
using System.Windows.Controls;

namespace Savaged.BlackNotepad.Views.Controls
{
    /// <summary>
    /// Displays line numbers in a gutter column that synchronizes its
    /// vertical scroll offset with the associated TextBox.
    /// </summary>
    public partial class LineNumbersControl : UserControl
    {
        /// <summary>
        /// Identifies the LineCount dependency property, which determines
        /// how many line numbers are rendered.
        /// </summary>
        public static readonly DependencyProperty LineCountProperty =
            DependencyProperty.Register(
                nameof(LineCount),
                typeof(int),
                typeof(LineNumbersControl),
                new PropertyMetadata(1, OnLineCountChanged));

        /// <summary>
        /// Identifies the ScrollOffset dependency property, which sets the
        /// vertical scroll offset in pixels to synchronize with the TextBox.
        /// </summary>
        public static readonly DependencyProperty ScrollOffsetProperty =
            DependencyProperty.Register(
                nameof(ScrollOffset),
                typeof(double),
                typeof(LineNumbersControl),
                new PropertyMetadata(0.0, OnScrollOffsetChanged));

        /// <summary>
        /// Identifies the LineHeight dependency property, which specifies the
        /// pixel height of each line for correct scroll alignment.
        /// </summary>
        public static readonly DependencyProperty LineHeightProperty =
            DependencyProperty.Register(
                nameof(LineHeight),
                typeof(double),
                typeof(LineNumbersControl),
                new PropertyMetadata(16.0, OnLineHeightChanged));

        /// <summary>
        /// Gets or sets the total number of lines to display as line numbers.
        /// </summary>
        /// <value>Positive integer representing the line count. Minimum is 1.</value>
        public int LineCount
        {
            get => (int)GetValue(LineCountProperty);
            set => SetValue(LineCountProperty, value);
        }

        /// <summary>
        /// Gets or sets the vertical scroll offset in pixels. This value is
        /// synchronized with the main TextBox's vertical scroll position.
        /// </summary>
        /// <value>Non-negative double representing the vertical offset in device-independent pixels.</value>
        public double ScrollOffset
        {
            get => (double)GetValue(ScrollOffsetProperty);
            set => SetValue(ScrollOffsetProperty, value);
        }

        /// <summary>
        /// Gets or sets the pixel height of each line. Used to calculate
        /// the correct scroll offset for synchronization.
        /// </summary>
        /// <value>Positive double representing line height in device-independent pixels.</value>
        public double LineHeight
        {
            get => (double)GetValue(LineHeightProperty);
            set => SetValue(LineHeightProperty, value);
        }

        /// <summary>
        /// Initializes a new instance of the LineNumbersControl class.
        /// </summary>
        public LineNumbersControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Rebuilds the line number text when the line count changes.
        /// </summary>
        /// <param name="d">The dependency object whose property changed.</param>
        /// <param name="e">Event data containing old and new values.</param>
        private static void OnLineCountChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LineNumbersControl control)
            {
                control.UpdateLineNumbers();
            }
        }

        /// <summary>
        /// Adjusts the scroll viewer's vertical offset when the scroll offset changes.
        /// </summary>
        /// <param name="d">The dependency object whose property changed.</param>
        /// <param name="e">Event data containing old and new values.</param>
        private static void OnScrollOffsetChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LineNumbersControl control)
            {
                control.LineNumberScrollViewer.ScrollToVerticalOffset(
                    (double)e.NewValue);
            }
        }

        /// <summary>
        /// Rebuilds the line number text when the line height changes,
        /// since line height affects the scroll viewer extent.
        /// </summary>
        /// <param name="d">The dependency object whose property changed.</param>
        /// <param name="e">Event data containing old and new values.</param>
        private static void OnLineHeightChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LineNumbersControl control)
            {
                control.UpdateLineNumbers();
            }
        }

        /// <summary>
        /// Regenerates the line number text by appending newline-separated
        /// numbers from 1 to LineCount.
        /// </summary>
        private void UpdateLineNumbers()
        {
            var count = Math.Max(1, LineCount);
            var lines = new string[count];
            for (int i = 0; i < count; i++)
            {
                lines[i] = (i + 1).ToString();
            }
            LineNumberTextBlock.Text = string.Join(Environment.NewLine, lines);
        }
    }
}

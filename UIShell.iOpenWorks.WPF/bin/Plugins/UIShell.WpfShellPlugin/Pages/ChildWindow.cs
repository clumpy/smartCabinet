using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows.Input;

namespace UIShell.WpfShellPlugin.Pages
{
    public class ChildWindow
    {
        private Rectangle maskRectangle = new Rectangle { Fill = new SolidColorBrush(Colors.DarkGray), Opacity = 0.0 };

        public FrameworkElement Parent
        {
            get;
            set;
        }

        public FrameworkElement Content
        {
            get;
            set;
        }

        public void Show()
        {
            Grid grid = GetRootGrid();

            if (grid != null)
            {
                DoubleAnimation opacityAnimation = new DoubleAnimation(0.5, new Duration(TimeSpan.FromSeconds(0.1)));

                Storyboard opacityBoard = new Storyboard();
                opacityBoard.Children.Add(opacityAnimation);

                Storyboard.SetTarget(opacityAnimation, maskRectangle);
                Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath("(Opacity)"));

                opacityBoard.Completed += delegate
                {
                    ScaleTransform scaleTransform = new ScaleTransform(0.0, 0.0, Content.Width / 2.0, Content.Height / 2.0);
                    Content.RenderTransform = scaleTransform;

                    grid.Children.Add(Content);
                    if (grid.RowDefinitions.Count > 0)
                    {
                        Grid.SetRowSpan(Content, grid.RowDefinitions.Count);
                    }
                    if (grid.ColumnDefinitions.Count > 0)
                    {
                        Grid.SetColumnSpan(Content, grid.ColumnDefinitions.Count);
                    }

                    Storyboard scaleBoard = new Storyboard();
                    EasingFunctionBase easing = new ElasticEase()
                    {
                        EasingMode = EasingMode.EaseOut,
                        Oscillations = 1,
                        Springiness = 3,
                    };

                    DoubleAnimation scaleXAnimation = new DoubleAnimation(1.0, TimeSpan.FromSeconds(0.3));
                    scaleXAnimation.EasingFunction = easing;

                    scaleBoard.Children.Add(scaleXAnimation);

                    Storyboard.SetTarget(scaleXAnimation, Content);
                    Storyboard.SetTargetProperty(scaleXAnimation, new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleX)"));

                    DoubleAnimation scaleYAnimation = new DoubleAnimation(1.0, TimeSpan.FromSeconds(0.3));
                    scaleYAnimation.EasingFunction = easing;
                    scaleBoard.Children.Add(scaleYAnimation);

                    Storyboard.SetTarget(scaleYAnimation, Content);
                    Storyboard.SetTargetProperty(scaleYAnimation, new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleY)"));

                    scaleBoard.Begin();
                };

                opacityBoard.Begin();

                grid.Children.Add(maskRectangle);
                if (grid.RowDefinitions.Count > 0)
                {
                    Grid.SetRowSpan(maskRectangle, grid.RowDefinitions.Count);
                }
                if (grid.ColumnDefinitions.Count > 0)
                {
                    Grid.SetColumnSpan(maskRectangle, grid.ColumnDefinitions.Count);
                }

            }
        }

        public void Close()
        {
            Grid grid = GetRootGrid();


            if (grid != null)
            {
                ScaleTransform scaleTransform = new ScaleTransform(1.0, 1.0, Content.Width / 2.0, Content.Height / 2.0);
                Content.RenderTransform = scaleTransform;

                Storyboard scaleBoard = new Storyboard();

                DoubleAnimation scaleXAnimation = new DoubleAnimation(0.0, TimeSpan.FromSeconds(0.3));

                scaleBoard.Children.Add(scaleXAnimation);

                Storyboard.SetTarget(scaleXAnimation, Content);
                Storyboard.SetTargetProperty(scaleXAnimation, new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleX)"));

                DoubleAnimation scaleYAnimation = new DoubleAnimation(0.0, TimeSpan.FromSeconds(0.3));

                scaleBoard.Children.Add(scaleYAnimation);

                Storyboard.SetTarget(scaleYAnimation, Content);
                Storyboard.SetTargetProperty(scaleYAnimation, new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleY)"));

                scaleBoard.Completed += delegate
                {
                    DoubleAnimation opacityAnimation = new DoubleAnimation(0.5, 0.0, new Duration(TimeSpan.FromSeconds(0.1)));

                    Storyboard opacityBoard = new Storyboard();
                    opacityBoard.Children.Add(opacityAnimation);

                    Storyboard.SetTarget(opacityAnimation, maskRectangle);
                    Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath("(Opacity)"));

                    opacityBoard.Completed += delegate
                    {
                        grid.Children.Remove(maskRectangle);
                        grid.Children.Remove(Content);
                    };

                    opacityBoard.Begin();
                };

                scaleBoard.Begin();
            }
        }

        private Grid GetRootGrid()
        {
            FrameworkElement root = Parent;

            while (root is FrameworkElement && root.Parent != null)
            {
                FrameworkElement rootElement = root as FrameworkElement;

                if (rootElement.Parent is FrameworkElement)
                {
                    root = rootElement.Parent as FrameworkElement;
                }
            }

            ContentControl contentControl = root as ContentControl;
            return contentControl.Content as Grid;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace JosephusProblem
{
    internal class Josephus
    {
        private Canvas canvas;
        private int soldiers;
        double mainRadius;
        double centerX, centerY;
        double border = 100;
        double soldierRadius = 70;
        List<Soldier> lstSoldiers = new List<Soldier>();
        Brush colorLive = Brushes.DodgerBlue;
        Brush colorDead = Brushes.Silver;
        Soldier actualSoldier;
        int correctId = -1;
        Ellipse bullet;
        bool skipClick = false;

        public Josephus(Canvas canvas, int soldiers)
        {
            canvas.Background = Brushes.White;

            var bits = Convert.ToString(soldiers, 2);
            var newbits = bits.Substring(1) + "1";
            correctId = Convert.ToInt32(newbits, 2);

            this.canvas = canvas;
            this.soldiers = soldiers;
            var minSize = Math.Min(canvas.ActualHeight, canvas.ActualWidth);
            this.mainRadius = minSize / 2 - border;
            this.centerX = canvas.ActualWidth / 2;
            this.centerY = canvas.ActualHeight / 2;

            // main circle kruh
            var mainCircle = new Ellipse() { Stroke = Brushes.Silver, StrokeThickness = 1 };
            mainCircle.Width = mainCircle.Height = this.mainRadius * 2;
            Canvas.SetLeft(mainCircle, centerX - mainRadius);
            Canvas.SetTop(mainCircle, centerY - mainRadius);
            canvas.Children.Add(mainCircle);
            /*
X = Cx + (r * cosine(angle))
Y = Cy + (r * sine(angle))
*/
            // soldiers
            var angleParts = 2 * Math.PI / (this.soldiers);

            double ang = 0;
            for (int i = 0; i < soldiers; i++)
            {

                var x = centerX + (mainRadius * Math.Cos(ang));
                var y = centerY + (mainRadius * Math.Sin(ang));
                addSoldier(i + 1, x, y);
                ang += angleParts;
            }

            for (int i = 0; i < lstSoldiers.Count - 1; i++)
            {
                lstSoldiers[i].Next = lstSoldiers[i + 1];
            }
            lstSoldiers[lstSoldiers.Count - 1].Next = lstSoldiers[0]; // toroidal
            this.actualSoldier = lstSoldiers[0];

            var tb = new TextBlock { Text = correctId.ToString(), FontSize = 32 };
            Canvas.SetLeft(tb, centerX);
            Canvas.SetTop(tb, centerY);
            canvas.Children.Add(tb);


            this.bullet = new Ellipse() { Width = 20, Height = 20, Fill = Brushes.Green };


            var left = Canvas.GetLeft(actualSoldier.Circle);
            var top = Canvas.GetTop(actualSoldier.Circle);
            Canvas.SetLeft(bullet, left);
            Canvas.SetTop(bullet, top);

            canvas.Children.Add(bullet);

        }

        internal void ScanOne()
        {
            if( skipClick )
            {
                return;
            }
            var nextToKill = findNextLive(actualSoldier);
            if (!nextToKill.Equals(actualSoldier))
            {
                skipClick = true;
                Kill(nextToKill);
            }
            else
            {
                canvas.Background = Brushes.Gray;
            }
        }

        private void Kill(Soldier soldierMustDie)
        {
            shotSoldier(soldierMustDie);
            actualSoldier = findNextLive(soldierMustDie);

        }

        private void shotSoldier(Soldier soldierMustDie)
        {
            var left = Canvas.GetLeft(actualSoldier.Circle);
            var top = Canvas.GetTop(actualSoldier.Circle);
            Canvas.SetLeft(bullet, left);
            Canvas.SetTop(bullet, top);
            soldierMustDie.Circle.Fill = Brushes.Red;
            soldierMustDie.IsLive = false;
            MoveTo(bullet, actualSoldier, soldierMustDie);
        }


        public void MoveTo(Ellipse bullet, Soldier fromSoldier, Soldier toSoldier)
        {
            Point oldP = new Point();
            oldP.X = Canvas.GetLeft(fromSoldier.Circle);
            oldP.Y = Canvas.GetTop(fromSoldier.Circle);

            Point newP = new Point();
            newP.X = Canvas.GetLeft(toSoldier.Circle);
            newP.Y = Canvas.GetTop(toSoldier.Circle);


            DoubleAnimation anim1 = new DoubleAnimation(oldP.X, newP.X, TimeSpan.FromSeconds(0.4));
            //anim1.EasingFunction = new System.Windows.Media.Animation.BackEase();
            DoubleAnimation anim2 = new DoubleAnimation(oldP.Y, newP.Y, TimeSpan.FromSeconds(0.4));
            anim2.EasingFunction = new System.Windows.Media.Animation.PowerEase();

            var toCircle = toSoldier.Circle;
            anim1.Completed += (o, e) =>
            {
                toCircle.Fill = colorDead;
                this.MoveTo2(toSoldier, actualSoldier);

            };

            bullet.Fill = Brushes.Red;
            bullet.BeginAnimation(Canvas.LeftProperty, anim1);
            bullet.BeginAnimation(Canvas.TopProperty, anim2);
        }

        public void MoveTo2(Soldier fromSoldier, Soldier toSoldier)
        {
            Point oldP = new Point();
            oldP.X = Canvas.GetLeft(fromSoldier.Circle);
            oldP.Y = Canvas.GetTop(fromSoldier.Circle);

            Point newP = new Point();
            newP.X = Canvas.GetLeft(toSoldier.Circle);
            newP.Y = Canvas.GetTop(toSoldier.Circle);


            DoubleAnimation anim1 = new DoubleAnimation(oldP.X, newP.X, TimeSpan.FromSeconds(1.8));
            //anim1.EasingFunction = new System.Windows.Media.Animation.BackEase();
            DoubleAnimation anim2 = new DoubleAnimation(oldP.Y, newP.Y, TimeSpan.FromSeconds(1.8));
            anim2.EasingFunction = new System.Windows.Media.Animation.PowerEase();

            bullet.Fill = Brushes.Green;

            anim1.Completed += (o, e) =>
            {
                skipClick = false;
            };

            bullet.BeginAnimation(Canvas.LeftProperty, anim1);
            bullet.BeginAnimation(Canvas.TopProperty, anim2);
        }




        private Soldier findNextLive(Soldier fromSoldier)
        {
            Soldier snew = null;
            Soldier scur = fromSoldier;

            while (true)
            {
                if (scur.Next.IsLive)
                {
                    snew = scur.Next;
                    break;
                }
                scur = scur.Next;
            }
            return snew;
        }

        private void addSoldier(int id, double x, double y)
        {
            var circle = new Ellipse() { Stroke = Brushes.Silver, StrokeThickness = 1, Fill = colorLive, ToolTip = id.ToString() };
            circle.Width = circle.Height = soldierRadius;
            var cx = x - soldierRadius / 2.0;
            var cy = y - soldierRadius / 2.0;
            Canvas.SetLeft(circle, cx);
            Canvas.SetTop(circle, cy);
            canvas.Children.Add(circle);

            var tb = new TextBlock { Text = id.ToString(), FontWeight = FontWeights.Bold, Foreground = Brushes.White, FontSize = 24 };
            //var ff = new FormattedText(id.ToString(), null, FlowDirection.LeftToRight, tb.FontFamily, )
            Canvas.SetLeft(tb, x - 10);
            Canvas.SetTop(tb, y - 10);
            canvas.Children.Add(tb);
            var sld = new Soldier() { Id = id, Circle = circle, IsLive = true };

            lstSoldiers.Add(sld);

        }
    }



    public class Soldier
    {
        public int Id { get; set; }

        public Ellipse Circle { get; set; }

        public bool IsLive { get; set; }


        public Soldier Next { get; set; }
    }
}
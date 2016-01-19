﻿using EscapeRunner.Animations;
using EscapeRunner.View;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace EscapeRunner.BusinessLogic.GameObjects
{
    public class Player : IDrawable
    {
        private static PlayerAnimation playerAnimation;
        private static Player playerInstance;
        private static int windowButtomMargin = 90;
        private static int windowSideMargin = 70;
        bool isFired = false;
        private Player()
        {
            AnimationFactory factory = new AnimationFactory();
            playerAnimation = (PlayerAnimation)factory.GetAnimationCommandResult();

            // Initialize the player location to the top of the screen
            playerAnimation.AnimationPosition = MapLoader.PlayerStartLocation;

            Direction = Directions.Right;
        }

        public static Directions Direction { get; set; }

        /// <summary>
        /// Differential movement element in the X direction
        /// </summary>
        public static int Dx { get; } = 4;

        /// <summary>
        /// Differential movement element in the Y direction
        /// </summary>
        public static int Dy { get; } = 4;

        public static Player PlayerInstance
        {
            get { return playerInstance == null ? playerInstance = new Player() : playerInstance; }
            private set { playerInstance = value; }
        }

        public static Point Position { get { return playerAnimation.AnimationPosition; } }

        /// <summary>
        /// Position of the player
        /// </summary>
        public void Move(Directions direction)
        {
            // Move the animation of the player
            if (CanMove(direction))
            {
                Move(direction, Dx, Dy);
            }
        }

        public void UpdateGraphics(Graphics g)
        {
            playerAnimation.Draw(g, Direction);
        }

        private static bool CanMove(Directions direction)
        {
            // Check if the player reached the bounds of the screen
            Point isoPosition = playerAnimation.AnimationPosition.TwoDimensionsToIso();

            Point isoScreenEdge = new Point(MainWindow.RightBound, MainWindow.LowerBound).TwoDimensionsToIso();
            switch (direction)
            {
                case Directions.Up:
                    return isoPosition.Y - Dy >= MainWindow.UpperBound ? true : false;

                case Directions.Down:
                    return isoPosition.Y + Dy <= MainWindow.LowerBound - windowButtomMargin ? true : false;

                case Directions.Left:
                    return isoPosition.X - Dx >= MainWindow.LeftBound ? true : false;

                case Directions.Right:
                    return isoPosition.X + Dx <= MainWindow.RightBound - windowSideMargin ? true : false;
            }
            // Invalid case
            return false;
        }

        /// <summary>
        /// Moves on the 2D Cartesian coordinates, conversion is on checking and drawing only
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="deltaHorizontal"></param>
        /// <param name="deltaVertical"></param>
        private async void Move(Directions direction, int deltaHorizontal, int deltaVertical)
        {
            // Get a handle to the player position because we can't change the contents of the points
            Point newPosition = playerAnimation.AnimationPosition;

            switch (direction)
            {
                case Directions.Up:
                    {
                        if (!isFired)
                        {
                            isFired = true;
                            await Task.Run(() =>
                            {
                                int counter = 0;
                                while (counter < 8)
                                {
                                    newPosition.Y -= deltaVertical;

                                    playerAnimation.AnimationPosition = newPosition;
                                    Thread.Sleep(100);
                                    counter++;
                                }
                                isFired = false;
                            });
                        }
                    }
                    // newPosition.Y -= deltaVertical;
                    break;

                case Directions.Down:
                    newPosition.Y += deltaVertical;
                    break;

                case Directions.Left:
                    newPosition.X -= deltaHorizontal;
                    break;

                case Directions.Right:
                    newPosition.X += deltaVertical;
                    break;
            }

            // Change the displayed image ( enable this when the image is constant when the player
            // is idle ) and disable the same method in PlayerAnimation.cs
            playerAnimation.LoadNextAnimationImage();

            Direction = direction;
            playerAnimation.AnimationPosition = newPosition;
        }
    }
}
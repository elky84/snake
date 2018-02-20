using System;
using System.Collections.Generic;
using System.Diagnostics;
using static System.Console;

namespace Snake
{
    class Program
    {
        static void Main ()
        {
            var rand = new Random ();

            var score = 5;

            var head = new Pixel (WindowWidth / 2, WindowHeight / 2, ConsoleColor.Red);
            var berry = new Pixel (toEvenNumber(rand.Next (1, WindowWidth - 2)), rand.Next (1, WindowHeight - 2),     ConsoleColor.Cyan);

            var body = new List<Pixel> ();

            var currentMovement = Direction.Right;

            var gameover = false;


            DrawBorder ();
            while (true)
            {
                gameover |= (head.XPos == WindowWidth - 1 || head.XPos == 0 || head.YPos == WindowHeight - 1 || head.YPos == 0);

                if (berry.XPos == head.XPos && berry.YPos == head.YPos)
                {
                    score++;
                    berry = new Pixel (toEvenNumber(rand.Next (1, WindowWidth - 2)), rand.Next (1, WindowHeight -     2), ConsoleColor.Cyan);
                }

                for (int i = 0; i < body.Count; i++)
                {
                    DrawPixel (body[i]);
                    gameover |= (body[i].XPos == head.XPos && body[i].YPos == head.YPos);
                }

                if (gameover)
                {
                    break;
                }

                DrawPixel (head);
                DrawPixel (berry);

                var sw = Stopwatch.StartNew();
                while (sw.ElapsedMilliseconds <= 500)
                {
                    currentMovement = ReadMovement (currentMovement);
                }

                body.Add (new Pixel (head.XPos, head.YPos, ConsoleColor.Green));

                switch (currentMovement)
                {
                    case Direction.Up:
                        head.YPos--;
                        break;
                    case Direction.Down:
                        head.YPos++;
                        break;
                    case Direction.Left:
                        head.XPos-=2;
                        break;
                    case Direction.Right:
                        head.XPos+=2;
                        break;
                }

                if (body.Count > score)
                {
                    RemovePixel(body[0]);
                    body.RemoveAt (0);
                }
            }
            SetCursorPosition (WindowWidth / 5, WindowHeight / 2);
            WriteLine ($"Game over, Score: {score - 5}");
            SetCursorPosition (WindowWidth / 5, WindowHeight / 2 + 1);
            ReadKey ();
        }

        static int toEvenNumber(int value)
        {
            return value % 2 == 0 ? value : value - value % 2;
        }

        static Direction ReadMovement (Direction movement)
        {
            if (KeyAvailable)
            {
                var key = ReadKey(true).Key;
                if (key == ConsoleKey.UpArrow && movement != Direction.Down)
                {
                    movement = Direction.Up;
                }
                else if (key == ConsoleKey.DownArrow && movement != Direction.Up)
                {
                    movement = Direction.Down;
                }
                else if (key == ConsoleKey.LeftArrow && movement != Direction.Right)
                {
                    movement = Direction.Left;
                }
                else if (key == ConsoleKey.RightArrow && movement != Direction.Left)
                {
                    movement = Direction.Right;
                }
            }

            return movement;
        }

        static void DrawPixel (Pixel pixel)
        {
            SetCursorPosition (pixel.XPos, pixel.YPos);
            ForegroundColor = pixel.ScreenColor;
            Write ("■");
            SetCursorPosition(0, 0);
        }


        static void RemovePixel (Pixel pixel)
        {
            SetCursorPosition (pixel.XPos, pixel.YPos);
            ForegroundColor = ConsoleColor.Black;
            Write ("■");
            SetCursorPosition(0, 0);
        }
        static void DrawBorder ()
        {
            for (int i = 0; i < WindowWidth; i += 2)
            {
                SetCursorPosition (i, 0);
                Write ("■");

                SetCursorPosition (i, WindowHeight - 1);
                Write ("■");
            }

            for (int i = 0; i < WindowHeight; i++)
            {
                SetCursorPosition (0, i);
                Write ("■");

                SetCursorPosition (WindowWidth - 2, i);
                Write ("■");
            }
        }

        struct Pixel
        {
            public Pixel (int xPos, int yPos, ConsoleColor color)
            {
                XPos = xPos;
                YPos = yPos;
                ScreenColor = color;
            }
            public int XPos { get; set; }
            public int YPos { get; set; }
            public ConsoleColor ScreenColor { get; set; }
        }

        enum Direction
        {
            Up,
            Down,
            Right,
            Left
        }
    }
}

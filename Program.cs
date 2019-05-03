using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static System.Console;

namespace Snake
{
    public class Program
    {
        private static readonly Random Rand = new Random();

        public static void Main()
        {
            var score = 5;

            var head = new Pixel(WindowWidth / 2, WindowHeight / 2, ConsoleColor.Red);
            var body = new List<Pixel>();

            var berry = GetBerryPos(head, body);

            var currentMovement = Direction.Right;

            var gameOver = false;

            CursorVisible = false;

            DrawBorder();
            while (true)
            {
                gameOver |= (head.XPos == WindowWidth - 1 || head.XPos == 0 || head.YPos == WindowHeight - 1 || head.YPos == 0);

                if (berry.XPos == head.XPos && berry.YPos == head.YPos)
                {
                    score++;
                    berry = GetBerryPos(head, body);
                }

                foreach (var pixel in body)
                {
                    DrawPixel(pixel);
                    gameOver |= (pixel.XPos == head.XPos && pixel.YPos == head.YPos);
                }

                if (gameOver)
                {
                    break;
                }

                DrawPixel(head);
                DrawPixel(berry);

                var sw = Stopwatch.StartNew();
                while (sw.ElapsedMilliseconds <= 500)
                {
                    var oldMovement = currentMovement;
                    bool pressed;

                    (currentMovement, pressed) = ReadMovement(currentMovement);

                    if (currentMovement == oldMovement && !pressed)
                    {
                        continue;
                    }

                    if (currentMovement == Direction.Quit)
                    {
                        gameOver = true;
                    }
                    break;
                }

                body.Add(new Pixel(head.XPos, head.YPos, ConsoleColor.Green));

                switch (currentMovement)
                {
                    case Direction.Up:
                        head.YPos--;
                        break;
                    case Direction.Down:
                        head.YPos++;
                        break;
                    case Direction.Left:
                        head.XPos -= 2;
                        break;
                    case Direction.Right:
                        head.XPos += 2;
                        break;
                }

                if (body.Count > score)
                {
                    RemovePixel(body[0]);
                    body.RemoveAt(0);
                }
            }
            SetCursorPosition(WindowWidth / 5, WindowHeight / 2);
            WriteLine($"Game over, Score: {score - 5}");
            SetCursorPosition(WindowWidth / 5, WindowHeight / 2 + 1);
            CursorVisible = true;
            ReadKey();
        }

        private static Pixel GetBerryPos(Pixel head, IReadOnlyCollection<Pixel> body)
        {
            Pixel berry;
            do
            {
                berry = new Pixel(ToEvenNumber(Rand.Next(1, WindowWidth - 2)), Rand.Next(1, WindowHeight - 2), ConsoleColor.Cyan);
            } while ((berry.XPos == head.XPos && berry.YPos == head.YPos) || body.Any(b => berry.XPos == b.XPos && berry.YPos == b.YPos));

            return berry;
        }

        private static int ToEvenNumber(int value)
        {
            return value % 2 == 0 ? value : value + 1;
        }

        private static (Direction Direction, bool KeyPressed) ReadMovement(Direction movement)
        {
            if (!KeyAvailable)
            {
                return (movement, false);
            }

            var key = ReadKey(true).Key;

            switch (key)
            {
                case ConsoleKey.UpArrow when movement != Direction.Down:
                    movement = Direction.Up;
                    break;
                case ConsoleKey.DownArrow when movement != Direction.Up:
                    movement = Direction.Down;
                    break;
                case ConsoleKey.LeftArrow when movement != Direction.Right:
                    movement = Direction.Left;
                    break;
                case ConsoleKey.RightArrow when movement != Direction.Left:
                    movement = Direction.Right;
                    break;
                case ConsoleKey.Escape:
                    movement = Direction.Quit;
                    break;
            }
            return (movement, true);
        }

        private static void DrawPixel(Pixel pixel)
        {
            SetCursorPosition(pixel.XPos, pixel.YPos);
            ForegroundColor = pixel.ScreenColor;
            Write("■");
            SetCursorPosition(0, 0);
        }


        private static void RemovePixel(Pixel pixel)
        {
            SetCursorPosition(pixel.XPos, pixel.YPos);
            ForegroundColor = ConsoleColor.Black;
            Write("■");
            SetCursorPosition(0, 0);
        }

        private static void DrawBorder()
        {
            for (var i = 0; i < WindowWidth; i += 2)
            {
                SetCursorPosition(i, 0);
                Write("■");

                SetCursorPosition(i, WindowHeight - 1);
                Write("■");
            }

            for (var i = 0; i < WindowHeight; i++)
            {
                SetCursorPosition(0, i);
                Write("■");

                SetCursorPosition(WindowWidth - 2, i);
                Write("■");
            }
        }

        private struct Pixel
        {
            public Pixel(int xPos, int yPos, ConsoleColor color)
            {
                XPos = xPos;
                YPos = yPos;
                ScreenColor = color;
            }
            public int XPos { get; set; }
            public int YPos { get; set; }
            public ConsoleColor ScreenColor { get; }
        }

        private enum Direction
        {
            Up,
            Down,
            Right,
            Left,
            Quit
        }
    }
}

using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Collections.Generic;
using static System.Console;

class Program
{
	static Random random = new Random();
	static int xwins = 0;
	static int owins = 0;
	
	static void Swap(List<Point> list, int leftIndex, int rightIndex)
	{
		Point temp = list[rightIndex];
		list[rightIndex] = list[leftIndex];
		list[leftIndex] = temp;
	}
	
	static void Shuffle(List<Point> list)
	{
		for (int i = list.Count - 1; i > 0; i--)
		{
		  int n = random.Next(i + 1);
		  Swap(list, i, n);
		}
	}
	
	static void SaveStats()
	{
		TextWriter tw = new StreamWriter("savestats.txt");
		
		tw.WriteLine(xwins);
		tw.WriteLine(owins);
		
		tw.Close();
	}
	
	static void LoadStats()
	{
		if (!File.Exists("savestats.txt"))
		{
			SaveStats();
		}
		
		TextReader tr = new StreamReader("savestats.txt");
		
		string xwinsString = tr.ReadLine();
		string owinsString = tr.ReadLine();
		
		xwins = Convert.ToInt32(xwinsString);
		owins = Convert.ToInt32(owinsString);
		
		tr.Close();
	}
	
	class Board
	{
		public char[,] grid = new char[3,3] { 
		{ ' ', ' ', ' ' }, 
		{ ' ', ' ', ' ' }, 
		{ ' ', ' ', ' ' } };
		
		public List<Point> GetPossibleMoves()
		{
			var possMoves = new List<Point>();
			
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					if (grid[i, j] == ' ')
					{
						possMoves.Add(new Point(i, j));
					}
				}
			}
			
			return possMoves;
		}
		
		public void OpponentTurn()
		{
			List<Point> possible = GetPossibleMoves();
			Shuffle(possible);
			bool placed = false;
			
			foreach (Point p in possible)
			{
				grid[p.X, p.Y] = 'X';
				if (WinCheck())
				{
					placed = true;
					break;
				}
				else
				{
					grid[p.X, p.Y] = ' ';
				}
			}
				
			if (!placed)
			{
				foreach (Point p in possible)
				{
					grid[p.X, p.Y] = 'O';
					if (WinCheck())
					{
						placed = true;
						grid[p.X, p.Y] = 'X';
						break;
					}
					else
					{
						grid[p.X, p.Y] = ' ';
					}
				}
			}
			
			if (!placed)
			{
				Point sel = possible[0];
				grid[sel.X, sel.Y] = 'X';
			}
		}
		
		public void PlayerTurn(Cursor c)
		{
			bool cont = false;

			while (!cont)
			{
				c.Set();
				switch (ReadKey(true).Key)
				{
					case ConsoleKey.UpArrow: c.Up(); break;
					case ConsoleKey.DownArrow: c.Down(); break;
					case ConsoleKey.LeftArrow: c.Left(); break;
					case ConsoleKey.RightArrow: c.Right(); break;
					case ConsoleKey.Enter: 
						if (grid[c.Y, c.X] == ' ') 
						{
							grid[c.Y, c.X] = 'O'; 
							cont = true; 
						}
						break;
					default: break;
				}
			}
		}
		
		public bool WinCheck()
		{
			bool hasWon = false;
			
			// Vertical
			if (grid[0, 0] == grid[0, 1] && grid[0, 1] == grid[0, 2] && grid[0, 2] != ' ') { hasWon = true; }
			if (grid[1, 0] == grid[1, 1] && grid[1, 1] == grid[1, 2] && grid[1, 2] != ' ') { hasWon = true; }
			if (grid[2, 0] == grid[2, 1] && grid[2, 1] == grid[2, 2] && grid[2, 2] != ' ') { hasWon = true; }
			
			// Horizontal
			if (grid[0, 0] == grid[1, 0] && grid[1, 0] == grid[2, 0] && grid[2, 0] != ' ') { hasWon = true; }
			if (grid[0, 1] == grid[1, 1] && grid[1, 1] == grid[2, 1] && grid[2, 1] != ' ') { hasWon = true; }
			if (grid[0, 2] == grid[1, 2] && grid[1, 2] == grid[2, 2] && grid[2, 2] != ' ') { hasWon = true; }
			
			// Diagonal
			if (grid[0, 0] == grid[1, 1] && grid[1, 1] == grid[2, 2] && grid[2, 2] != ' ') { hasWon = true; }
			if (grid[0, 2] == grid[1, 1] && grid[1, 1] == grid[2, 0] && grid[2, 0] != ' ') { hasWon = true; }
			
			return hasWon;
		}
		
		public bool DrawCheck()
		{
			bool hasDrawn = false;
			
			if ((grid[0, 0] != ' ' && grid[0, 1] != ' ' && grid[0, 2] != ' ') &&
				(grid[1, 0] != ' ' && grid[1, 1] != ' ' && grid[1, 2] != ' ') &&
				(grid[2, 0] != ' ' && grid[2, 1] != ' ' && grid[2, 2] != ' '))
				{
					hasDrawn = true;
				}
				
			return hasDrawn;
		}
		
		public void RenderBoard(int offset, int start)
		{
			StringBuilder sb = new StringBuilder();
			StringBuilder offs = new StringBuilder();
			
			for (int i = 0; i < offset; i++)
			{
				sb.Append('\n');
				offs.Append(' ');
			}
			
			//sb.Append(offs.ToString() + "   |   |   " + '\n');
			sb.Append(offs.ToString() + $" {grid[0, 0]} | {grid[0, 1]} | {grid[0, 2]}   Player Wins: " + owins + '\n'); 
			sb.Append(offs.ToString() + "---+---+---  Computer Wins: " + xwins + '\n');
			sb.Append(offs.ToString() + $" {grid[1, 0]} | {grid[1, 1]} | {grid[1, 2]}" + '\n'); 
			if (start == 1)
			{
				sb.Append(offs.ToString() + "---+---+---  Computer starts." + '\n');
			}
			else if (start == 2)
			{
				sb.Append(offs.ToString() + "---+---+---  Player starts." + '\n');
			}
			else
			{
				sb.Append(offs.ToString() + "---+---+---" + '\n');
			}
			sb.Append(offs.ToString() + $" {grid[2, 0]} | {grid[2, 1]} | {grid[2, 2]}" + '\n');
			//sb.Append(offs.ToString() + "   |   |   " + '\n');
			
			Console.Clear();
			Console.WriteLine(sb.ToString());
		}
	}
	
	class Cursor
	{
		public int X { get; set; } = 0;
		public int Y { get; set; } = 0;
		public int Offset { get; private set; }
		
		public Cursor(int offset)
		{
			Offset = offset;
		}
		
		public void Up() { if (Y > 0) { Y -= 1; } }
		public void Down() { if (Y < 2) { Y += 1; } }
		public void Left() { if (X > 0) { X -= 1; } }
		public void Right() { if (X < 2) { X += 1; } }
		
		public void Set() 
		{ 
			int x = (X * 4) + Offset + 1;
			int y = (Y * 2) + Offset;
			Console.SetCursorPosition(x, y); 
		}
	}
	
	static void Main()
	{
		while (true)
		{
			Board board = new Board();
			Cursor cursor = new Cursor(1);
			LoadStats();
			byte gameEndReason = 0;
			int starter = 1;
			string[] messageWin = { "Cool!", "Nice one!", "Brief statement of approval!", "You are good in it!" };
			string[] messageLose = { "Oh no, you failed.", "Better luck next time", "I believe in you!", "Negative statement." };
			string[] messageDraw = { "Wow, what a twist!", "That was anti-climatic", "Aww come on!", "Everyone's a winner today." };
			
			// 0 = opponent wins
			// 1 = player wins
			// 2 = draw
			
			if (random.Next(0, 2) == 0)
			{
				starter = 2;
				board.RenderBoard(1, starter);
				board.PlayerTurn(cursor);
				board.RenderBoard(1, starter);
				starter = 0;
			}
			
			while (true)
			{
				board.OpponentTurn();
				if (board.WinCheck())
				{
					gameEndReason = 0;
					break;
				}
				if (board.DrawCheck())
				{
					gameEndReason = 2;
					break;
				}
				board.RenderBoard(1, starter);
				starter = 0;
				
				board.PlayerTurn(cursor);
				if (board.WinCheck())
				{
					gameEndReason = 1;
					break;
				}
				if (board.DrawCheck())
				{
					gameEndReason = 2;
					break;
				}
				board.RenderBoard(1, starter);
			}
			
			switch (gameEndReason)
			{
				case 0:
					board.RenderBoard(1, starter);
					Console.WriteLine($"\n X wins!\n {messageLose[random.Next(0, 3)]}\n");
					xwins++;
					break;
				case 1:
					board.RenderBoard(1, starter);
					Console.WriteLine($"\n O wins!\n {messageWin[random.Next(0, 3)]}\n");
					owins++;
					break;
				case 2:
					board.RenderBoard(1, starter);
					Console.WriteLine($"\n Draw!\n {messageDraw[random.Next(0, 3)]}\n");
					break;
			}
			
			SaveStats();
			
			Console.Write(" Try again? (Y/N)\n ");
			string response = Console.ReadLine();
			if (response == "delete")
			{
				xwins = 0;
				owins = 0;
				SaveStats();
			}
			else if (response[0] != 'Y' && response[0] != 'y')
			{
				Console.Clear();
				Console.WriteLine("Thanks for playing!");
				break;
			}
		}
	}
}
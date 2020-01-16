//  
// Copyright (c) Lewis (LewisTehMinerz) Crichton. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  
//  

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace bf
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length < 1)
                PrintHelp();

            var file = args[0];

            if (!File.Exists(file))
                Exit($"error: cannot find {file}");

            var src = File.ReadAllText(file).ToCharArray();

            var memory = new Dictionary<int, int>();
            var memoryPointer = 0;

            var instructionPointer = 0;

            var endOfFile = src.Length;

            var stepThrough = false;

            while (instructionPointer < endOfFile)
            {
                try
                {
                    // dynamic memory, initializes the cells that are
                    // being currently pointed to, regardless of following
                    // instructions
                    if (!memory.ContainsKey(memoryPointer))
                        memory[memoryPointer] = 0;

                    var instruction = src[instructionPointer];

                    if (stepThrough)
                    {
                        Console.Write(instruction);
                        Console.ReadKey(true);
                    }

                    switch (instruction)
                    {
                        case '>':
                            memoryPointer++;

                            break;

                        case '<':
                            memoryPointer--;

                            break;

                        case '+':
                            memory[memoryPointer]++;

                            break;

                        case '-':
                            memory[memoryPointer]--;

                            break;

                        case '.':
                            Console.Write((char)memory[memoryPointer]);

                            break;

                        case ',':
                            memory[memoryPointer] = Console.Read();

                            break;

                        case '[':
                            if (memory[memoryPointer] == 0)
                            {
                                var skip = 0;
                                var ptr = instructionPointer + 1;

                                while (src[ptr] != ']' || skip > 0)
                                {
                                    switch (src[ptr])
                                    {
                                        case '[':
                                            skip++;
                                            break;
                                        case ']':
                                            skip--;
                                            break;
                                    }

                                    ptr++;
                                    instructionPointer = ptr;

                                    if (!stepThrough) continue;

                                    // display skipped code in yellow
                                    Console.ForegroundColor = ConsoleColor.Yellow;
                                    Console.Write(src[instructionPointer]);
                                    Console.ResetColor();
                                }
                            }

                            break;

                        case ']':
                            if (memory[memoryPointer] != 0)
                            {
                                var skip = 0;
                                var ptr = instructionPointer - 1;

                                while (src[ptr] != '[' || skip > 0)
                                {
                                    switch (src[ptr])
                                    {
                                        case '[':
                                            skip--;
                                            break;
                                        case ']':
                                            skip++;
                                            break;
                                    }

                                    ptr--;
                                    instructionPointer = ptr;
                                }
                            }

                            break;

                        // debugging tools

                        // breakpoint
                        case '$':
                            Console.WriteLine("\nBREAKPOINT\n");

                            // memory dump
                            Console.WriteLine("\nMemory");
                            Console.WriteLine("──────\n");

                            Console.WriteLine($"Current Memory Pointer: {memoryPointer}");

                            Console.WriteLine("Memory Ptr │ Value");
                            Console.WriteLine("───────────┼──────");
                            foreach (var pointer in memory.Keys.OrderBy(x => x))
                            {
                                Console.WriteLine($"0x{pointer:X8} │ {memory[pointer]}");
                            }

                            Console.WriteLine($"\nTotal Cells: {memory.Keys.Count}");

                            // instruction info
                            Console.WriteLine("\nLast Instruction");
                            Console.WriteLine("────────────────");

                            var previousPointer = instructionPointer - 1;
                            var previousInstruction = src[previousPointer];

                            Console.WriteLine($"Instruction: {previousInstruction} @ {previousPointer}");

                            // extra debugging information for instruction
                            switch (previousInstruction)
                            {
                                case '>':
                                    Console.WriteLine($"Old Pointer: {memoryPointer - 1}");
                                    Console.WriteLine($"New Pointer: {memoryPointer}");
                                    break;

                                case '<':
                                    Console.WriteLine($"Old Pointer: {memoryPointer + 1}");
                                    Console.WriteLine($"New Pointer: {memoryPointer}");
                                    break;

                                case '+':
                                    Console.WriteLine($"Old Value: {memory[memoryPointer] - 1}");
                                    Console.WriteLine($"New Value: {memory[memoryPointer]}");
                                    break;

                                case '-':
                                    Console.WriteLine($"Old Value: {memory[memoryPointer] + 1}");
                                    Console.WriteLine($"New Value: {memory[memoryPointer]}");
                                    break;

                                case '.':
                                case ',':
                                    Console.WriteLine($"Memory Cell as Char: {(char)memory[memoryPointer]}");
                                    break;

                                case '[':
                                    Console.WriteLine($"Skipping: {memory[memoryPointer] == 0}");
                                    break;

                                case ']':
                                    Console.WriteLine($"Jumping Back: {memory[memoryPointer] != 0}");
                                    break;

                                case '$':
                                    Console.WriteLine("Confused: True\n");
                                    Console.WriteLine("You have put two breakpoints next to each other.");
                                    Console.WriteLine("The state of the program will never change when breakpoints");
                                    Console.WriteLine("are put next to each other. It is redundant.");
                                    break;
                            }

                            Console.WriteLine("\nPress any key to continue execution.");
                            Console.ReadKey(true);

                            break;

                        // step through toggle
                        case '#':
                            stepThrough = !stepThrough;

                            break;
                    }
                }
                catch (Exception e)
                {
                    Exit($@"error: interpreter exception: {e.Message}
   at: instruction {instructionPointer}
  ptr: {memoryPointer}
 char: {src[instructionPointer]}");
                }

                instructionPointer++;
            }
        }

        private static void PrintHelp()
        {
            var asmName = Assembly.GetExecutingAssembly().GetName().Name;
            Exit($@"usage: {asmName} <file>
example: {asmName} helloworld.b");
        }

        private static void Exit(string message, int code = -1)
        {
            Console.WriteLine(message);
            Environment.Exit(code);
        }
    }
}

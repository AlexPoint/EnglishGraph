﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnglishGraph.Models;

namespace Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            var context = new EnglishGraphContext();
            
            /*context.Words.Add(new Word() {Name = "test"});
            context.SaveChanges();*/
            Console.WriteLine(context.Words.Count());


            Console.WriteLine("OK");
            Console.ReadLine();
        }
    }
}

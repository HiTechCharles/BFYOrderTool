using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;


namespace BFY_OrderTool
{
    class Program
    {
        #region Public variables
        /*GLOBAL VARIABLES AND ARRAYS
         *  Name - names of products
         *  CatPath - directory where category CSV files are stored
         *  ShopDate - Order date, next closest monday, long date 
         *  QTYSelected - qty for associated item
         *  NumItems - Number of items in current file
         *  CategoryName - Name of the current category of items 
         *  ListPath - File name for order listing, text file  */
        
        public static string[] Name = new string[99];  //product names
        public static string CatPath = Environment.GetEnvironmentVariable("onedriveconsumer") + "\\documents\\brew for you\\NextOrder\\";
        public static string ShopDate;
        public static Double[] QtySelected = new Double[99];  //qty selected to buy
        public static int NumItems;
        public static string CategoryName;
        public static string ListPath = @CatPath + "NextOrder.txt";
        #endregion

        static void Main(string[] args)  //entrypoint of program
        {

            Console.Title = "BFY Order Creation tool";  // console title
            Console.ForegroundColor = ConsoleColor.White;  //text color for console
            Console.WriteLine("This is the Brew for You Order creation Tool!  This program");
            Console.WriteLine("will help you create your weekly sandwich order.  All you need");
            Console.WriteLine("to do is select how many of each item you need.");
            Console.WriteLine("\nBe sure to complete your shopping list first.");

            if ( File.Exists(ListPath) == true )  //if report file exists, 
                {
                    File.Delete(ListPath);  //delete it
                }

            DateTime today = DateTime.Today;  //store current date
            DateTime closestMonday = GetClosestMonday(today);  //find out when the next monday is
            ShopDate = closestMonday.ToLongDateString();  //long date for order report file
            StreamWriter bn = File.AppendText(ListPath);  //start writing report file with business name and date
            bn.WriteLine("Items needed at brew for you on");
            bn.WriteLine(ShopDate);
            bn.Close();  //close files when writing is done

            foreach (string file in Directory.EnumerateFiles
              (@CatPath, "*.csv")) //for each csv file in the CATPATH directory
            {
                LoadFile(file);  //load items from file to array name;
                GetValues();  //see how many of each item is desired
            }
            
            //shopping list file from another of my apps.  it is added to the end of the report file
            string ShopListPath = @Environment.GetEnvironmentVariable("onedriveconsumer") + "\\documents\\brew for you\\shoplist\\Shopping List.txt";
            string ShopContents = File.ReadAllText(ShopListPath);  //contents of file
            File.AppendAllText(ListPath, ShopContents);  //add lines from shoplist file to end of report
            Console.WriteLine("\n\nYour order selections are all set.  viewing the");
            Console.WriteLine("order text file.  Press a key to continue...");
            Console.ReadKey();  //read a key for pause before viewing text file 
            System.Diagnostics.Process.Start(ListPath);  //open report text file for viewing 
        }

        static DateTime GetClosestMonday(DateTime date)  //get the next date of closest monday 
        {
            int daysUntilMonday = ((int)DayOfWeek.Monday - (int)date.DayOfWeek + 7) % 7;
            int daysSinceMonday = ((int)date.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;

            return date.AddDays(daysUntilMonday);
        }

        static void LoadFile(string FileName)  //read a csv file
        {
            //clear all storage arrays before each file loads
            Array.Clear(Name, 0, 99);
            Array.Clear(QtySelected, 0, 99);
            NumItems = 0; string line;  //stores current line read from file and number of lines in it 
            StreamReader CsvRip = new StreamReader (FileName);

            CategoryName = CsvRip.ReadLine();  //first line is category name 
            while (!CsvRip.EndOfStream)  //loop until EOF
            {
                line = CsvRip.ReadLine();  //read a line from file
                Name[NumItems] = line;  //store in array 
                NumItems++;  //count number of lines
            }
        }

        static void GetValues()  //gets how many of each product to buy
        {
            string input;  //take input; 

            Console.WriteLine("\nThe following items are in the " + CategoryName + " category:");   
            for (int I = 0; I < NumItems; I++)  //for each item
            {
                do
                {
                    Console.Write("    " + Name[I] + " - ");  //print product name on console
                    input = Console.ReadLine();  //get a number
                                 
                    #region word matching 

                    if ((input == "prev") && (I > 1))  //previous item, make sure you can't go past beginning of list 
                    {
                        Console.WriteLine("Going to previous item");
                        I--;  //decrement index 
                    }

                    if ( input == "skip" )  //exit current category
                        {
                            Console.WriteLine("Skipping remaining items in the " + CategoryName + " category");
                            SaveToFile();  //save selected items to report
                            return;  //exit function
                    }
                    #endregion 
                } while (Double.TryParse(input, out QtySelected[I]) == false);
            }

            SaveToFile();  //save all selections to disk
    }

        static void SaveToFile () //save current selections to report
        {
            StreamWriter bn = File.AppendText(ListPath);  //append to report file in progress
            bn.WriteLine("\n" + CategoryName + ":");  //write category name 
            for (int I = 0; I < NumItems; I++)  //for each item 
            {
                if (QtySelected[I] > 0)  //if qty not zero 
                {
                    bn.WriteLine("    " + QtySelected[I] + " " + Name[I]);  
                }
            }
            bn.Close();  //close file
        }

    } //end class
}  //end namespace  i always seem to mess up the braces so i do this 
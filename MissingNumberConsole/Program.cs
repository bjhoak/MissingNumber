using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissingNumberConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            FindMissingNumbers();                             // Call the root function

        }

        // FindMissingNumbers 
        // Purpose: Root function for finding missing numbers in an input file
        // Inputs: none
        // Outputs: None
        public static void FindMissingNumbers ()
        {
            string FilePath = "";
            Console.WriteLine("Beginning \"Missing Number\" operation");
            if (CollectFilePath(ref FilePath))            //Open the input file
            {
                if(ParseFile(FilePath))                  //Parse the file
                {
                    Console.WriteLine("\"Missing Number\" operation completed successfully.");
                }
            }
           
            Console.ReadKey();
        }

        // CollectFilePath
        // Purpose: Prompt the user for the input file and check that it exists
        // Input: File path (from console)
        //        string to place the new file path by reference
        // Output: boolean representing if the file was successfully opened
        private static Boolean CollectFilePath(ref string FilePath)
        {
            Boolean FilePresent = false;
            string Response;

            Console.Write("Please enter the path to your input file:");
            FilePath = Console.ReadLine();
            
            // Verify that the entered path exists
            //  File is assumed to be a flat file, if this cannot be assumed, a check will need to be added here as well
            if(System.IO.File.Exists(FilePath) )
            {
                return true;
            }
            else 
            {   // If the path is not valid, prompt the user to retry
                Console.Write("The entered file path does not exist. Do you wish to re-enter the path? :");
                Response = Console.ReadLine();              //collect the first character of the response
                //check the first character of the response and force it to lower case
                if (Response.Substring(0, 1).ToLower() == "y")
                { 
                    FilePresent = CollectFilePath(ref FilePath); //Call the function recursively in order to retry
                }
                else                                       //If the user enters anything other than y
                {                                          //This case needs to be in here for the recursion
                    return false;
                }
            }         
            return FilePresent;                            //return a boolean variable in order to make the function recursive
        }

        // ParseFile
        // Purpose: Open the file and parse it one line as a time
        // Input: string containing the path to the file
        // Output: boolean representing if the file was parsed successfully 
        private static Boolean ParseFile(string FilePath)
        {
            string FileLine = "";
            Boolean Status = true;                          //Used to abort the parsing operation if invalid characters are encountered

            //Attempt to open the file.
            System.IO.StreamReader IOFile = new System.IO.StreamReader(FilePath); 

            while (((FileLine = IOFile.ReadLine()) != null ) && Status)
            {
                Console.WriteLine(ParseLine(FileLine,ref Status));      //Output the first number that is missing from the list
            }
            IOFile.Close();                                  //close the file before exiting
            return Status;
        }

        // ParseLine
        // Purpose: Parse an individual line and return the missing number
        // Input: An individual line read from the input file
        //        A boolean that indicates if operation should continue after this line is finished
        // Output: a string containing the missing line
        private static string ParseLine(string line,ref Boolean Status)
        {
            NumberList IONumList = new NumberList();

            // use a split function to parse the list into an array
            string[] items = line.Split(',');
            // step through the array and add each item
            foreach (string item in items)
            {
                //Attempt to add the item to the number list
                try { IONumList.Add(System.Convert.ToInt32(item)); }
                catch    //A catch statement is needed for when invalid characters are encountered
                {        //  For now a generic catch is used to just exit the loop since a "flat file" was supposed to be entered
                    Console.WriteLine("Invalid characters detected in the input file.  Operation aborted.");
                    Status = false;                            //Status is used to stop parsing lines
                    break;
                }     
                
            }
            return IONumList.FindFirstMissingItem();
        }  
    }

    //Number List class is used to store, and sort a list of numbers
    //  The add method of this class adds new items in numerical order 
    class NumberList
    {
        //Nodes are used to store additional numbers
        public class Node
        {                                              //simple linked list object, contains a value and a pointer to the next item
            public int Value;
            public Node Next;
        }
 
        private int size;                              // Used to maintain the size of the list
        private Node head;                             // Starting point of the list
        private Node tail;                             // The current node, used to avoid adding nodes before the head
 
        //Intialize the list
        public NumberList()
        {
            //default the list to empty
            size = 0;
            head = null;
            tail = null;
        }
 
        // Add a new Node to the list.
        public void Add(int NewValue)
        {
            size++;                                    //Increment the size of the list by 1
 
            Node NewItem = new Node();                 //create a new Node item to hold the new value
            NewItem.Value = NewValue;

            
            if (head == null)                         //If the list is currently empty
            {
                head = NewItem;                       //Make the new item the head
                tail = NewItem;                       //both pointers point at the first item initially
            }
            else                                      //If the list is not empty, step through to find the new spot
            {
                // This is not the head. Make it current's next node.
                //current.Next = NewItem;             
                Node prevNode = head;
                Node tempNode = head.Next;

                if (NewValue <= prevNode.Value )
                {  
                    //special case where the new item is actually the smallest
                    //shift the items and add the newitem as the first entry
                    head = NewItem;
                    head.Next = prevNode;
                }
                else if (tail.Value <= NewValue)                   //This case has been added to speed up parsing the list for the worst case scenerio (aka list is already in order)
                { 
                    //If the newitem belongs at the end of the list
                    tail.Next = NewItem;          //point the last item at the newitem
                    tail = NewItem;               //shift the end of list pointer
                    tail.Next = null;             //default the pointer to null 
                }
                else
                {
                    while (tempNode != null)
                    {
                        if (NewValue <= tempNode.Value)
                        {   //Insert the item into the middle of the list
                            prevNode.Next = NewItem;
                            NewItem.Next = tempNode;
                            tempNode = tail;      //point the tempnode to the end of the list so that the while loop exits
                        }
                        //Shift Both pointers
                        prevNode = prevNode.Next;
                        tempNode = tempNode.Next; 
                    }
                }
            }
        }
 
        //PrintList 
        //Purpose: Outputs the list for debugging purposes
        public void PrintList()
        {
            Node tempNode = head;
            while (tempNode != null)
            {
                Console.Write(tempNode.Value + ",");
                tempNode = tempNode.Next;                         //Shift temp pointer to the next item
            }
            Console.WriteLine();
        }


        // Purpose: Delete a specific node from the list
        // Input: Position of node to be deleted
        // Output: Boolean representing if the operation was successful
        public bool Delete(int Position)
        {
            //Do not attempt to delete an invalid position
            if ((Position < 1) || (Position > size))
            {
                return false;
            }
            if (Position == 1)                                            //If the first item is being deleted, shift the head pointer
            {               
                head = head.Next;
                size--;                                                   // decrement the size of the list
                return true;
            }
            Node TempNode = Item(Position - 1);                           //Find the item before the target position
            TempNode.Next = TempNode.Next.Next;                           //Cut the target item out of the list
            size--;                                                       //decrement the size
            return true;
        }

        // Item
        // Purpose: Return the Node at a particular location
        // Inputs: Integer containing the target location
        // Outputs: A node pointer to the item at the target location
        public Node Item(int Position)
        {
            // If the position is not valid, return null 
            if ((Position < 1) || (Position > size))
            {
                return null;
            }
            // Start looking at the first item
            int count = 1;
            Node NextItem = head;
            while (count < Position )                                           //step through the list
            {
                NextItem = NextItem.Next;                                      //move to the next item in the list
                count++;                                                       //increment the counter
            }
            return NextItem;                                                   //return the found item
        }

        // FindFirstMissingItem
        // Purpose: Step through the list until a number is missing, then return the number as a string
        // Inputs: None
        // Outputs: A string containing the number that is missing from the list
        public string FindFirstMissingItem ()
        {
            if(head == null)                                                   // if the list is empty
            {
                return "";                                                     //return an empty string
            }
            else
            {
                int value = head.Value;                                        //start value
                Node TempNode = head;
                while((TempNode != null) && (value == TempNode.Value ))        //Step through the list looking for the first missing item
                {
                    TempNode = TempNode.Next;                                  //shift to the next item in the list
                    value++;                                                   //increment the value in order to compare it to the next item
                    if((TempNode!=null) && (TempNode.Value < value))
                    { //This case should only be possible if the list contains duplicate values
                        value--;                                               //move the value back to account for the duplicated value
                    }
                }
                //If no number is actually missing, the returned value will be the next number possible in the sequence
                return value.ToString();                                       //return a string representation of the value
            }
        }
    }
}

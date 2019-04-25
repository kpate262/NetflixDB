//
// Project #06
// 
// SQL, C# and ADO.NET program to retrieve Netflix movie data.
//
// <<YOUR NAME>>
// 

using System;
using System.Data.SqlClient;
using System.Data;

namespace workspace
{
  class Program
  {
    
    //
    // Main:
    //
    static void Main(string[] args)
    {
      string connectionInfo = String.Format(@"
Server=tcp:jhummel2.database.windows.net,1433;Initial Catalog=Netflix;
Persist Security Info=False;User ID=student;Password=cs341!uic;
MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;
Connection Timeout=30;
");
      
      
      System.Console.WriteLine();
      System.Console.WriteLine("TODO!");
      System.Console.WriteLine();
      
      
    }//Main
    
  }//class
}//namespace

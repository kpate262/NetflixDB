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
using System.Collections.Generic;

namespace workspace
{
  public class Movie{
      public string movieName;
      public int year;
      private string avgRating;
      private int[] reviews;
      private int totalReviews;
      private int rank;
      private int totalMovies;
      
      /*Movie(){
          reviews = new int[5];
          for(int i = 0; i < reviews.Length; i++){
              reviews[i] = 0;
          }
      }*/
      
      public Movie(string movieName, string avgRating, int totalReviews, SqlConnection db){
          this.reviews = new int[5];
          this.movieName = movieName;
          this.totalReviews = totalReviews;
          this.avgRating = avgRating;
          
          movieName = movieName.Replace("'", "''");
          string sql = string.Format(@"SELECT Count(*) AS Rating
                                    FROM Reviews
                                    INNER JOIN Movies
                                        ON Movies.MovieID = Reviews.MovieID
                                    WHERE MovieName = '{0}' AND Rating = 5

                                    SELECT Count(*) AS Rating
                                    FROM Reviews
                                    INNER JOIN Movies
                                        ON Movies.MovieID = Reviews.MovieID
                                    WHERE MovieName = '{1}' AND Rating = 4

                                    SELECT Count(*) AS Rating
                                    FROM Reviews
                                    INNER JOIN Movies
                                        ON Movies.MovieID = Reviews.MovieID
                                    WHERE MovieName = '{2}' AND Rating = 3

                                    SELECT Count(*) AS Rating
                                    FROM Reviews
                                    INNER JOIN Movies
                                        ON Movies.MovieID = Reviews.MovieID
                                    WHERE MovieName = '{3}' AND Rating = 2

                                    SELECT Count(*) AS Rating
                                    FROM Reviews
                                    INNER JOIN Movies
                                        ON Movies.MovieID = Reviews.MovieID
                                    WHERE MovieName = '{4}' AND Rating = 1
                                    
                                    SELECT MovieYear
                                    FROM Movies
                                    FULL OUTER JOIN Reviews 
                                        ON Movies.MovieID = Reviews.MovieID
                                    WHERE MovieName = '{5}'", 
                                     movieName, movieName, movieName, movieName, movieName, movieName);
          
          DataSet ds = new DataSet();
          SqlCommand cmd = new SqlCommand();
          cmd.Connection = db;
          SqlDataAdapter adapter = new SqlDataAdapter(cmd);
          cmd.CommandText = sql;
          adapter.Fill(ds);
          
          var rows = ds.Tables[0].Rows;

          /*var rows1 = ds.Tables[1].Rows;
          var rows2 = ds.Tables[2].Rows;
          var rows3 = ds.Tables[3].Rows;
          var rows4 = ds.Tables[4].Rows;*/
          
          for (int i = 0; i < 6; i++){
              rows = ds.Tables[i].Rows;
              foreach(DataRow row in rows){
                  if(i == 5){
                      this.year = System.Convert.ToInt32(row["MovieYear"]);
                  }
                  else{
                      reviews[4-i] = System.Convert.ToInt32(row["Rating"]);
                  }
              }
          }
          
          
          ds.Clear();
          
          sql = string.Format(@"SELECT MovieName, AVG(CONVERT(float, Rating)) AS AvgRating
                            FROM Movies
                            FULL OUTER JOIN Reviews
                                ON Movies.MovieID = Reviews.MovieID
                            GROUP BY MovieName 
                            ORDER BY AvgRating DESC, MovieName ASC");
          
          cmd = new SqlCommand();
          cmd.Connection = db;
          adapter = new SqlDataAdapter(cmd);
          cmd.CommandText = sql;
          adapter.Fill(ds);
          
          var ranks = ds.Tables[0].Rows;
          this.totalMovies = ranks.Count;
          int counter = 0;
          
          foreach(DataRow row in ranks){
              counter += 1;
              if(string.Compare(this.movieName, System.Convert.ToString(row["MovieName"]))==0){
                  this.rank = counter;
                  break;
              }
          }
          
      }
      
      
      public string getAvgRating(){
          return avgRating;
      }
      
      public int[] getReviews(){
          return reviews;
      }
      
      public int getRank(){
          return rank;
      }
      
      public string outPutReviews(){
          return string.Format(@"[5,4,3,2,1: {0},{0},{0},{0},{0}]", 
                               reviews[4], reviews[3], reviews[2], reviews[1], reviews[0]);
      }
      
      public void print(){
          System.Console.WriteLine("'{0}', released {1}", movieName, year);
          if(totalReviews == 0){
              System.Console.WriteLine(" Avg rating: <<no reviews>>"); 
                                   
          }
          else{
              System.Console.WriteLine(" Avg rating: {0} across {1} reviews [5,4,3,2,1: {2},{3},{4},{5},{6}]", 
                                   avgRating, totalReviews, reviews[4], reviews[3], reviews[2], reviews[1], reviews[0]);
          }
          System.Console.WriteLine(" Ranked {0} out of {1}", rank, totalMovies);
      }
      
  }
    
  
  public class Layer{
      public List<Movie> matchedMovies;
      
      
      public List<Movie> GetMovies(string movieName){
          List<Movie> movieList = new List<Movie>();
          string connectionInfo = String.Format(@"
            Server=tcp:jhummel2.database.windows.net,1433;Initial Catalog=Netflix;
            Persist Security Info=False;User ID=student;Password=cs341!uic;
            MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;
            Connection Timeout=30;
            ");
          
          SqlConnection db = null;
          
          try{
              db = new SqlConnection(connectionInfo);
              db.Open();
          }
          catch(Exception ex){
              System.Console.WriteLine("**Error: {0}", ex.Message);
          }
          finally{
              movieName = movieName.Replace("'", "''");
              string sql = string.Format(@"SELECT MovieName, AVG(Convert(float, Rating)) AS AvgRating, COUNT(Rating) AS TotalRating
                                           FROM Movies
                                           FULL OUTER JOIN Reviews
                                               ON Movies.MovieID = Reviews.MovieID
                                           WHERE MovieName LIKE '%{0}%'
                                           GROUP BY MovieName
                                           ORDER BY MovieName ASC", movieName);
              
              DataSet ds = new DataSet();
              SqlCommand cmd = new SqlCommand();
              cmd.Connection = db;
              SqlDataAdapter adapter = new SqlDataAdapter(cmd);
              cmd.CommandText = sql;
              adapter.Fill(ds);
              
              var rows = ds.Tables[0].Rows;
              //System.Console.WriteLine("Rows {0}: ", rows.Count);
                  
              
              if(rows.Count == 0){
                  System.Console.WriteLine("**none found");
              }
              else{
                  foreach(DataRow row in rows){
                      string movieNames = System.Convert.ToString(row["MovieName"]);
                      string avgrate = System.Convert.ToString(row["AvgRating"]);
                      int totalRating = System.Convert.ToInt32(row["TotalRating"]);
                      //System.Console.WriteLine("{0}", avgrate);
                      Movie movie = new Movie(movieNames, avgrate, totalRating, db);
                      movieList.Add(movie);
                  }
              }
                  
              if(db != null && db.State != ConnectionState.Closed)
                  db.Close();
          }
          
          return movieList;
      }
  }
  
  class Program
  {    
    //
    // Main:
    //
    static void Main(string[] args)
    {
      string input;
        
      System.Console.Write("movie> ");
      input = System.Console.ReadLine();
      
      Layer ORMLayer = new Layer();
        

      while(input != ""){
          List<Movie> movieList = ORMLayer.GetMovies(input);
          foreach(Movie m in movieList){
              m.print();
          }
          System.Console.Write("movie> ");
          input = System.Console.ReadLine();
      }
      
      
    }//Main
    
  }//class
}//namespace

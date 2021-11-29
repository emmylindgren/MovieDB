using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace MovieDB.Models
{
    public class MovieMethods
    {
        public MovieMethods() { }

        public int InsertMovie(MovieDetail md, out string errormsg)
        {
            //Create SqlConnection object
            SqlConnection dbConnection = new SqlConnection();

            //Connection towards SQL Server
            dbConnection.ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Movies;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

            //Sqlstring and adding a movie to the db
            String sqlstring = "INSERT INTO Tbl_Movie (Mo_Title, Mo_ReleaseYear, Mo_OnLanguage, Mo_Grade, Mo_Genre) " +
                "VALUES (@title,@releaseYear,@onLanguage,@grade,@genre)";
            SqlCommand dbCommand = new SqlCommand(sqlstring, dbConnection);

            dbCommand.Parameters.Add("title", SqlDbType.NVarChar, 50).Value = md.Title;
            dbCommand.Parameters.Add("releaseYear", SqlDbType.Int).Value = md.ReleaseYear;
            dbCommand.Parameters.Add("onLanguage", SqlDbType.Int).Value = Convert.ToInt32(md.OnLanguage);
            dbCommand.Parameters.Add("grade", SqlDbType.Int).Value = md.Grade;
            dbCommand.Parameters.Add("genre", SqlDbType.Int).Value = Convert.ToInt32(md.Genre);

            List<String> actors = md.Actors;


            try
            {
                dbConnection.Open();
                int i = 0;
                //Adding the movie to the db. Returns integer representing how many rows has been 
                //updated in the db. If 1, success in adding one movie to the db. 
                i = dbCommand.ExecuteNonQuery();
                if(i == 1) {
                    errormsg = "";

                }
                else
                {
                    errormsg = "A movie was not created in the database";
                }
                return (i);
            }
            catch(Exception e)
            {
                errormsg = e.Message;
                return 0;
            }
            finally
            {
                dbConnection.Close();
                if (actors != null)
                {
                    int movie_id = getMovieID(md.Title, out errormsg);
                    addMovieConnections(movie_id, actors, out errormsg);
                }
            }

        }
        
        public int UpdateMovie(MovieDetail md, out string errormsg)
        {
            //Create SqlConnection object
            SqlConnection dbConnection = new SqlConnection();

            //Connection towards SQL Server
            dbConnection.ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Movies;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

            //Sqlstring and updating the movie on the db
            String sqlstring = "UPDATE Tbl_Movie SET Mo_Title = @title," +
                " Mo_ReleaseYear = @releaseYear, Mo_OnLanguage = @onLanguage,Mo_Grade = @grade, " +
                "Mo_Genre = @genre " +
                "WHERE Mo_Id = @id";
            SqlCommand dbCommand = new SqlCommand(sqlstring, dbConnection);

            dbCommand.Parameters.Add("title", SqlDbType.NVarChar, 50).Value = md.Title;
            dbCommand.Parameters.Add("releaseYear", SqlDbType.Int).Value = md.ReleaseYear;
            dbCommand.Parameters.Add("onLanguage", SqlDbType.Int).Value = Convert.ToInt32(md.OnLanguage);
            dbCommand.Parameters.Add("grade", SqlDbType.Int).Value = md.Grade;
            dbCommand.Parameters.Add("genre", SqlDbType.Int).Value = Convert.ToInt32(md.Genre);
            dbCommand.Parameters.Add("id", SqlDbType.Int).Value = md.Id;

            List<String> actors = md.Actors;


            try
            {
                dbConnection.Open();
                int i = 0;
                //Updating the movie in the db. Returns integer representing how many rows has been 
                //updated in the db. If 1, success in adding one movie to the db. 
                i = dbCommand.ExecuteNonQuery();
                if (i == 1)
                {
                    errormsg = "";

                }
                else
                {
                    errormsg = "The Movie was not updated in the database";
                }
                return (i);
            }
            catch (Exception e)
            {
                errormsg = e.Message;
                return 0;
            }
            finally
            {
                dbConnection.Close();
                if (actors != null)
                {
                    
                    int movie_id = getMovieID(md.Title, out errormsg);
                    clearMovieConnections(movie_id, out errormsg);
                    addMovieConnections(movie_id, actors, out errormsg);
                }
            }

        }

        public int getMovieID(String movieTitle, out string errormsg)
        {
            SqlConnection dbConnection = new SqlConnection();

            dbConnection.ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Movies;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

            String sqlstring = "SELECT Mo_Id FROM Tbl_Movie WHERE Mo_Title= @title";
            SqlCommand dbCommand = new SqlCommand(sqlstring, dbConnection);
            dbCommand.Parameters.Add("title", SqlDbType.NVarChar, 50).Value = movieTitle;

            //Create adapter 
            SqlDataAdapter movieAdapter = new SqlDataAdapter(dbCommand);
            DataSet movieDS = new DataSet();

            try
            {
                dbConnection.Open();

                //Fill the dataset with data in a table named Movie.
                movieAdapter.Fill(movieDS, "Movie");

                int count = 0;

                count = movieDS.Tables["Movie"].Rows.Count;

                if(count > 1)
                {
                    errormsg = "There is two or more movies with that title";
                    return 0;
                }
                else if (count == 1)
                {
                    int i = Convert.ToInt16(movieDS.Tables["Movie"].Rows[0]["Mo_Id"]);
                    errormsg = "";
                    return i;
                }
                else
                {
                    errormsg = "No movies was fetched";
                    return 0;
                }
            }
            catch (Exception e)
            {
                errormsg = e.Message;
                return 0;
            }
            finally
            {
                dbConnection.Close();
            }

        }

        private int addMovieConnections(int movie_id, List<String> actors, out string errormsg)
        {

            //Create SqlConnection object
            SqlConnection dbConnection = new SqlConnection();

            //Connection towards SQL Server
            dbConnection.ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Movies;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            
            int count = actors.Count;
            int i = 0;

            try
            {
                dbConnection.Open();
                int update = 0; 

                while (count > 0)
                {
                    //Sqlstring adding the connections betweeen movie and actor. 
                    String sqlstringConnection = "INSERT INTO Tbl_MovieActor (MA_Actor, MA_Movie) " +
                            "VALUES (@actor,@movie)";
                    SqlCommand dbCommandConnection = new SqlCommand(sqlstringConnection, dbConnection);
                    dbCommandConnection.Parameters.Add("movie", SqlDbType.Int).Value = movie_id;
                    dbCommandConnection.Parameters.Add("actor", SqlDbType.Int).Value = Convert.ToInt32(actors[i]);
                    update = dbCommandConnection.ExecuteNonQuery();
                    i++;
                }
                errormsg = "";
                return (i);
            }
            catch (Exception e)
            {
                errormsg = e.Message;
                return 0;
            }
            finally
            {
                dbConnection.Close();
            }
        }

        private int clearMovieConnections(int movie_id,out string errormsg)
        {

            //Create SqlConnection object
            SqlConnection dbConnection = new SqlConnection();

            //Connection towards SQL Server
            dbConnection.ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Movies;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

            String sqlstringConnection = "DELETE FROM Tbl_MovieActor WHERE MA_Movie = @movieID";
            SqlCommand dbCommandConnection = new SqlCommand(sqlstringConnection, dbConnection);
            dbCommandConnection.Parameters.Add("movieID", SqlDbType.Int).Value = movie_id;

            try
            {
                dbConnection.Open();
                int i = 0;
                i = dbCommandConnection.ExecuteNonQuery();

                errormsg = "";
                return (i);
            }
            catch (Exception e)
            {
                errormsg = e.Message;
                return 0;
            }
            finally
            {
                dbConnection.Close();
            }
        }



        public int DeleteMovie(int movie_id, out string errormsg)
        {

            SqlConnection dbConnection = new SqlConnection();

            dbConnection.ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Movies;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

            String sqlstring = "DELETE FROM Tbl_Movie WHERE Mo_Id =@id";
            SqlCommand dbCommand = new SqlCommand(sqlstring, dbConnection);

            dbCommand.Parameters.Add("id", SqlDbType.Int).Value = movie_id;

            try
            {
                dbConnection.Open();
                int i = 0;
                //Removing the movie from the db. Returns integer representing how many rows has been 
                //updated in the db. If 1, success in removing one movie from the db. 
                i = dbCommand.ExecuteNonQuery();
                if (i == 1)
                {
                    errormsg = "";
                }
                else
                {
                    errormsg = "The movie was not deleted from the database";
                }
                return (i);
            }
            catch (Exception e)
            {
                errormsg = e.Message;
                return 0;
            }
            finally
            {
                dbConnection.Close();
            }

        }

        public List<MovieDetail> GetAllMovies(out string errormsg)
        {

            SqlConnection dbConnection = new SqlConnection();

            dbConnection.ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Movies;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

            String sqlstring = "SELECT Mo_Id, Mo_Title, Mo_ReleaseYear, La_Language AS Mo_OnLanguage, Mo_Grade, Ge_Title AS Mo_Genre FROM" +
                "((Tbl_Movie INNER JOIN Tbl_Languages ON Mo_OnLanguage = La_Id) " +
                "INNER JOIN Tbl_Genre ON Mo_Genre = Ge_Id)";
            SqlCommand dbCommand = new SqlCommand(sqlstring, dbConnection);

            //Create adapter 
            SqlDataAdapter movieAdapter = new SqlDataAdapter(dbCommand);
            DataSet movieDS = new DataSet();

            List<MovieDetail> MovieList = new List<MovieDetail>();

            try
            {
                dbConnection.Open();

                //Fill the dataset with data in a table named Movie.
                movieAdapter.Fill(movieDS, "Movie");

                int count = 0; 
                int i = 0;

                count = movieDS.Tables["Movie"].Rows.Count;

                if(count > 0)
                {
                    while(i < count)
                    {
                        //Read the data from the dataset. 
                        MovieDetail md = new MovieDetail();

                        md.Id = Convert.ToInt16(movieDS.Tables["Movie"].Rows[i]["Mo_Id"]);

                        md.Title = movieDS.Tables["Movie"].Rows[i]["Mo_Title"].ToString();
                        md.ReleaseYear = Convert.ToInt16(movieDS.Tables["Movie"].Rows[i]["Mo_ReleaseYear"]);
                        md.OnLanguage = (movieDS.Tables["Movie"].Rows[i]["Mo_OnLanguage"]).ToString();
                        md.Grade = Convert.ToInt16(movieDS.Tables["Movie"].Rows[i]["Mo_Grade"]);
                        md.Genre = (movieDS.Tables["Movie"].Rows[i]["Mo_Genre"]).ToString();

                        i++;
                        MovieList.Add(md);
                    }
                    errormsg = "";
                    return MovieList;
                }
                else
                {
                    errormsg = "No movies was fetched";
                    return (null);
                }
            }
            catch (Exception e)
            {
                errormsg = e.Message;
                return (null);
            }
            finally
            {
                dbConnection.Close();
            }

        }

        public List<SelectListItem> GetAllMoviesTitleandID(out string errormsg)
        {

            SqlConnection dbConnection = new SqlConnection();

            dbConnection.ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Movies;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

            String sqlstring = "SELECT Mo_Id, Mo_Title FROM Tbl_Movie";
            SqlCommand dbCommand = new SqlCommand(sqlstring, dbConnection);

            //Create adapter 
            SqlDataAdapter movieAdapter = new SqlDataAdapter(dbCommand);
            DataSet movieDS = new DataSet();

            List<SelectListItem> MovieList = new List<SelectListItem>();

            try
            {
                dbConnection.Open();

                //Fill the dataset with data in a table named Movie.
                movieAdapter.Fill(movieDS, "Movie");

                int count = 0;
                int i = 0;

                count = movieDS.Tables["Movie"].Rows.Count;

                if (count > 0)
                {
                    while (i < count)
                    {
                        //Read the data from the dataset.
                        MovieList.Add(new SelectListItem
                        {
                            Text = movieDS.Tables["Movie"].Rows[i]["Mo_Title"].ToString(),
                            Value = movieDS.Tables["Movie"].Rows[i]["Mo_Id"].ToString()
                        });
                        i++;
                    }
                    errormsg = "";
                    return MovieList;
                }
                else
                {
                    errormsg = "No movies was fetched";
                    return (null);
                }
            }
            catch (Exception e)
            {
                errormsg = e.Message;
                return (null);
            }
            finally
            {
                dbConnection.Close();
            }

        }

        public List<MovieDetail> GetAllMoviesChosenGenre(out string errormsg, int genre)
        {

            SqlConnection dbConnection = new SqlConnection();

            dbConnection.ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Movies;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

            String sqlstring = "SELECT Mo_Id, Mo_Title, Mo_ReleaseYear, La_Language AS Mo_OnLanguage, Mo_Grade, Ge_Title AS Mo_Genre FROM" +
                "((Tbl_Movie INNER JOIN Tbl_Languages ON Mo_OnLanguage = La_Id) " +
                "INNER JOIN Tbl_Genre ON Mo_Genre = Ge_Id) WHERE Ge_Id = @genreID";
            SqlCommand dbCommand = new SqlCommand(sqlstring, dbConnection);

            dbCommand.Parameters.Add("genreID", SqlDbType.Int).Value = genre;

            //Create adapter 
            SqlDataAdapter movieAdapter = new SqlDataAdapter(dbCommand);
            DataSet movieDS = new DataSet();

            List<MovieDetail> MovieList = new List<MovieDetail>();

            try
            {
                dbConnection.Open();

                //Fill the dataset with data in a table named Movie.
                movieAdapter.Fill(movieDS, "Movie");

                int count = 0;
                int i = 0;

                count = movieDS.Tables["Movie"].Rows.Count;

                if (count > 0)
                {
                    while (i < count)
                    {
                        //Read the data from the dataset. 
                        MovieDetail md = new MovieDetail();

                        md.Id = Convert.ToInt16(movieDS.Tables["Movie"].Rows[i]["Mo_Id"]);

                        md.Title = movieDS.Tables["Movie"].Rows[i]["Mo_Title"].ToString();
                        md.ReleaseYear = Convert.ToInt16(movieDS.Tables["Movie"].Rows[i]["Mo_ReleaseYear"]);
                        md.OnLanguage = (movieDS.Tables["Movie"].Rows[i]["Mo_OnLanguage"]).ToString();
                        md.Grade = Convert.ToInt16(movieDS.Tables["Movie"].Rows[i]["Mo_Grade"]);
                        md.Genre = (movieDS.Tables["Movie"].Rows[i]["Mo_Genre"]).ToString();

                        i++;
                        MovieList.Add(md);
                    }
                    errormsg = "";
                    return MovieList;
                }
                else
                {
                    errormsg = "No movies was fetched";
                    return (null);
                }
            }
            catch (Exception e)
            {
                errormsg = e.Message;
                return (null);
            }
            finally
            {
                dbConnection.Close();
            }

        }

        public MovieDetail GetOneMovie(int movie_id, out string errormsg)
        {

            SqlConnection dbConnection = new SqlConnection();

            dbConnection.ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Movies;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

            String sqlstring = "SELECT Mo_Id, Mo_Title, Mo_ReleaseYear, La_Language AS Mo_OnLanguage, Mo_Grade, Ge_Title AS Mo_Genre FROM" +
                "((Tbl_Movie INNER JOIN Tbl_Languages ON Mo_OnLanguage = La_Id) " +
                "INNER JOIN Tbl_Genre ON Mo_Genre = Ge_Id)" +
                "WHERE Mo_Id = @id"; 
            SqlCommand dbCommand = new SqlCommand(sqlstring, dbConnection);

            dbCommand.Parameters.Add("id", SqlDbType.Int).Value = movie_id;

            //Create adapter 
            SqlDataAdapter movieAdapter = new SqlDataAdapter(dbCommand);
            DataSet movieDS = new DataSet();

            MovieDetail md = new MovieDetail();

            try
            {
                dbConnection.Open();

                //Fill the dataset with data in a table named Movie.
                movieAdapter.Fill(movieDS, "Movie");

                int count = 0;

                count = movieDS.Tables["Movie"].Rows.Count;

                if (count == 1)
                {
                    //Read the data from the dataset.

                    md.Id = Convert.ToInt16(movieDS.Tables["Movie"].Rows[0]["Mo_Id"]);

                    md.Title = movieDS.Tables["Movie"].Rows[0]["Mo_Title"].ToString();
                    md.ReleaseYear = Convert.ToInt16(movieDS.Tables["Movie"].Rows[0]["Mo_ReleaseYear"]);
                    md.OnLanguage = (movieDS.Tables["Movie"].Rows[0]["Mo_OnLanguage"]).ToString();
                    md.Grade = Convert.ToInt16(movieDS.Tables["Movie"].Rows[0]["Mo_Grade"]);
                    md.Genre = (movieDS.Tables["Movie"].Rows[0]["Mo_Genre"]).ToString();
                    
                    errormsg = "";
                    return md;
                }
                else if (count < 1)
                {
                    errormsg = "No movies was fetched";
                    return (null);
                }
                else
                {
                    errormsg = "Too many movies was fetched";
                    return (null);
                }
            }
            catch (Exception e)
            {
                errormsg = e.Message;
                return (null);
            }
            finally
            {
                dbConnection.Close();
            }

        }

        public List<String> GetMoviesCorrespondingTo(int actor_id, out string errormsg)
        {
            SqlConnection dbConnection = new SqlConnection();

            dbConnection.ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Movies;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

            String sqlstring = "SELECT Mo_Title " +
                "FROM Tbl_Movie, Tbl_MovieActor WHERE MA_Actor = @Id AND Mo_Id = MA_Movie";
            SqlCommand dbCommand = new SqlCommand(sqlstring, dbConnection);

            dbCommand.Parameters.Add("Id", SqlDbType.Int).Value = actor_id;

            //Create adapter 
            SqlDataAdapter movieAdapter = new SqlDataAdapter(dbCommand);
            DataSet movieDS = new DataSet();

            List<String> MovieList = new List<String>();

            try
            {
                dbConnection.Open();

                movieAdapter.Fill(movieDS, "Movie");

                int count = 0;
                int i = 0;

                count = movieDS.Tables["Movie"].Rows.Count;

                if (count > 0)
                {
                    while (i < count)
                    {
                        //Read the data from the dataset.
                        MovieList.Add(movieDS.Tables["Movie"].Rows[i]["Mo_Title"].ToString());
                        i++;
                    }
                    errormsg = "";
                    return MovieList;
                }
                else
                {
                    errormsg = "No movies was fetched";
                    return (null);
                }
            }
            catch (Exception e)
            {
                errormsg = e.Message;
                return (null);
            }
            finally
            {
                dbConnection.Close();
            }

        }
    }
}

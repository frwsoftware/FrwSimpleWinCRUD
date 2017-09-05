using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FrwSoftware.Model.TestChinook;
using System.Threading;

namespace FrwSoftware
{
    public class PerformanceTest
    {

        static public void Test(int count, bool compare)
        {
            StringBuilder mes = new StringBuilder();
            TestFrwSimpleJsonORMDatabse(mes, count, false);
            if (compare)
            {
                mes.Append("\r\n");
                mes.Append("\r\n");
                TestFrwSimpleJsonORMDatabse(mes, count, true);
                mes.Append("\r\n");
                mes.Append("\r\n");
                TestSQlite(mes, count);
            }
            MessageBox.Show(mes.ToString());

        }


        static public void TestFrwSimpleJsonORMDatabse(StringBuilder mes, int count, bool saveFileOnEachOperation)
        {
            //prepare
            Dm.Instance.DeleteAllObjects(typeof(Album));
            Dm.Instance.DeleteAllObjects(typeof(Artist));

            int i = 0;
            Artist artist;
            for (i = 0; i < count; i++)
            {
                artist = (Artist)Dm.Instance.EmptyObject(typeof(Artist), null);
                artist.Name = "Artist " + i;
                Dm.Instance.SaveObject(artist);
                //if (saveFileOnEachOperation) Dm.Instance.SaveDataList(typeof(Artist));
            }

            Album album;
            i = 0;
            long tstart = DateTime.Now.Ticks;
            for (i = 0; i < count; i++)
            {
                album = (Album)Dm.Instance.EmptyObject(typeof(Album), null);
                album.Title = "Album " + i;
                Dm.Instance.SaveObject(album);
                if (saveFileOnEachOperation) Dm.Instance.SaveEntityData(typeof(Album));
            }
            long tend = DateTime.Now.Ticks;
            mes.Append("Generated " + (saveFileOnEachOperation ? "(save after each operation) " : "") + i + " records with time " + (tend - tstart) / 10000 + " ms. " + (tend - tstart)/i + " ticks per record");
            Thread.Sleep(1000); //Sleep to give the system time to recover for next run

            mes.Append("\r\n");
            tstart = DateTime.Now.Ticks;
            for (i = 0; i < count; i++)
            {
                album = (Album)Dm.Instance.Find<Album>(i.ToString());
            }
            tend = DateTime.Now.Ticks;
            mes.Append("Find by PK " + i + " records with time " + (tend - tstart) / 10000 + " ms. " + (tend - tstart) / i + " ticks per record");
            Thread.Sleep(1000); //Sleep to give the system time to recover for next run

           
            IList<Album> list = Dm.Instance.FindAll<Album>();
            IList<Artist> arlist = Dm.Instance.FindAll<Artist>();



            mes.Append("\r\n");
            list = Dm.Instance.FindAll<Album>();
            tstart = DateTime.Now.Ticks;
            foreach (var a in list)
            {
                a.Title = a.Title + " Updated";
                Dm.Instance.SaveObject(a);
                if (saveFileOnEachOperation) Dm.Instance.SaveEntityData(typeof(Album));
            }
            tend = DateTime.Now.Ticks;
            mes.Append("Update " + (saveFileOnEachOperation ? "(save) " : "") + i + " records with time " + (tend - tstart) / 10000 + " ms. " + (tend - tstart) / i + " ticks per record");
            Thread.Sleep(1000); //Sleep to give the system time to recover for next run

            mes.Append("\r\n");
            tstart = DateTime.Now.Ticks;
            i = 0;
            foreach (var a in list)
            {
                a.Artist = arlist[i];
                Dm.Instance.SaveObject(a);
                if (saveFileOnEachOperation) Dm.Instance.SaveEntityData(typeof(Album));
                i++;
            }
            tend = DateTime.Now.Ticks;
            mes.Append("Update relation " + (saveFileOnEachOperation ? "(save) " : "") + i + " records with time " + (tend - tstart) / 10000 + " ms. " + (tend - tstart) / i + " ticks per record");
            Thread.Sleep(1000); //Sleep to give the system time to recover for next run

            mes.Append("\r\n");
            tstart = DateTime.Now.Ticks;
            i = 0;
            foreach (var a in arlist)
            {
                a.Albums.Clear();
                Dm.Instance.SaveObject(a);
                if (saveFileOnEachOperation) Dm.Instance.SaveEntityData(typeof(Artist));
                i++;
            }
            tend = DateTime.Now.Ticks;
            mes.Append("Update relation reverse " + (saveFileOnEachOperation ? "(save) " : "") + i + " records with time " + (tend - tstart) / 10000 + " ms. " + (tend - tstart) / i + " ticks per record");
            Thread.Sleep(1000); //Sleep to give the system time to recover for next run

   
            mes.Append("\r\n");
            list = Dm.Instance.FindAll<Album>();
            List<Album> listCopy = new List<Album>();
            listCopy.AddRange(list);
            tstart = DateTime.Now.Ticks;
            foreach (var a in listCopy)
            {
                Dm.Instance.DeleteObject(a);
                if (saveFileOnEachOperation) Dm.Instance.SaveEntityData(typeof(Album));
            }
            tend = DateTime.Now.Ticks;
            mes.Append("Delete " + (saveFileOnEachOperation ? "(save) ": "") + i + " records with time " + (tend - tstart) / 10000+ " ms. " + (tend - tstart) / i + " ticks per record");

            //clear
            Dm.Instance.DeleteAllObjects(typeof(Album));
            Dm.Instance.DeleteAllObjects(typeof(Artist));
        }

        static public void TestSQlite(StringBuilder mes, int count)
        {
            /////////////////////////sqlite for comparation 
            SQLiteConnection m_dbConnection =
                new SQLiteConnection("Data Source=Data\\Profile\\sqlite\\Chinook_Album_ForTest.db;Version=3;");
            m_dbConnection.Open();
            //prepare
            string sql = "DELETE FROM Album";
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();

            //test
            int i = 0;
            mes.Append("\r\n");
            long tstart = DateTime.Now.Ticks;
            for (i = 0; i < count; i++)
            {
                sql = "INSERT INTO Album (AlbumId, Title, ArtistId) VALUES('" + i + "','" + "Album " + i + "', '1'  );";
                command = new SQLiteCommand(sql, m_dbConnection);
                command.ExecuteNonQuery();
            }
            long tend = DateTime.Now.Ticks;
            mes.Append("SQLite: Insert " + i + " records with time " + (tend - tstart) / 10000 + " ms. " + (tend - tstart) / i + " ticks per record");

            i = 0;
            mes.Append("\r\n");
            tstart = DateTime.Now.Ticks;
            for (i = 0; i < count; i++)
            {
                sql = "SELECT * FROM Album WHERE AlbumId = '" + i + "';";
                command = new SQLiteCommand(sql, m_dbConnection);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                }
                reader.Close();
            }
            tend = DateTime.Now.Ticks;
            mes.Append("SQLite: Find by PK " + i + " records with time " + (tend - tstart) / 10000 + " ms. " + (tend - tstart) / i + " ticks per record");

            i = 0;
            mes.Append("\r\n");
            tstart = DateTime.Now.Ticks;
            for (i = 0; i < count; i++)
            {
                sql = "UPDATE Album SET Title = '" + "Album " + i + " updated" + "' WHERE AlbumId = '" + i + "';";
                command = new SQLiteCommand(sql, m_dbConnection);
                command.ExecuteNonQuery();
            }
            tend = DateTime.Now.Ticks;
            mes.Append("SQLite: Update " + i + " records with time " + (tend - tstart) / 10000 + " ms. " + (tend - tstart) / i + " ticks per record");

            i = 0;
            mes.Append("\r\n");
            tstart = DateTime.Now.Ticks;
            for (i = 0; i < count; i++)
            {
                sql = "DELETE FROM Album  WHERE AlbumId = '" + i + "';";
                command = new SQLiteCommand(sql, m_dbConnection);
                command.ExecuteNonQuery();
            }
            tend = DateTime.Now.Ticks;
            mes.Append("SQLite: Delete " + i + " records with time " + (tend - tstart) / 10000 + " ms. " + (tend - tstart) / i + " ticks per record");

            //clear
            sql = "DELETE FROM Album";
            command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();

            m_dbConnection.Close();
            //////////////////////////////

        }

    }
}

/**********************************************************************************
 *   FrwSimpleWinCRUD   https://github.com/frwsoftware/FrwSimpleWinCRUD
 *   The Open-Source Library for most quick  WinForm CRUD application creation
 *   MIT License Copyright (c) 2016 FrwSoftware
 *
 *   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *   OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 *   SOFTWARE.
 **********************************************************************************/
using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using FrwSoftware;
using FrwSoftware.Model.Chinook;
using System.Collections.Generic;
using System.Reflection;

namespace FrwSimpleWinCRUDUnitTestProject
{
    [TestClass]
    public class DmUnitTest
    {
        [ClassInitialize]
        static public void Init(TestContext context)
        {
            FrwConfig.Instance = new FrwSimpleWinCRUDConfig();
            FrwConfig.Instance.ProfileDir = "Data\\TestProfile";//set custom profile name to prevent override demo database 
            MainAppUtils.InitAppPaths();

            Console.WriteLine("FrwConfig.Instance.GlobalDir: " + FrwConfig.Instance.GlobalDir);
            Console.WriteLine("FrwConfig.Instance.ProfileDir: " + FrwConfig.Instance.ProfileDir);
            Console.WriteLine("FrwConfig.Instance.ComputerUserDir: " + FrwConfig.Instance.ComputerUserDir);
            Console.WriteLine("FrwConfig.Instance.UserTempDir: " + FrwConfig.Instance.UserTempDir);

        }

        [TestMethod]
        public void TestMethod()
        {
            Assert.IsFalse(Dm.Instance.IsEntityModified(typeof(Album)));
            Assert.IsFalse(Dm.Instance.IsEntityModified(typeof(Artist)));
            Assert.IsFalse(Dm.Instance.IsEntityModified(typeof(Playlist)));
            Assert.IsFalse(Dm.Instance.IsEntityModified(typeof(Track)));
            Assert.IsFalse(Dm.Instance.IsEntityModified(typeof(Employee)));
            Assert.IsFalse(Dm.Instance.IsJoinEntityModified(typeof(Track), typeof(Playlist) , null));

            Album album0 = (Album)Dm.Instance.EmptyObject(typeof(Album), null);
            Album album2 = (Album)Dm.Instance.EmptyObject(typeof(Album), null);

            Assert.IsFalse(Dm.Instance.IsEntityModified(typeof(Album)));

            Dm.Instance.SaveObject(album0);
            Dm.Instance.SaveObject(album2);

            Assert.IsTrue(Dm.Instance.IsEntityModified(typeof(Album)));
            Dm.Instance.SaveEntityData(typeof(Album));
            Assert.IsFalse(Dm.Instance.IsEntityModified(typeof(Album)));

            Track track0 = (Track)Dm.Instance.EmptyObject(typeof(Track), null);
            track0.Name = "000";
            Track track2 = (Track)Dm.Instance.EmptyObject(typeof(Track), null);
            track2.Name = "222";
            Track track3 = (Track)Dm.Instance.EmptyObject(typeof(Track), null);
            track3.Name = "333";
            Dm.Instance.SaveObject(track0);
            Dm.Instance.SaveObject(track2);
            Dm.Instance.SaveObject(track3);

            //simple find
            Assert.AreSame(album0, (Album)Dm.Instance.Find(typeof(Album), album0.AlbumId));

            IEnumerable<Track> found =  Dm.Instance.FindByParams<Track>(new Dictionary<string, object> { { "Name", "222" }});
            Assert.IsFalse(found.Contains(track0));
            Assert.IsTrue(found.Contains(track2));
            Assert.IsFalse(found.Contains(track3));

            //many to one
            //set
            track0.Album = album2;
            Dm.Instance.SaveObject(track0);
            Assert.IsNull(album0.Tracks.FirstOrDefault<Track>());
            Assert.AreSame(album2.Tracks.FirstOrDefault<Track>(), track0);
            //unset
            track0.Album = null;
            Dm.Instance.SaveObject(track0);
            Assert.IsFalse(album2.Tracks.Contains(track0));

            //reverse set
            track0.Album = album2;
            Dm.Instance.SaveObject(track0);
            album2.Tracks.Remove(track0);
            Dm.Instance.SaveObject(album2);
            Assert.AreEqual(album2.Tracks.Count, 0);
            Assert.IsNull(track0.Album);

            //set unsaved
            Album albumNotSaved = (Album)Dm.Instance.EmptyObject(typeof(Album), null);
            track0.Album = albumNotSaved;
            try
            {
                Dm.Instance.SaveObject(track0);
                Assert.Fail("No exeption generated when update with not saved object");
            }
            catch
            {
                track0.Album = null;
            }


            ////one to many 
            //add
            album0.Tracks.Add(track0);
            Dm.Instance.SaveObject(album0);
            Track t1 = album0.Tracks.FirstOrDefault<Track>();
            Assert.AreSame(t1, track0);
            Assert.AreSame(track0.Album, album0);
            //check is modified
            Dm.Instance.SaveEntityData(typeof(Album));
            Assert.IsFalse(Dm.Instance.IsEntityModified(typeof(Album)));
            Dm.Instance.SaveEntityData(typeof(Track));
            Assert.IsFalse(Dm.Instance.IsEntityModified(typeof(Track)));
            //set
            album2.Tracks.Add(track0);
            Dm.Instance.SaveObject(album2);
            Assert.AreEqual(track0.Album, album2);
            Assert.IsTrue(album2.Tracks.Contains(track0));
            Assert.IsFalse(album0.Tracks.Contains(track0));
            //set
            album0.Tracks.Add(track0);
            Dm.Instance.SaveObject(album0);
            Assert.AreEqual(track0.Album, album0);
            Assert.IsTrue(album0.Tracks.Contains(track0));
            Assert.IsFalse(album2.Tracks.Contains(track0));

            Assert.IsTrue(Dm.Instance.IsEntityModified(typeof(Album)));
            Assert.IsFalse(Dm.Instance.IsEntityModified(typeof(Track)));

            Track trackNotSaved = (Track)Dm.Instance.EmptyObject(typeof(Track), null);
            album2.Tracks.Add(trackNotSaved);
            try
            {
                Dm.Instance.SaveObject(track0);
                Assert.Fail("No exeption generated when update with not saved object");
            }
            catch {
                album2.Tracks.Remove(trackNotSaved);
            }


            //many to many
            Dm.Instance.SaveEntityData(typeof(Track));
            Dm.Instance.SaveEntityData(typeof(Playlist));
            Assert.IsFalse(Dm.Instance.IsJoinEntityModified(typeof(Track), typeof(Playlist), null));
            Assert.IsFalse(Dm.Instance.IsEntityModified(typeof(Track)));
            Assert.IsFalse(Dm.Instance.IsEntityModified(typeof(Playlist)));

            Playlist playlist0 = (Playlist)Dm.Instance.EmptyObject<Playlist>(null);
            Playlist playlist2 = (Playlist)Dm.Instance.EmptyObject<Playlist>(null);
            Playlist playlist3 = (Playlist)Dm.Instance.EmptyObject<Playlist>(null);

            Dm.Instance.SaveObject(playlist0);
            Dm.Instance.SaveObject(playlist2);
            Dm.Instance.SaveObject(playlist3);

            track0.Playlists.Add(playlist0);
            track0.Playlists.Add(playlist2);
            Dm.Instance.SaveObject(track0);

            track2.Playlists.Add(playlist2);
            track2.Playlists.Add(playlist3);
            Dm.Instance.SaveObject(track2);

            Assert.IsTrue(track0.Playlists.Contains(playlist0));
            Assert.IsTrue(track0.Playlists.Contains(playlist2));
            Assert.IsFalse(track0.Playlists.Contains(playlist3));

            Assert.IsFalse(track2.Playlists.Contains(playlist0));
            Assert.IsTrue(track2.Playlists.Contains(playlist2));
            Assert.IsTrue(track2.Playlists.Contains(playlist3));

            Assert.IsFalse(track3.Playlists.Contains(playlist0));
            Assert.IsFalse(track3.Playlists.Contains(playlist2));
            Assert.IsFalse(track3.Playlists.Contains(playlist3));

            Assert.IsTrue(playlist0.Tracks.Contains(track0));
            Assert.IsFalse(playlist0.Tracks.Contains(track2));
            Assert.IsFalse(playlist0.Tracks.Contains(track3));

            Assert.IsTrue(playlist2.Tracks.Contains(track0));
            Assert.IsTrue(playlist2.Tracks.Contains(track2));
            Assert.IsFalse(playlist2.Tracks.Contains(track3));

            Assert.IsFalse(playlist3.Tracks.Contains(track0));
            Assert.IsTrue(playlist3.Tracks.Contains(track2));
            Assert.IsFalse(playlist3.Tracks.Contains(track3));

            Assert.IsTrue(Dm.Instance.IsEntityModified(typeof(Track)));
            Assert.IsTrue(Dm.Instance.IsEntityModified(typeof(Playlist)));
            Assert.IsTrue(Dm.Instance.IsJoinEntityModified(typeof(Track), typeof(Playlist), null));
            Dm.Instance.SaveEntityData(typeof(Track));
            Dm.Instance.SaveEntityData(typeof(Playlist));
            Assert.IsFalse(Dm.Instance.IsEntityModified(typeof(Track)));
            Assert.IsFalse(Dm.Instance.IsEntityModified(typeof(Playlist)));
            Assert.IsFalse(Dm.Instance.IsJoinEntityModified(typeof(Track), typeof(Playlist), null));

            //change
            track2.Playlists.Add(playlist0);
            Dm.Instance.SaveObject(track2);

            Assert.IsTrue(track0.Playlists.Contains(playlist0));
            Assert.IsTrue(track0.Playlists.Contains(playlist2));
            Assert.IsFalse(track0.Playlists.Contains(playlist3));

            Assert.IsTrue(track2.Playlists.Contains(playlist0));
            Assert.IsTrue(track2.Playlists.Contains(playlist2));
            Assert.IsTrue(track2.Playlists.Contains(playlist3));

            Assert.IsFalse(track3.Playlists.Contains(playlist0));
            Assert.IsFalse(track3.Playlists.Contains(playlist2));
            Assert.IsFalse(track3.Playlists.Contains(playlist3));

            Assert.IsTrue(playlist0.Tracks.Contains(track0));
            Assert.IsTrue(playlist0.Tracks.Contains(track2));
            Assert.IsFalse(playlist0.Tracks.Contains(track3));

            Assert.IsTrue(playlist2.Tracks.Contains(track0));
            Assert.IsTrue(playlist2.Tracks.Contains(track2));
            Assert.IsFalse(playlist2.Tracks.Contains(track3));

            Assert.IsFalse(playlist3.Tracks.Contains(track0));
            Assert.IsTrue(playlist3.Tracks.Contains(track2));
            Assert.IsFalse(playlist3.Tracks.Contains(track3));


            Assert.IsTrue(Dm.Instance.IsEntityModified(typeof(Track)));
            Assert.IsTrue(Dm.Instance.IsEntityModified(typeof(Playlist)));
            Assert.IsTrue(Dm.Instance.IsJoinEntityModified(typeof(Track), typeof(Playlist), null));
            Dm.Instance.SaveEntityData(typeof(Track));
            Dm.Instance.SaveEntityData(typeof(Playlist));
            Assert.IsFalse(Dm.Instance.IsEntityModified(typeof(Track)));
            Assert.IsFalse(Dm.Instance.IsEntityModified(typeof(Playlist)));
            Assert.IsFalse(Dm.Instance.IsJoinEntityModified(typeof(Track), typeof(Playlist), null));

            //change
            track2.Playlists.Remove(playlist0);
            track2.Playlists.Remove(playlist2);
            Dm.Instance.SaveObject(track2);

            track3.Playlists.Add(playlist3);
            Dm.Instance.SaveObject(track3);


            Assert.IsTrue(track0.Playlists.Contains(playlist0));
            Assert.IsTrue(track0.Playlists.Contains(playlist2));
            Assert.IsFalse(track0.Playlists.Contains(playlist3));

            Assert.IsFalse(track2.Playlists.Contains(playlist0));
            Assert.IsFalse(track2.Playlists.Contains(playlist2));
            Assert.IsTrue(track2.Playlists.Contains(playlist3));

            Assert.IsFalse(track3.Playlists.Contains(playlist0));
            Assert.IsFalse(track3.Playlists.Contains(playlist2));
            Assert.IsTrue(track3.Playlists.Contains(playlist3));

            Assert.IsTrue(playlist0.Tracks.Contains(track0));
            Assert.IsFalse(playlist0.Tracks.Contains(track2));
            Assert.IsFalse(playlist0.Tracks.Contains(track3));

            Assert.IsTrue(playlist2.Tracks.Contains(track0));
            Assert.IsFalse(playlist2.Tracks.Contains(track2));
            Assert.IsFalse(playlist2.Tracks.Contains(track3));

            Assert.IsFalse(playlist3.Tracks.Contains(track0));
            Assert.IsTrue(playlist3.Tracks.Contains(track2));
            Assert.IsTrue(playlist3.Tracks.Contains(track3));

            Assert.IsTrue(Dm.Instance.IsEntityModified(typeof(Track)));
            Assert.IsTrue(Dm.Instance.IsEntityModified(typeof(Playlist)));
            Assert.IsTrue(Dm.Instance.IsJoinEntityModified(typeof(Track), typeof(Playlist), null));
            Dm.Instance.SaveEntityData(typeof(Track));
            Dm.Instance.SaveEntityData(typeof(Playlist));
            Assert.IsFalse(Dm.Instance.IsEntityModified(typeof(Track)));
            Assert.IsFalse(Dm.Instance.IsEntityModified(typeof(Playlist)));
            Assert.IsFalse(Dm.Instance.IsJoinEntityModified(typeof(Track), typeof(Playlist), null));


            Playlist PlaylistNotSaved = (Playlist)Dm.Instance.EmptyObject(typeof(Playlist), null);
            track0.Playlists.Add(PlaylistNotSaved);
            try
            {
                Dm.Instance.SaveObject(track0);
                Assert.Fail("No exeption generated when update with not saved object");
            }
            catch {
                track0.Playlists.Remove(PlaylistNotSaved);
            }


            //self relation 
            Employee employee0 = (Employee)Dm.Instance.EmptyObject(typeof(Employee), null);
            Employee employee2 = (Employee)Dm.Instance.EmptyObject(typeof(Employee), null);
            Employee employee3 = (Employee)Dm.Instance.EmptyObject(typeof(Employee), null);
            Dm.Instance.SaveObject(employee0);
            Dm.Instance.SaveObject(employee2);
            Dm.Instance.SaveObject(employee3);

            //Dm.Instance.ResolveToManyRelations(employee0);
            //Dm.Instance.ResolveToManyRelations(employee2);
            //Dm.Instance.ResolveToManyRelations(employee3);

            employee0.ReportsToManager = employee2;
            employee2.ReportsToManager = employee3;
            Dm.Instance.SaveObject(employee0);
            Dm.Instance.SaveObject(employee2);

            Assert.IsTrue(employee0.WhoReportsToManager.Count == 0);
            Assert.IsTrue(employee2.WhoReportsToManager.Contains(employee0));
            Assert.IsTrue(employee3.WhoReportsToManager.Contains(employee2));

            //end 
            //remove
            Dm.Instance.DeleteObject(album0);
            Assert.IsFalse(Dm.Instance.FindAll<Album>().Contains(album0));
            Assert.IsTrue(Dm.Instance.FindAll<Album>().Contains(album2));
            Dm.Instance.DeleteObject(album2);
            Assert.IsFalse(Dm.Instance.FindAll<Album>().Contains(album2));

            Dm.Instance.DeleteAllObjects(typeof(Album));
            Assert.IsTrue(Dm.Instance.FindAll<Album>().Count == 0);

            Dm.Instance.DeleteAllObjects(typeof(Track));
            Assert.IsTrue(Dm.Instance.FindAll<Track>().Count == 0);



            //test cascade deletion 

            TestDto1 t1_1 = (TestDto1)Dm.Instance.EmptyObject(typeof(TestDto1), null);
            TestDto1 t1_2 = (TestDto1)Dm.Instance.EmptyObject(typeof(TestDto1), null);
            TestDto1 t1_3 = (TestDto1)Dm.Instance.EmptyObject(typeof(TestDto1), null);
            TestDto1 t1_4 = (TestDto1)Dm.Instance.EmptyObject(typeof(TestDto1), null);
            TestDto1 t1_5 = (TestDto1)Dm.Instance.EmptyObject(typeof(TestDto1), null);
            TestDto1 t1_6 = (TestDto1)Dm.Instance.EmptyObject(typeof(TestDto1), null);
            TestDto1 t1_7 = (TestDto1)Dm.Instance.EmptyObject(typeof(TestDto1), null);
            TestDto1 t1_8 = (TestDto1)Dm.Instance.EmptyObject(typeof(TestDto1), null);
            TestDto1 t1_9 = (TestDto1)Dm.Instance.EmptyObject(typeof(TestDto1), null);
            TestDto1 t1_10 = (TestDto1)Dm.Instance.EmptyObject(typeof(TestDto1), null);
            TestDto1 t1_11 = (TestDto1)Dm.Instance.EmptyObject(typeof(TestDto1), null);
            TestDto1 t1_12 = (TestDto1)Dm.Instance.EmptyObject(typeof(TestDto1), null);
            Dm.Instance.SaveObject(t1_1);
            Dm.Instance.SaveObject(t1_2);
            Dm.Instance.SaveObject(t1_3);
            Dm.Instance.SaveObject(t1_4);
            Dm.Instance.SaveObject(t1_5);
            Dm.Instance.SaveObject(t1_6);
            Dm.Instance.SaveObject(t1_7);
            Dm.Instance.SaveObject(t1_8);
            Dm.Instance.SaveObject(t1_9);
            Dm.Instance.SaveObject(t1_10);
            Dm.Instance.SaveObject(t1_11);
            Dm.Instance.SaveObject(t1_12);

            TestDto2 t2_1 = (TestDto2)Dm.Instance.EmptyObject(typeof(TestDto2), null);
            TestDto2 t2_2 = (TestDto2)Dm.Instance.EmptyObject(typeof(TestDto2), null);
            Dm.Instance.SaveObject(t2_1);
            Dm.Instance.SaveObject(t2_2);
            TestDto3 t3_1 = (TestDto3)Dm.Instance.EmptyObject(typeof(TestDto3), null);
            TestDto3 t3_2 = (TestDto3)Dm.Instance.EmptyObject(typeof(TestDto3), null);
            Dm.Instance.SaveObject(t3_1);
            Dm.Instance.SaveObject(t3_2);
            TestDto4 t4_1 = (TestDto4)Dm.Instance.EmptyObject(typeof(TestDto4), null);
            TestDto4 t4_2 = (TestDto4)Dm.Instance.EmptyObject(typeof(TestDto4), null);
            Dm.Instance.SaveObject(t4_1);
            Dm.Instance.SaveObject(t4_2);
            TestDto5 t5_1 = (TestDto5)Dm.Instance.EmptyObject(typeof(TestDto5), null);
            TestDto5 t5_2 = (TestDto5)Dm.Instance.EmptyObject(typeof(TestDto5), null);
            Dm.Instance.SaveObject(t5_1);
            Dm.Instance.SaveObject(t5_2);

            TestDto6 t6_1 = (TestDto6)Dm.Instance.EmptyObject(typeof(TestDto6), null);
            TestDto6 t6_2 = (TestDto6)Dm.Instance.EmptyObject(typeof(TestDto6), null);
            Dm.Instance.SaveObject(t6_1);
            Dm.Instance.SaveObject(t6_2);

            TestDto7 t7_1 = (TestDto7)Dm.Instance.EmptyObject(typeof(TestDto7), null);
            TestDto7 t7_2 = (TestDto7)Dm.Instance.EmptyObject(typeof(TestDto7), null);
            Dm.Instance.SaveObject(t7_1);
            Dm.Instance.SaveObject(t7_2);

            TestDto8 t8_1 = (TestDto8)Dm.Instance.EmptyObject(typeof(TestDto8), null);
            TestDto8 t8_2 = (TestDto8)Dm.Instance.EmptyObject(typeof(TestDto8), null);
            Dm.Instance.SaveObject(t8_1);
            Dm.Instance.SaveObject(t8_2);

            //set relationships
            t1_1.TestDto2 = t2_1;
            t1_1.TestDto2_1 = t2_2;
            Dm.Instance.SaveObject(t1_1);
            t1_2.TestDto3s.Add(t3_1);
            Dm.Instance.SaveObject(t1_2);
            t1_3.TestDto4s.Add(t4_1);
            Dm.Instance.SaveObject(t1_3);
            t5_1.TestDto1s.Add(t1_4);
            Dm.Instance.SaveObject(t5_1);
            t1_5.TestDto6 = t6_1;
            Dm.Instance.SaveObject(t1_5);
            t7_1.TestDto1 = t1_6;
            Dm.Instance.SaveObject(t7_1);
            Assert.IsTrue(t7_1.TestDto1 == t1_6);
            t1_7.TestDto8s.Add(t8_1);
            Dm.Instance.SaveObject(t1_7);
            //self
            t1_8.ParentTestDto1 = t1_9;
            Dm.Instance.SaveObject(t1_8);
            t1_10.ParentTestDto1 = t1_10;
            Dm.Instance.SaveObject(t1_10);
            t1_11.ParentTestDto1 = t1_12;
            Dm.Instance.SaveObject(t1_11);

            //test
            Assert.IsTrue(t2_1.TestDto1s.Contains(t1_1));
            Assert.IsTrue(t2_2.TestDto1_1s.Contains(t1_1));
            Dm.Instance.DeleteObject(t1_1);
            Assert.IsFalse(t2_1.TestDto1s.Contains(t1_1));

            Assert.IsTrue(t3_1.TestDto1s.Contains(t1_2));
            Dm.Instance.DeleteObject(t1_2);
            Assert.IsFalse(t3_1.TestDto1s.Contains(t1_2));

            //Assert.IsTrue(t4_1.TestDto1s.Contains(t1_3));
            Dm.Instance.DeleteObject(t1_3);
            //Assert.IsFalse(t4_1.TestDto1s.Contains(t1_3));
            //todo

            Assert.IsTrue(t5_1.TestDto1s.Contains(t1_4));
            Dm.Instance.DeleteObject(t1_4);
            Assert.IsFalse(t5_1.TestDto1s.Contains(t1_4));

            Dm.Instance.DeleteObject(t1_5);

            Assert.IsTrue(t7_1.TestDto1 == t1_6);
            Dm.Instance.DeleteObject(t1_6);
            Assert.IsTrue(t7_1.TestDto1 == null);

            Assert.IsTrue(t8_1.TestDto1 == t1_7);
            Assert.IsTrue(t1_7.TestDto8s.Contains(t8_1));
            Dm.Instance.DeleteObject(t1_7);
            Assert.IsTrue(t8_1.TestDto1 == null);
            //Assert.IsFalse(t1_7.TestDto8s.Contains(t8_1));//not necessary - t1_7 deleted 

            Assert.IsTrue(t1_9.ChildTestDto1s.Contains(t1_8));
            Dm.Instance.DeleteObject(t1_8);
            Assert.IsFalse(t1_9.ChildTestDto1s.Contains(t1_8));

            Dm.Instance.DeleteObject(t1_10);//test selfe referenced 

            Assert.IsTrue(t1_11.ParentTestDto1 == t1_12);
            Dm.Instance.DeleteObject(t1_12);
            Assert.IsTrue(t1_11.ParentTestDto1 == null);
        }

        [ClassCleanup]
        static  public void Destroy()
        {
        }
    }
}

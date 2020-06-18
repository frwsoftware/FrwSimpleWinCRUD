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
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace FrwSoftware
{
    public class TypeHelper
    {
        static public object FindTypeAddCreateNewInstance(string fullTypeName)
        {
            Type type = FindType(fullTypeName);
            if (type == null) throw new InvalidOperationException("Type not found for: " + fullTypeName);
            return Activator.CreateInstance(type);
        }
        static public Type FindType(string fullTypeName)
        {
            //this method provide full search 
            //http://stackoverflow.com/questions/12422744/searching-type-in-assemblies
            //another alternative - find in assemlies only from current directory 

            //if this method will work slow, we can optimize it by ovveride and set short list of dll to find

            Type myType = Type.GetType(fullTypeName);//find in current dll
            if (myType == null)
            {
                //find in all allready loaded 
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

                try
                {
                    myType = assemblies.SelectMany(a => a.GetTypes())
                                .FirstOrDefault(t => t.FullName == fullTypeName);
                }
                catch (Exception ex0)
                {
                    //we need detalized information about with assembly gets this error 
                    Log.LogError("Error when finding type for : " + fullTypeName, ex0);
                    foreach (var a1 in assemblies)
                    {
                        try
                        {
                            a1.GetTypes();
                        }
                        catch (System.Reflection.ReflectionTypeLoadException ex)
                        {
                            Log.LogError("Error loading assembly for name: " + a1, ex);
                            if (ex.LoaderExceptions != null)
                            {
                                foreach (var e in ex.LoaderExceptions)
                                {
                                    Log.LogError("LoaderExceptions", e);
                                }
                            }
                        }
                    }
                    throw ex0;
                }

                if (myType == null)
                {
                    //find in all referenced dll
                    Assembly asm = Assembly.GetEntryAssembly();//entry 
                    AssemblyName[] asms = asm.GetReferencedAssemblies();//ref
                    foreach (AssemblyName aname in asms)
                    {
                        if (assemblies.FirstOrDefault(a => a.FullName == aname.FullName) == null)//not loaded yet
                        {
                            //load it
                            Assembly a = Assembly.Load(aname);
                            //try to find type
                            myType = a.GetTypes().FirstOrDefault(t => t.FullName == fullTypeName);
                            if (myType != null) break;
                        }
                    }
                }
            }
            return myType;
        }
        static public Image LookupImageResource(Type resourceManagerProvider, string resourceKey)
        {

            //foreach (PropertyInfo staticProperty in resourceManagerProvider.GetProperties(BindingFlags.Static | BindingFlags.NonPublic)) - do not working on public Resources
            foreach (PropertyInfo staticProperty in resourceManagerProvider.GetProperties())
            {
                if (staticProperty.PropertyType == typeof(System.Resources.ResourceManager))
                {
                    System.Resources.ResourceManager resourceManager = (System.Resources.ResourceManager)staticProperty.GetValue(null, null);
                    return (Image)resourceManager.GetObject(resourceKey);
                }
            }

            return null;
        }
  
        static public Image FindImageInAllDiskStorages(string imageName)
        {
            FileInfo fi = new FileInfo(Path.Combine(FrwConfig.Instance.GlobalDir, imageName));
            if (fi.Exists)
                return Image.FromFile(fi.FullName, true);
            else return null;
        }


        //////////////////////////////////////////
        //todo
        #region PreLoadAllAssemblies
        //from https://stackoverflow.com/questions/3021613/how-to-pre-load-all-deployed-assemblies-for-an-appdomain
        static public void PreLoadAllAssemblies()
        {
            AssembliesFromApplicationBaseDirectory();
        }

        static void AssembliesFromApplicationBaseDirectory()
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            AssembliesFromPath(baseDirectory);

            string privateBinPath = AppDomain.CurrentDomain.SetupInformation.PrivateBinPath;
            if (Directory.Exists(privateBinPath))
                AssembliesFromPath(privateBinPath);
        }

        static void AssembliesFromPath(string path)
        {
            var assemblyFiles = Directory.GetFiles(path)
                .Where(file => Path.GetExtension(file).Equals(".dll", StringComparison.OrdinalIgnoreCase));

            foreach (var assemblyFile in assemblyFiles)
            {
                // TODO: check it isnt already loaded in the app domain
                Assembly.LoadFrom(assemblyFile);
            }
        }
        #endregion

    }
}

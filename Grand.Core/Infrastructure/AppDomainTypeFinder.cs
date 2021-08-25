using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Grand.Core.Infrastructure
{
    /// <summary>
    /// A class that finds types needed by Grand by looping assemblies in the 
    /// currently executing AppDomain. 
    /// </summary>
    public class AppDomainTypeFinder : ITypeFinder
    {

        #region Methods

        public IEnumerable<Type> FindClassesOfType<T>(bool onlyConcreteClasses = true)
        {
            return FindClassesOfType(typeof(T), onlyConcreteClasses);
        }

        public IEnumerable<Type> FindClassesOfType(Type assignTypeFrom, bool onlyConcreteClasses = true)
        {
            return FindClassesOfType(assignTypeFrom, GetAssemblies(), onlyConcreteClasses);
        }

        public IEnumerable<Type> FindClassesOfType<T>(IEnumerable<Assembly> assemblies, bool onlyConcreteClasses = true)
        {
            return FindClassesOfType(typeof(T), assemblies, onlyConcreteClasses);
        }

        public IEnumerable<Type> FindClassesOfType(Type assignTypeFrom, IEnumerable<Assembly> assemblies, bool onlyConcreteClasses = true)
        {
            var result = new List<Type>();
            try
            {
                assemblies.Select(x => x.GetTypes())
                    .Where(x => x != null)
                    .SelectMany(x => x)
                    .Where(x => assignTypeFrom.IsAssignableFrom(x) || assignTypeFrom.IsGenericTypeDefinition)
                    .Where(x => DoesTypeImplementOpenGeneric(x, assignTypeFrom))
                    .Where(x => !x.IsInterface)
                    .Where(x => !onlyConcreteClasses || x.IsClass && !x.IsAbstract)
                    .ToList()
                    .ForEach(result.Add);
            }
            catch (ReflectionTypeLoadException ex)
            {
                var fail = FlattenException(ex);
                Debug.WriteLine(fail.Message, fail);

                throw fail;
            }
            return result;
        }

        private static Exception FlattenException(ReflectionTypeLoadException ex)
        {
            var msg = string.Empty;
            foreach (var e in ex.LoaderExceptions)
                msg += e.Message + Environment.NewLine;

            return new Exception(msg, ex);
        }

        // <summary>
        /// Does type implement generic?
        /// </summary>
        /// <param name="type"></param>
        /// <param name="openGeneric"></param>
        /// <returns></returns>
        protected virtual bool DoesTypeImplementOpenGeneric(Type type, Type openGeneric)
        {
            try
            {
                var genericTypeDefinition = openGeneric.GetGenericTypeDefinition();
                return type.FindInterfaces((objType, objCriteria) => true, null)
                    .Where(x => x.IsGenericType)
                    .Any(x => genericTypeDefinition.IsAssignableFrom(x.GetGenericTypeDefinition()));
            }
            catch
            {
                return false;
            }
        }

        /// <summary>Gets the assemblies related to the current implementation.</summary>
        /// <returns>A list of assemblies that should be loaded by the Grand factory.</returns>
        public virtual IList<Assembly> GetAssemblies()
        {
            var addedAssemblyNames = new List<string>();
            var assemblies = new List<Assembly>();

            AddAssembliesInAppDomain(addedAssemblyNames, assemblies);

            return assemblies;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Iterates all assemblies in the AppDomain and if it's name matches the configured patterns add it to our list.
        /// </summary>
        /// <param name="addedAssemblyNames"></param>
        /// <param name="assemblies"></param>
        private void AddAssembliesInAppDomain(List<string> addedAssemblyNames, List<Assembly> assemblies)
        {
            AddAssemblies(addedAssemblyNames, assemblies);

            if (Roslyn.RoslynCompiler.ReferencedScripts == null)
            {
                return;
            }

            AddScripts(addedAssemblyNames, assemblies);
        }

        private static void AddAssemblies(List<string> addedAssemblyNames, List<Assembly> assemblies)
        {
            Assembly currentAssem = Assembly.GetExecutingAssembly();
            AppDomain.CurrentDomain.GetAssemblies()
                .Where(x => x.GetReferencedAssemblies().ToList().Where(
                    x => x.FullName == currentAssem.FullName).Any() ||
                    x.GetCustomAttribute<AssemblyProductAttribute>()?.Product == "grandnode")
                .Where(x => addedAssemblyNames.Contains(x.FullName))
                .ToList()
                .ForEach(x =>
                {
                    assemblies.Add(x);
                    addedAssemblyNames.Add(x.FullName);
                });
        }

        private static void AddScripts(List<string> addedAssemblyNames, List<Assembly> assemblies)
        {
            Roslyn.RoslynCompiler.ReferencedScripts
                .Where(x => !string.IsNullOrEmpty(x.ReferencedAssembly.FullName))
                .Where(x => !addedAssemblyNames.Contains(x.ReferencedAssembly.FullName))
                .ToList()
                .ForEach(x =>
                {
                    assemblies.Add(x.ReferencedAssembly);
                    addedAssemblyNames.Add(x.ReferencedAssembly.FullName);
                });
        }

        /// <summary>
        /// Makes sure matching assemblies in the supplied folder are loaded in the app domain.
        /// </summary>
        /// <param name="directoryPath">
        /// The physical path to a directory containing dlls to load in the app domain.
        /// </param>
        protected virtual void LoadMatchingAssemblies()
        {
            var loadedAssemblyNames = new List<string>();
            foreach (Assembly a in GetAssemblies())
            {
                loadedAssemblyNames.Add(a.FullName);
            }
        }

        #endregion
    }
}

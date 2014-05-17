using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.SymbolStore;
using System.Reflection;
using System.Reflection.Emit;

namespace EmitDebugging.Tests
{
	public abstract class AssemblyCreationTests
	{
		private static void AddDebuggingAttribute(AssemblyBuilder assembly)
		{
			var debugAttribute = typeof(DebuggableAttribute);
			var debugConstructor = debugAttribute.GetConstructor(
				new Type[] { typeof(DebuggableAttribute.DebuggingModes) });
			var debugBuilder = new CustomAttributeBuilder(
				debugConstructor, new object[] { 
					DebuggableAttribute.DebuggingModes.DisableOptimizations | 
					DebuggableAttribute.DebuggingModes.Default });
			assembly.SetCustomAttribute(debugBuilder);
		}

		protected static AssemblyDebugging CreateDebuggingAssembly(string name)
		{
			var assemblyName = new AssemblyName();
			assemblyName.Name = name;
			var assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(
				assemblyName, AssemblyBuilderAccess.Save);
			DebuggingTests.AddDebuggingAttribute(assembly);

			var module = assembly.DefineDynamicModule(assemblyName.Name,
				assemblyName.Name + ".dll", true);

			var symbolWriter = module.DefineDocument(
				 assemblyName.Name + ".il", SymDocumentType.Text,
				 SymLanguageType.ILAssembly, SymLanguageVendor.Microsoft);

			return new AssemblyDebugging(assemblyName.Name + ".il", assembly, symbolWriter);
		}

		[SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
		[SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1")]
		protected static TypeDebugging CreateDebuggingType(
			AssemblyDebugging assembly, ModuleBuilder module, string name)
		{
			return assembly.GetTypeDebugging(module.DefineType(
				assembly.Builder.GetName().Name + "." + name,
				TypeAttributes.Class | TypeAttributes.Sealed |
				TypeAttributes.Public, typeof(object)));
		}

		[SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
		[SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1")]
		protected static TypeDebugging CreateDebuggingType(
			AssemblyDebugging assembly, ModuleBuilder module, string name, HashSet<Type> interfacesToImplement)
		{
			return assembly.GetTypeDebugging(module.DefineType(
				assembly.Builder.GetName().Name + "." + name,
				TypeAttributes.Class | TypeAttributes.Sealed |
				TypeAttributes.Public, typeof(object)), interfacesToImplement);
		}
	}
}

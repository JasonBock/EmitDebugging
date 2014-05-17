using System.IO;
using System.Xml.XPath;
using AssemblyVerifier;
using Xunit;

namespace EmitDebugging.Tests
{
	public abstract class DebuggingTests
		: AssemblyCreationTests
	{
		private static void AssertFileContents(string[] expectedLines, string[] actualLines)
		{
			Assert.Equal(expectedLines.Length, actualLines.Length);

			for (var i = 0; i < expectedLines.Length; i++)
			{
				Assert.Equal(expectedLines[i], actualLines[i]);
			}
		}

		protected abstract void AssertSequencePoints(XPathNavigator navigator);

		protected abstract AssemblyDebugging CreateAssembly();

		protected string CreateAssemblyAndVerify(bool verifyAssembly)
		{
			var assembly = this.CreateAssembly();
			var assemblyName = assembly.Builder.GetName().Name;

			assembly.Builder.Save(assemblyName + ".dll");

			if (verifyAssembly)
			{
				AssemblyVerification.Verify(assembly.Builder);
			}

			return assemblyName;
		}

		protected abstract string[] GetExpectedFileLines();

		protected void RunTest()
		{
			this.RunTest(true);
		}

		protected void RunTest(bool verifyAssembly)
		{
			var assemblyName = this.CreateAssemblyAndVerify(verifyAssembly);

			DebuggingTests.AssertFileContents(this.GetExpectedFileLines(),
				File.ReadAllLines(assemblyName + ".il"));

			XmlPdbReader.Program.PdbToXML(assemblyName + ".pdb", assemblyName + ".xml");

			this.AssertSequencePoints(new XPathDocument(assemblyName + ".xml").CreateNavigator());
		}
	}
}

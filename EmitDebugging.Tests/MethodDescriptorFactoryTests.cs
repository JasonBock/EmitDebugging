using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Xunit;

namespace EmitDebugging.Tests
{
	public sealed class MethodDescriptorFactoryTests
		: AssemblyCreationTests
	{
		[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "U")]
		[SuppressMessage("Microsoft.Naming", "CA1715:IdentifiersShouldHaveCorrectPrefix", MessageId = "T")]
		[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
		public void AGenericMethod<T, U>() { }

		[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
		public void AMethod() { }

		[Fact]
		public void GetDescriptionFromGenericMethod()
		{
			var descriptor = MethodDescriptorFactory.Create(this.GetType().GetMethod("AGenericMethod"));
			Assert.Equal(typeof(MethodGenericDeclarationDescriptor), descriptor.GetType());
			Assert.Equal(".method public hidebysig instance void AGenericMethod<T, U>() cil managed",
				descriptor.Value);
		}

		[Fact]
		public void GetDescriptionFromGenericMethodInvocation()
		{
			var method = this.GetType().GetMethod("AGenericMethod").MakeGenericMethod(
				new Type[] { typeof(Guid), typeof(Random) });
			var descriptor = MethodDescriptorFactory.Create(method);
			Assert.Equal(typeof(MethodGenericInvocationDescriptor), descriptor.GetType());
			Assert.Equal(
				"instance void EmitDebugging.Tests.MethodDescriptorFactoryTests::AGenericMethod<valuetype [mscorlib]System.Guid, class [mscorlib]System.Random>()",
				descriptor.Value);
		}

		[Fact]
		public void GetDescriptionFromMethod()
		{
			var descriptor = MethodDescriptorFactory.Create(this.GetType().GetMethod("AMethod"), true);
			Assert.Equal(typeof(MethodDescriptor), descriptor.GetType());
			Assert.Equal(".method public hidebysig instance void AMethod() cil managed",
				descriptor.Value);
		}

		[Fact]
		public void GetDescriptionFromMethodWithDifferentContainingAssembly()
		{
			var descriptor = MethodDescriptorFactory.Create(
				this.GetType().GetMethod("AMethod"), typeof(Guid).Assembly);
			Assert.Equal(typeof(MethodDescriptor), descriptor.GetType());
			Assert.Equal("instance void [EmitDebugging.Tests]EmitDebugging.Tests.MethodDescriptorFactoryTests::AMethod()",
				descriptor.Value);
		}

		[Fact]
		public void GetDescriptionFromMethodInvocation()
		{
			var descriptor = MethodDescriptorFactory.Create(this.GetType().GetMethod("AMethod"), false);
			Assert.Equal(typeof(MethodDescriptor), descriptor.GetType());
			Assert.Equal("instance void EmitDebugging.Tests.MethodDescriptorFactoryTests::AMethod()",
				descriptor.Value);
		}

		[Fact]
		public static void GetDescriptionFromGenericMethodBuilder()
		{
			using (var assembly = AssemblyCreationTests.CreateDebuggingAssembly("BuilderGenericDeclaration"))
			{
				using (var type = AssemblyCreationTests.CreateDebuggingType(
					assembly, assembly.Builder.GetDynamicModule(assembly.Builder.GetName().Name), "Holder"))
				{
					using (var method = type.GetMethodDebugging(
						type.Builder.DefineMethod("Method",
						MethodAttributes.HideBySig | MethodAttributes.Public)))
					{
						method.Builder.DefineGenericParameters("T", "U", "V");

						var descriptor = MethodDescriptorFactory.Create(method.Builder, true);
						Assert.Equal(typeof(MethodGenericDeclarationDescriptor), descriptor.GetType());
						Assert.Equal(".method public hidebysig instance void Method<T, U, V>() cil managed",
							descriptor.Value);
					}
				}
			}
		}

		public static void GetDescriptionFromMethodBuilder()
		{
			using (var assembly = AssemblyCreationTests.CreateDebuggingAssembly("BuilderGenericDeclaration"))
			{
				using (var type = AssemblyCreationTests.CreateDebuggingType(
					assembly, assembly.Builder.GetDynamicModule(assembly.Builder.GetName().Name), "Holder"))
				{
					using (var method = type.GetMethodDebugging(
						type.Builder.DefineMethod("Method",
						MethodAttributes.HideBySig | MethodAttributes.Public)))
					{
						var descriptor = MethodDescriptorFactory.Create(method.Builder, true);
						Assert.Equal(typeof(MethodDescriptor), descriptor.GetType());
						Assert.Equal(".method public hidebysig instance void Method() cil managed",
							descriptor.Value);
					}
				}
			}
		}
	}
}
